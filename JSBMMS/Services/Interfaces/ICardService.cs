using JSBMMS.Models;

namespace JSBMMS.Services.Interfaces;

public interface ICardService
{
  Task<CardDetails?> GetCardDetails(string userId, string cardNumber);
}
