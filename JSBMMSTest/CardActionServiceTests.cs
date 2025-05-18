using JSBMMS.Models;
using JSBMMS.Services;
using JSBMMS.Services.Interfaces;
using Moq;
using NuGet.Frameworks;

namespace JSBMMSTest;

public class CardActionServiceTests : IDisposable
{
  private readonly string exampleFile = "actionRuleTakNie.json";

  [Fact]
  public async Task CheckActions_FileIsMissing_ReturnErrorMsg()
  {
    var expectedErrorMessage = "Błędny plik JSON lub jego brak.";
    if (File.Exists(exampleFile))
      File.Delete(exampleFile);

    var service = new Mock<ICardActionService>();
    var cardDetails = new CardDetails("TestCard", CardType.Debit, CardStatus.Restricted, true);

    service.Setup(s => s.CheckActions(It.IsAny<CardDetails>()))
      .ReturnsAsync(new List<string> { expectedErrorMessage });

    var action = await service.Object.CheckActions(cardDetails);

    Assert.Single(action);
    Assert.Equal(expectedErrorMessage, action.First());
  }

  [Fact]
  public async Task CheckActions_FileIsEmpty_ReturnEmtpyMessage()
  {
    var expectedEmptyMessage = "Plik jest pusty.";
    File.WriteAllText(exampleFile, "");

    var service = new Mock<ICardActionService>();
    var cardDetails = new CardDetails("TestCard", CardType.Debit, CardStatus.Restricted, true);

    service.Setup(s => s.CheckActions(It.IsAny<CardDetails>()))
   .ReturnsAsync(new List<string> { expectedEmptyMessage });

    var action = await service.Object.CheckActions(cardDetails);

    Assert.Single(action);
    Assert.Equal(expectedEmptyMessage, action.First());
  }

  [Fact]
  public async Task CheckActions_Valid_ReturnAllowed()
  {
    var jsonContent = @"
             [
                 {
                     ""ActionName"": ""ACTION1"",
                     ""CardTypeConditions"": { ""Debit"": ""TAK"" },
                     ""CardStatusConditions"": { ""Active"": ""TAK"" }
                 }
             ]";
    File.WriteAllText(exampleFile, jsonContent);

    var service = new CardActionService();
    var cardDetails = new CardDetails("TestCard", CardType.Debit, CardStatus.Active, true);
    var actions = await service.CheckActions(cardDetails);

    Assert.Single(actions);
    Assert.Equal("ACTION1", actions.First());

  }

  [Fact]
  public async Task CheckActions_Valid_NoMatchingAction()
  {
    var jsonContent = @"
             [
                 {
                     ""ActionName"": ""ACTION1"",
                     ""CardTypeConditions"": { ""Prepaid"": ""TAK"" },
                     ""CardStatusConditions"": { ""Ordered"": ""TAK"" }
                 }
             ]";
    File.WriteAllText(exampleFile, jsonContent);
    var service = new CardActionService();
    var cardDetails = new CardDetails("TestCard", CardType.Debit, CardStatus.Active, true);
    var action = await service.CheckActions(cardDetails);

    Assert.Empty(action);

  }

  public void Dispose()
  {
    if (File.Exists(exampleFile))
      File.Delete(exampleFile);
  }
}
