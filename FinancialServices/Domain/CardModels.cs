namespace FinancialServices.API.Domain;

public record CardDetails(string CardNumber, CardType CardType, CardStatus CardStatus, bool IsPinSet);