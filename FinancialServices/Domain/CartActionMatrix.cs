using System.Collections.ObjectModel;

namespace FinancialServices.API.Domain;

public enum CardAction
{
    ACTION1,
    ACTION2,
    ACTION3,
    ACTION4,
    ACTION5,
    ACTION6,
    ACTION7,
    ACTION8,
    ACTION9,
    ACTION10,
    ACTION11,
    ACTION12,
    ACTION13
}

/// <summary>
/// Stałe tablice z dostępnością akcji z tabeli zadania:
/// - AllowedByType: kolumny CARD KIND (PREPAID/DEBIT/CREDIT)
/// - AllowedByStatus: kolumny CARD STATUS (ORDERED..CLOSED), z warunkami PIN
/// </summary>
public static class CardActionMatrix
{
    // ========= CARD KIND (z lewej części tabeli) =========
    // Jeśli w kolumnie dla danego typu jest "TAK" → akcja jest dopuszczalna dla typu.
    // Jeśli "NIE" → akcja dla tego typu nie może wystąpić, niezależnie od statusu.
    public static readonly IReadOnlyDictionary<CardType, IReadOnlySet<CardAction>> AllowedByType =
        new ReadOnlyDictionary<CardType, IReadOnlySet<CardAction>>(
            new Dictionary<CardType, IReadOnlySet<CardAction>>
            {
                // Na podstawie kolumn PREPAID / DEBIT / CREDIT
                [CardType.Prepaid] = new HashSet<CardAction>
                {
                    CardAction.ACTION1,
                    CardAction.ACTION2,
                    CardAction.ACTION3,
                    CardAction.ACTION4,
                    // Wg tabeli: ACTION5 dla Prepaid = NIE (nie dodajemy)
                    CardAction.ACTION6,
                    CardAction.ACTION7,
                    CardAction.ACTION8,
                    CardAction.ACTION9,
                    CardAction.ACTION10,
                    CardAction.ACTION11,
                    CardAction.ACTION12,
                    CardAction.ACTION13
                },
                [CardType.Debit] = new HashSet<CardAction>
                {
                    CardAction.ACTION1,
                    CardAction.ACTION2,
                    CardAction.ACTION3,
                    CardAction.ACTION4,
                    CardAction.ACTION5,
                    CardAction.ACTION6,
                    CardAction.ACTION7,
                    CardAction.ACTION8,
                    CardAction.ACTION9,
                    CardAction.ACTION10,
                    CardAction.ACTION11,
                    CardAction.ACTION12,
                    CardAction.ACTION13
                },
                [CardType.Credit] = new HashSet<CardAction>
                {
                    CardAction.ACTION1,
                    CardAction.ACTION2,
                    CardAction.ACTION3,
                    CardAction.ACTION4,
                    CardAction.ACTION5,
                    CardAction.ACTION6,
                    CardAction.ACTION7,
                    CardAction.ACTION8,
                    CardAction.ACTION9,
                    CardAction.ACTION10,
                    CardAction.ACTION11,
                    CardAction.ACTION12,
                    CardAction.ACTION13
                },
            });

    // ========= CARD STATUS (prawa część tabeli) =========
    // Każdy status ma 3 zbiory:
    //  - Always: "TAK"
    //  - RequiresPinSet: "TAK – jeżeli pin nadany" / "TAK – ale jak nie ma pin to NIE"
    //  - RequiresNoPin: "TAK – jeżeli brak pin"
    public sealed record StatusAvailability(
        IReadOnlySet<CardAction> Always,
        IReadOnlySet<CardAction> RequiresPinSet,
        IReadOnlySet<CardAction> RequiresNoPin
    );

