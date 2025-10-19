using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FinancialServices.Tests;

public class Endpoint_AllowedActions_EdgeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public Endpoint_AllowedActions_EdgeTests(WebApplicationFactory<Program> factory)
        => _factory = factory;

    [Fact(DisplayName = "[API] 400 + ProblemDetails for missing userId")]
    public async Task Missing_UserId_Returns_400_With_ProblemDetails()
    {
        var client = _factory.CreateClient();

        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed",
                    new { userId = "", cardNumber = "Card11" });
        var body = await res.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal("Validation error", problem!.Title);
        Assert.Equal("Both userId and cardNumber are required.", problem.Detail);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);

        Assert.Contains("problem+json",
                 res.Content.Headers.ContentType!.ToString(),
                 StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "[API] 400 + ProblemDetails for missing cardNumber")]
    public async Task Missing_CardNumber_Returns_400_With_ProblemDetails()
    {
        var client = _factory.CreateClient();

        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed",
                    new { userId = "User1", cardNumber = "" });
        var body = await res.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal("Validation error", problem!.Title);
        Assert.Equal("Both userId and cardNumber are required.", problem.Detail);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
    }

    [Fact(DisplayName = "[API] 404 + ProblemDetails when user does not exist")]
    public async Task Unknown_User_Returns_404_With_ProblemDetails()
    {
        var client = _factory.CreateClient();

        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed",
                    new { userId = "NoSuchUser", cardNumber = "Card11" });
        var body = await res.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal("User not found", problem!.Title);
        Assert.Equal("User 'NoSuchUser' does not exist.", problem.Detail);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }

    [Fact(DisplayName = "[API] 404 + ProblemDetails when user exists but card does not")]
    public async Task Unknown_Card_For_User_Returns_404_With_ProblemDetails()
    {
        var client = _factory.CreateClient();

        var res = await client.PostAsJsonAsync("/api/card-actions/actionsallowed",
                    new { userId = "User1", cardNumber = "Nope" });
        var body = await res.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);

        var problem = await res.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);

        Assert.Equal("Card not found", problem!.Title);
        Assert.Equal("User 'User1' does not own card 'Nope'.", problem.Detail);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
    }
}
