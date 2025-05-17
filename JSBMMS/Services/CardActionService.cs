using System.Text.Json;
using JSBMMS.Helpers;
using JSBMMS.Models;
using JSBMMS.Services.Interfaces;

namespace JSBMMS.Services;

public class CardActionService : ICardActionService
{
  private readonly string file = "actionRuleTakNie.json";
  private readonly string[] errorMessages = { "Brak dostępnego pliku", "Błędna struktura", "Plik" };
  private readonly List<ActionRule> _rule;

  public CardActionService()
  {
    _rule = LoadRuleFromFile();
  }

  public Task<List<string>> CheckActions(CardDetails cardDetails)
  {
    if (_rule.Count == 1 && errorMessages.Any(msg => _rule[0].ActionName.Contains(msg)))
    {
      return Task.FromResult(new List<string> { $"Błędny plik JSON lub jego brak." });
    }
    return Task.FromResult(GetAllowedActions(cardDetails));
  }


  private List<ActionRule> LoadRuleFromFile()
  {
    if (!File.Exists(file) || new FileInfo(file).Length == 0)
    {
      return new List<ActionRule>
      {
        new ActionRule { ActionName = "Brak dostępnego pliku - lub plik pusty."}
      };
    }
    return VerifyFileData();
  }


  private List<ActionRule> VerifyFileData()
  {
    try
    {
      string json = File.ReadAllText(file);
      if (string.IsNullOrWhiteSpace(json))
      {
        return new List<ActionRule>
        {
          new ActionRule { ActionName = "Plik jest pusty."}
        };
      }

      var rules = JsonSerializer.Deserialize<List<ActionRule>>(json);
      if (rules is null || rules.Count == 0)
      {
        return new List<ActionRule>
        {
          new ActionRule { ActionName = "Plik nie zawiera poprawnych danych"}
        };
      }
      return rules;
    }
    catch (JsonException e)
    {
      return new List<ActionRule>
        {
          new ActionRule{ ActionName = "Błędna struktura pliku JSON."}
        };
    }
  }

  private List<string> GetAllowedActions(CardDetails cardDetails)
  {
    if (_rule is null)
    {
      return new List<string>();
    }

    return _rule
        .Where(r =>
            r.CardTypeConditions?.TryGetValue(cardDetails.CardType.ToString(), out string typeCondition) == true &&
            !string.IsNullOrWhiteSpace(typeCondition) &&
            typeCondition == HelpStaticVal.TAK &&
            r.CardStatusConditions?.TryGetValue(cardDetails.CardStatus.ToString(), out string statusCondition) == true &&
            StatusConditionRule(statusCondition ?? HelpStaticVal.NIE, cardDetails.IsPinSet)
        )
        .Select(r => r.ActionName)
        .ToList();

  }

  private bool StatusConditionRule(string condition, bool isPinSet)
  {
    return StatusRules.TryGetValue(condition, out var rule) ? rule(isPinSet) : false;
  }

  private readonly Dictionary<string, Func<bool, bool>> StatusRules = new()
  {
    { "TAK", _ => true },
    { "NIE", _ => false },
    { "TAK_JESLNI_NIE_MA_PIN_TO_NIE", isPinSet => isPinSet },
    { "TAK_JSELI_PIN_NADANY", isPinSet => isPinSet },
    { "TAK_JESLNI_BRAK_PIN", isPinSet => !isPinSet }
  };
}
