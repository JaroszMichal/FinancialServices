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
/// Constant tables with action availability from the task table:
/// - AllowedByType: CARD KIND columns (PREPAID/DEBIT/CREDIT)
/// - AllowedByStatus: CARD STATUS columns (ORDERED..CLOSED), with PIN conditions
/// </summary>
public static class CardActionMatrix
{
    // ========= CARD KIND (left part of the table) =========
    // If the column for a given type says "YES" → the action is allowed for that type.
    // If it says "NO" → the action cannot occur for that type, regardless of status.
    public static readonly IReadOnlyDictionary<CardType, IReadOnlySet<CardAction>> AllowedByType =
        new ReadOnlyDictionary<CardType, IReadOnlySet<CardAction>>(
            new Dictionary<CardType, IReadOnlySet<CardAction>>
            {
                // Based on PREPAID / DEBIT / CREDIT columns
                [CardType.Prepaid] = new HashSet<CardAction>
                {
                    CardAction.ACTION1,
                    CardAction.ACTION2,
                    CardAction.ACTION3,
                    CardAction.ACTION4,
                    // According to the table: ACTION5 for Prepaid = NIE (do not include)
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
                    // According to the table: ACTION5 for Debit = NIE (do not include)
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

    // ========= CARD STATUS (right part of the table) =========
    // Each status has 3 sets:
    //  - Always: "YES"
    //  - RequiresPinSet: "YES – if PIN is set" / "YES – but if there is no PIN then NO"
    //  - RequiresNoPin: "YES – if there is no PIN"
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
                        CardAction.ACTION12,
                        CardAction.ACTION13
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        CardAction.ACTION6 // "YES – but if there is no PIN then NO"
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                        CardAction.ACTION7 // "YES – if there is no PIN"
                    }
                ),

                // ===== INACTIVE =====
                [CardStatus.Inactive] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
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

                // ===== ACTIVE =====
                [CardStatus.Active] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION1,
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
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION9
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                    },
                    RequiresNoPin: new HashSet<CardAction>
                    {
                    }
                ),

                // ===== BLOCKED =====
                [CardStatus.Blocked] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION8,
                        CardAction.ACTION9
                    },
                    RequiresPinSet: new HashSet<CardAction>
                    {
                        // from the table: "YES – if PIN is set"
                        CardAction.ACTION6,
                        CardAction.ACTION7
                    },
                    RequiresNoPin: new HashSet<CardAction>()
                ),

                // ===== EXPIRED =====
                [CardStatus.Expired] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION9
                    },
                    RequiresPinSet: new HashSet<CardAction>(),
                    RequiresNoPin: new HashSet<CardAction>()
                ),

                // ===== CLOSED =====
                [CardStatus.Closed] = new StatusAvailability(
                    Always: new HashSet<CardAction>
                    {
                        CardAction.ACTION3,
                        CardAction.ACTION4,
                        CardAction.ACTION5,
                        CardAction.ACTION9
                    },
                    RequiresPinSet: new HashSet<CardAction>(),
                    RequiresNoPin: new HashSet<CardAction>()
                ),
            });

    /// <summary>
    /// Returns the final set of actions for (type, status, isPinSet), i.e., the intersection of
    /// AllowedByType[type] and:
    ///  - AllowedByStatus[status].Always
    ///  - + AllowedByStatus[status].RequiresPinSet (if isPinSet)
    ///  - + AllowedByStatus[status].RequiresNoPin (if !isPinSet)
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

        // intersection with actions allowed for the given type
        candidate.IntersectWith(byType);

        return candidate.OrderBy(a => a).ToArray();
    }
}