    public static readonly IReadOnlyDictionary<CardStatus, StatusAvailability> AllowedByStatus =
        new ReadOnlyDictionary<CardStatus, StatusAvailability>(
            new Dictionary<CardStatus, StatusAvailability>
            {
                // ===== ORDERED =====
                [CardStatus.Ordered] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION8,
                        CardAction.ACTION9,
                        CardAction.ACTION10,
                        CardAction.ACTION11,
                        CardAction.ACTION12,
                        CardAction.ACTION13
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        CardAction.ACTION6 // "TAK – ale jak nie ma pin to NIE"
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                        CardAction.ACTION7 // "TAK – jeżeli brak pin"
                    }
                ),

                // ===== INACTIVE =====
                [CardStatus.Inactive] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION8,
                        CardAction.ACTION9,
                        CardAction.ACTION10,
                        CardAction.ACTION11,
                        CardAction.ACTION12,
                        CardAction.ACTION13
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        CardAction.ACTION6
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                        CardAction.ACTION7
                    }
                ),

                // ===== ACTIVE =====
                [CardStatus.Active] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION1,
                        CardAction.ACTION2,
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION8,
                        CardAction.ACTION9,
                        CardAction.ACTION10,
                        CardAction.ACTION11,
                        CardAction.ACTION12,
                        CardAction.ACTION13
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        CardAction.ACTION6
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                        CardAction.ACTION7
                    }
                ),

                // ===== RESTRICTED =====
                [CardStatus.Restricted] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION1,
                        CardAction.ACTION2,
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION8,
                        CardAction.ACTION9,
                        CardAction.ACTION10,
                        CardAction.ACTION11,
                        CardAction.ACTION12,
                        CardAction.ACTION13
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        CardAction.ACTION6
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                        CardAction.ACTION7
                    }
                ),

                // ===== BLOCKED =====
                [CardStatus.Blocked] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        // z tabeli: 1 = NIE, 2 = TAK, 3 = TAK, 4 = TAK, 5 = TAK,
                        CardAction.ACTION2,
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION8,
                        CardAction.ACTION9,
                        CardAction.ACTION10,
                        CardAction.ACTION11,
                        CardAction.ACTION12
                        // 13 = NIE
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        // z tabeli: "TAK – jeżeli pin nadany"
                        CardAction.ACTION6,
                        CardAction.ACTION7
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                        // brak pozycji „jeżeli brak pin” dla BLOCKED
                    }
                ),

                // ===== EXPIRED =====
                [CardStatus.Expired] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        // z tabeli: 3,4,5,9,11,12,13 = TAK
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION9,
                        CardAction.ACTION11,
                        CardAction.ACTION12,
                        CardAction.ACTION13
                    },
                    RequiresPinSet: new HashSet<CardAction>(),
                    RequiresNoPin: new HashSet<CardAction>()
                ),

                // ===== CLOSED =====
                [CardStatus.Closed] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        // z tabeli: 3,4,9 = TAK (reszta NIE)
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION9
                    },
                    RequiresPinSet: new HashSet<CardAction>(),
                    RequiresNoPin: new HashSet<CardAction>()
                ),
            });

    /// <summary>
    /// Zwraca finalny zestaw akcji dla (type, status, isPinSet), tj. część wspólną
    /// AllowedByType[type] oraz:
    ///  - AllowedByStatus[status].Always
    ///  - + AllowedByStatus[status].RequiresPinSet (jeśli isPinSet)
    ///  - + AllowedByStatus[status].RequiresNoPin (jeśli !isPinSet)
    /// </summary>
    public static IReadOnlyList<CardAction> GetAllowedActions(CardType type, CardStatus status, bool isPinSet)
    {
        if (!AllowedByType.TryGetValue(type, out var byType) ||
            !AllowedByStatus.TryGetValue(status, out var byStatus))
            return Array.Empty<CardAction>();

        var candidate = new HashSet<CardAction>(byStatus.Always);

        if (isPinSet)
            candidate.UnionWith(byStatus.RequiresPinSet);
        else
            candidate.UnionWith(byStatus.RequiresNoPin);

        // część wspólna z dopuszczalnymi dla typu
        candidate.IntersectWith(byType);

        return candidate.OrderBy(a => a).ToArray();
    }
}
