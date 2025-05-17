using JSBMMS.Models;

namespace JSBMMS.Services.Interfaces;

public interface ICardActionService
{
  Task<List<ActionRule>> GetActionsByRule();
  Task<List<string>> CheckActions(CardDetails cardDetails);
}
