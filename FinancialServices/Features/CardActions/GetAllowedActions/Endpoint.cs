using FinancialServices.API.Domain;
using FinancialServices.API.Features.CardActions.GetAllowedActions;
using Microsoft.AspNetCore.Mvc;

namespace FinancialServices.API.Features.CardActions.GetAllowedActions;

public static class Endpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/allowed", HandleAsync)
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
        if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.CardNumber))
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Validation error",
                Detail = "Both userId and cardNumber are required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var details = await cardService.GetCardDetails(request.UserId.Trim(), request.CardNumber.Trim());
        if (details is null)
        {
            return Results.Problem(new ProblemDetails
            {
                Title = "Not found",
                Detail = "Card not found for given userId and cardNumber.",
                Status = StatusCodes.Status404NotFound
            });
        }

        var allowed = AllowedActionsCalculator.GetAllowedAsStrings(details);
        // if enums should be returned:
        // var allowed = AllowedActionsCalculator.GetAllowed(details);

        var response = new Response(
            allowed
        );

        return Results.Ok(response);
    }
}
