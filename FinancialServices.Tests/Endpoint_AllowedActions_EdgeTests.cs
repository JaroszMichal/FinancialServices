using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace FinancialServices.Tests;

public class Endpoint_actionsallowedActions_EdgeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Endpoint_actionsallowedActions_EdgeTests(WebApplicationFactory<Program> factory)
        => _factory = factory;

    [Fact(DisplayName = "[API] 400 when userId missing")]
    public async Task Missing_UserId_Returns_400()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed", new { userId = "", cardNumber = "Card11" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact(DisplayName = "[API] 400 when cardNumber missing")]
    public async Task Missing_CardNumber_Returns_400()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed", new { userId = "User1", cardNumber = "" });
        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
    }

    [Fact(DisplayName = "[API] 404 when user does not exist")]
    public async Task Unknown_User_Returns_404()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed", new { userId = "NoSuchUser", cardNumber = "Card11" });
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact(DisplayName = "[API] 404 when card does not exist for user")]
    public async Task Unknown_Card_For_User_Returns_404()
    {
        var client = _factory.CreateClient();
        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed", new { userId = "User1", cardNumber = "Nope" });
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
    }

    [Fact(DisplayName = "[API] content-type application/json required")]
    public async Task ContentType_Json_Required()
    {
        var client = _factory.CreateClient();
        using var req = new HttpRequestMessage(HttpMethod.Post, "/api/card-actions/actionsallowed")
        {
            Content = new StringContent(@"{""userId"":""User1"",""cardNumber"":""Card11""}") // no content-type
        };
        var res = await client.SendAsync(req);
        // Model binding może nie zadziałać -> 415 lub 400 (w zależności od hosta)
        Assert.True(res.StatusCode is HttpStatusCode.UnsupportedMediaType or HttpStatusCode.BadRequest);
    }
}
