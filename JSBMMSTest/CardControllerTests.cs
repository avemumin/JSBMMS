using JSBMMS.Controllers;
using JSBMMS.Models;
using JSBMMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JSBMMSTest;

public class CardControllerTests
{
  [Fact]
  public async Task GetActions_CardNotFound()
  {
    var cardService = new Mock<ICardService>();
    var actionService = new Mock<ICardActionService>();

    cardService.Setup(s => s.GetCardDetails(It.IsAny<string>(), It.IsAny<string>()))
      .ReturnsAsync((CardDetails?)null);

    var controller = new CardController(cardService.Object, actionService.Object);

    var result = await controller.GetActions("User1", "CardSomeDoesNotExists");

    Assert.IsType<NotFoundObjectResult>(result);

  }

  [Fact]
  public async Task GetActions_ReturnOK()
  {
    var cardDetails = new CardDetails("TestCard", CardType.Debit, CardStatus.Active, true);

    var cardService = new Mock<ICardService>();
    var actionService = new Mock<ICardActionService>();

    cardService.Setup(s => s.GetCardDetails(It.IsAny<string>(), It.IsAny<string>()))
     .ReturnsAsync(cardDetails);

    actionService.Setup(s => s.CheckActions(cardDetails))
      .ReturnsAsync(new List<string> { "ACTION1", "ACTION2" });

    var controller = new CardController(cardService.Object, actionService.Object);

    var result = await controller.GetActions("User1", "TestCard");

    var ok = Assert.IsType<OkObjectResult>(result);
    Assert.NotNull(ok.Value);
  }
}
