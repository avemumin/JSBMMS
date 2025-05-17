using JSBMMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JSBMMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardController : ControllerBase
{
  private readonly ICardService _cardService;
  private readonly ICardActionService _cardActionService;

  public CardController(ICardService cardService, ICardActionService cardActionService)
  {
    _cardService = cardService;
    _cardActionService = cardActionService;
  }

  [HttpGet("{userId}/{cardNumber}")]
  public async Task<IActionResult> GetActions(string userId, string cardNumber)
  {
    var cardDetails = await _cardService.GetCardDetails(userId, cardNumber);
    if (cardDetails is null)
    {
      return NotFound(new { Message = $"Nie odnaleziono karty: {cardNumber} użytkownika: {userId}" });
    }

    var actions = await _cardActionService.CheckActions(cardDetails);
    if (actions.FirstOrDefault() == "Błędny plik JSON lub jego brak.")
    {
      return BadRequest(new { Message = actions[0] });
    }

    if (!actions.Any())
    {
      return BadRequest(new { Message = "Brak dostępnych akcji dla wskazanych parametrów." });
    }

    return Ok(new { Card = cardDetails, AllowedActions = actions });
  }
}
