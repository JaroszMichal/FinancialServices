using System.Linq;
using FinancialServices.API.Domain;

namespace FinancialServices.API.Features.CardActions.GetAllowedActions;

public static class AllowedActionsCalculator
{
    /// <summary>
    /// Zwraca dozwolone akcje jako enumy (preferowane w kodzie).
    /// Wykorzystuje stałą macierz reguł z CardActionMatrix.
    /// </summary>
    public static CardAction[] GetAllowed(CardDetails card)
        => CardActionMatrix
            .GetAllowedActions(card.CardType, card.CardStatus, card.IsPinSet)
            .ToArray();

    /// <summary>
    /// Wygodny wrapper, jeśli chcesz zwrócić teksty "ACTION1"..."ACTION13" do JSON-a.
    /// </summary>
    public static string[] GetAllowedAsStrings(CardDetails card)
        => GetAllowed(card)
            .Select(a => a.ToString())
            .ToArray();
}
