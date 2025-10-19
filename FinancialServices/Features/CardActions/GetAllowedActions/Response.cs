namespace FinancialServices.API.Features.CardActions.GetAllowedActions;

public record Response(
    string UserId,
    string CardNumber,
    string CardType,
    string CardStatus,
    string[] AllowedActions
);