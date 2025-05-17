namespace JSBMMS.Models;

public enum CardType
{
  Prepaid,
  Debit,
  Credit
}

public enum CardStatus
{
  Ordered,
  Inactive,
  Active,
  Restricted,
  Blocked,
  Expired,
  Closed
}
public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);

