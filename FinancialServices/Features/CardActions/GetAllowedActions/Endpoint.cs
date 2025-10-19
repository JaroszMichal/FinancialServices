using FinancialServices.API.Domain;
using FinancialServices.API.Features.CardActions.GetAllowedActions;
using Microsoft.AspNetCore.Mvc;

namespace FinancialServices.API.Features.CardActions.GetAllowedActions;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/actionsallowed", HandleAsync)
             .WithName("GetAllowedCardActions")
             .WithSummary("Get allowed actions for a given card")
             .WithDescription("Accepts userId and cardNumber, returns the list of allowed actions for that card.")
             .Produces<Response>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] Request request,
        [FromServices] CardService cardService)
    {
        var userId = request.UserId?.Trim();
        var cardNumber = request.CardNumber?.Trim();

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(cardNumber))
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "Both userId and cardNumber are required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var lookup = await cardService.GetCardDetails(userId, cardNumber);

        if (lookup.Status == CardLookupStatus.UserNotFound)
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"User '{userId}' does not exist.",
                Status = StatusCodes.Status404NotFound
            });
        }

        if (lookup.Status == CardLookupStatus.CardNotFound)
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Card not found",
                Detail = $"User '{userId}' does not own card '{cardNumber}'.",
                Status = StatusCodes.Status404NotFound
            });
        }

        // OK
        var details = lookup.Details!;

        var allowed = AllowedActionsCalculator.GetAllowedAsStrings(details);
        var response = new Response(allowed);

        return Results.Ok(response);
    }
}