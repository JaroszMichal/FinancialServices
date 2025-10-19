using System.Collections.Concurrent;

namespace FinancialServices.API.Domain;

public enum CardLookupStatus
{
    Ok,
    UserNotFound,
    CardNotFound
}

public record CardLookupResult(CardLookupStatus Status, CardDetails? Details);

public class CardService
{
    private readonly Dictionary<string, Dictionary<string, CardDetails>> _userCards = CreateSampleUserCards();

    public async Task<CardLookupResult> GetCardDetails(string userId, string cardNumber)
    {
        // In production, you'd call an external API/DB; here we simulate latency.
        await Task.Delay(1000);

        if (!_userCards.TryGetValue(userId, out var cards))
        {
            return new CardLookupResult(CardLookupStatus.UserNotFound, null);
        }

        if (!cards.TryGetValue(cardNumber, out var cardDetails))
        {
            return new CardLookupResult(CardLookupStatus.CardNotFound, null);
        }

        return new CardLookupResult(CardLookupStatus.Ok, cardDetails);
    }

    private static Dictionary<string, Dictionary<string, CardDetails>> CreateSampleUserCards()
    {
        var userCards = new Dictionary<string, Dictionary<string, CardDetails>>();
        for (var i = 1; i <= 3; i++)
        {
            var cards = new Dictionary<string, CardDetails>();
            var cardIndex = 1;

            foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
                foreach (CardStatus cardStatus in Enum.GetValues(typeof(CardStatus)))
                {
                    var cardNumber = $"Card{i}{cardIndex}";
                    cards.Add(cardNumber,
                        new CardDetails(
                            CardNumber: cardNumber,
                            CardType: cardType,
                            CardStatus: cardStatus,
                            IsPinSet: cardIndex % 2 == 0));
                    cardIndex++;
                }

            var userId = $"User{i}";
            userCards.Add(userId, cards);
        }
        return userCards;
    }
}
