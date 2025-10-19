using FinancialServices.API.Domain;

namespace FinancialServices.Tests;

/// Hard-coded expectations from the PDF table (strings "ACTIONx")
public static class ExpectedByTypeAndStatus
{
    // CARD KIND
    public static readonly IReadOnlyDictionary<CardType, string[]> ByType =
        new Dictionary<CardType, string[]>
        {
            [CardType.Prepaid] = new[]
            {
                "ACTION1","ACTION2","ACTION3","ACTION4",
                // ACTION5 = NO for Prepaid
                "ACTION6","ACTION7","ACTION8","ACTION9","ACTION10","ACTION11","ACTION12","ACTION13"
            },
            [CardType.Debit] = new[]
            {
                "ACTION1","ACTION2","ACTION3","ACTION4",
                // ACTION5 = NO for Debit
                "ACTION6","ACTION7","ACTION8","ACTION9","ACTION10","ACTION11","ACTION12","ACTION13"
            },
            [CardType.Credit] = new[]
            {
                "ACTION1","ACTION2","ACTION3","ACTION4","ACTION5",
                "ACTION6","ACTION7","ACTION8","ACTION9","ACTION10","ACTION11","ACTION12","ACTION13"
            }
        };

    // CARD STATUS  -> (always, whenPinSet, whenNoPin)
    public static (string[] always, string[] pin, string[] nopin) ByStatus(CardStatus s) => s switch
    {
        CardStatus.Ordered => (
            new[] { "ACTION3", "ACTION4", "ACTION5", "ACTION8", "ACTION9", "ACTION10", "ACTION12", "ACTION13" },
            new[] { "ACTION6" },
            new[] { "ACTION7" }
        ),
        CardStatus.Inactive => (
            new[] { "ACTION2", "ACTION3", "ACTION4", "ACTION5", "ACTION8", "ACTION9", "ACTION10", "ACTION11", "ACTION12", "ACTION13" },
            new[] { "ACTION6" },
            new[] { "ACTION7" }
        ),
        CardStatus.Active => (
            new[] { "ACTION1", "ACTION3", "ACTION4", "ACTION5", "ACTION8", "ACTION9", "ACTION10", "ACTION11", "ACTION12", "ACTION13" },
            new[] { "ACTION6" },
            new[] { "ACTION7" }
        ),
        CardStatus.Restricted => (
            new[] { "ACTION3", "ACTION4", "ACTION5", "ACTION9" },
            Array.Empty<string>(),
            Array.Empty<string>()
        ),
        CardStatus.Blocked => (
            new[] { "ACTION3", "ACTION4", "ACTION5", "ACTION8", "ACTION9" },
            new[] { "ACTION6", "ACTION7" },
            Array.Empty<string>()
        ),
        CardStatus.Expired => (
            new[] { "ACTION3", "ACTION4", "ACTION5", "ACTION9" },
            Array.Empty<string>(),
            Array.Empty<string>()
        ),
        CardStatus.Closed => (
            new[] { "ACTION3", "ACTION4", "ACTION5", "ACTION9" },
            Array.Empty<string>(),
            Array.Empty<string>()
        ),
        _ => (Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>())
    };

    // Final expected result for (type,status,pin): ByType ∩ (ByStatus + PIN conditions)
    public static string[] Expected(CardType type, CardStatus status, bool pin)
    {
        var byType = ByType[type];
        var (always, whenPin, whenNoPin) = ByStatus(status);
        var union = always.Concat(pin ? whenPin : whenNoPin)
                          .Distinct(StringComparer.OrdinalIgnoreCase);
        return union.Intersect(byType, StringComparer.OrdinalIgnoreCase)
                    .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                    .ToArray();
    }
}
