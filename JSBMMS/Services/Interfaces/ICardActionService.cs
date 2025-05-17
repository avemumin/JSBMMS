using JSBMMS.Models;

namespace JSBMMS.Services.Interfaces;

public interface ICardActionService
{
  Task<List<string>> CheckActions(CardDetails cardDetails);
}
