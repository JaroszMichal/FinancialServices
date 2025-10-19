using FinancialServices.API.Domain;

namespace FinancialServices.API.Features.CardActions.GetAllowedActions;

public static class AllowedActionsCalculator
{
    /// <summary>
    /// Returns allowed actions as enums (preferred in code).
    /// Uses the constant rules matrix from CardActionMatrix.
    /// </summary>
    public static CardAction[] GetAllowed(CardDetails card)
        => CardActionMatrix
            .GetAllowedActions(card.CardType, card.CardStatus, card.IsPinSet)
            .ToArray();

    /// <summary>
    /// Convenience wrapper if you want to return "ACTION1"..."ACTION13" strings in JSON.
    /// </summary>
    public static string[] GetAllowedAsStrings(CardDetails card)
        => GetAllowed(card)
            .Select(a => a.ToString())
            .ToArray();
}