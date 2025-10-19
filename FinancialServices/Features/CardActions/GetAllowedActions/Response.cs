namespace FinancialServices.API.Features.CardActions.GetAllowedActions;

public record Response(
    //The task requirements demand that only the list of actions be returned.
    string[] AllowedActions
);