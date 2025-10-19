namespace FinancialServices.API.Domain;

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