using JSBMMS.Models;
using JSBMMS.Services.Interfaces;

namespace JSBMMS.Services;

public class CardActionService : ICardActionService
{
  public Task<List<string>> CheckActions(CardDetails cardDetails)
  {
    throw new NotImplementedException();
  }

  public Task<List<ActionRule>> GetActionsByRule()
  {
    throw new NotImplementedException();
  }
}
