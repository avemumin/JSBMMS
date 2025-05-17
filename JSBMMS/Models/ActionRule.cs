namespace JSBMMS.Models;

public class ActionRule
{
  public string ActionName { get; set; }
  public Dictionary<string, string>? CardTypeConditions { get; set; }
  public Dictionary<string, string>? CardStatusCondition { get; set; }
}
