using FinancialServices.API.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FinancialServices.Tests;

public class Endpoint_actionsallowedActions_TableTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Endpoint_actionsallowedActions_TableTests(WebApplicationFactory<Program> factory)
        => _factory = factory;

    // ===== helper: mapowanie indeksu 1..21 -> (CardType, CardStatus) + pin =====
    private static readonly CardType[] Types = { CardType.Prepaid, CardType.Debit, CardType.Credit };
    private static readonly CardStatus[] Statuses =
    {
        CardStatus.Ordered, CardStatus.Inactive, CardStatus.Active,
        CardStatus.Restricted, CardStatus.Blocked, CardStatus.Expired, CardStatus.Closed
    };

    private static (CardType type, CardStatus status, bool pin) FromIndex(int idx)
    {
        var zero = idx - 1; // 0..20
        var type = Types[zero / 7];
        var status = Statuses[zero % 7];
        var pin = idx % 2 == 0; // mock: IsPinSet for even index
        return (type, status, pin);
    }

    // ===== dane testowe: 3 użytkowników × 21 kart (== 63 przypadki z tabeli) =====
    public static IEnumerable<object[]> AllUsersAndCardIndexes()
    {
        for (int u = 1; u <= 3; u++)
            for (int idx = 1; idx <= 21; idx++)
                yield return new object[] { u, idx };
    }

    [Theory(DisplayName = "[API] /api/card-actions/actionsallowed matches hard-coded TABLE (all mocked cards)")]
    [MemberData(nameof(AllUsersAndCardIndexes))]
    public async Task Endpoint_Matches_Table_For_All_Mocked_Cards(int userNo, int idx)
    {
        var client = _factory.CreateClient(); // domyślnie uderzy w https://localhost:<port> z test hosta

        var userId = $"User{userNo}";
        var cardNo = $"Card{userNo}{idx}";
        var (type, status, pin) = FromIndex(idx);
        var expected = ExpectedByTypeAndStatus.Expected(type, status, pin);

        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed",
                                               new { userId, cardNumber = cardNo });

        var body = await res.Content.ReadAsStringAsync(); // dla debug
        Assert.True(res.IsSuccessStatusCode, $"Status={res.StatusCode}, Body={body}");

        var payload = await res.Content.ReadFromJsonAsync<AllowedDto>();
        Assert.NotNull(payload);
        Assert.NotNull(payload!.AllowedActions);

        Assert.Equal(
            expected,
            payload.AllowedActions!
                   .OrderBy(a => a, StringComparer.OrdinalIgnoreCase)
                   .ToArray()
        );
    }
    private sealed class AllowedDto
    {
        [JsonPropertyName("allowedActions")]
        public string[]? AllowedActions { get; set; }
    }
}
