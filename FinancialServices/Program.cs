using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Us³ugi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Globalny handler wyj¹tków (ProblemDetails)
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        var problem = new ProblemDetails
        {
            Title = "Unexpected error",
            Detail = ex?.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://httpstatuses.com/500"
        };
        ctx.Response.ContentType = MediaTypeNames.Application.Json;
        ctx.Response.StatusCode = problem.Status ?? 500;
        await ctx.Response.WriteAsJsonAsync(problem);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect do HTTPS tylko poza Dockerem
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

// ===== Endpointy =====

// Health
app.MapGet("/health", () => Results.Ok(new { status = "OK" }))
   .WithName("Health")
   .WithTags("System");

// POST /api/actions
app.MapPost("/api/actions", (ActionRequest request) =>
{
    var (ok, problem) = Validators.Validate(request);
    if (!ok) return Results.Problem(problem);

    var created = new ActionResponse(
        Id: Guid.NewGuid(),
        Type: request.Type.Trim(),
        Amount: request.Amount,
        Currency: request.Currency.ToUpperInvariant(),
        CreatedAt: DateTimeOffset.UtcNow
    );

    return Results.Created($"/api/actions/{created.Id}", created);
})
.WithName("CreateAction")
.Produces<ActionResponse>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.ProducesProblem(StatusCodes.Status500InternalServerError)
.WithTags("Actions");

app.Run();


// ===== Deklaracje typów (po app.Run!) =====
public record ActionRequest(string Type, decimal Amount, string Currency);
public record ActionResponse(Guid Id, string Type, decimal Amount, string Currency, DateTimeOffset CreatedAt);

static class Validators
{
    public static (bool ok, ProblemDetails? problem) Validate(ActionRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Type))
            return (false, new ProblemDetails { Title = "Validation error", Detail = "Type is required.", Status = StatusCodes.Status400BadRequest });

        if (r.Amount <= 0)
            return (false, new ProblemDetails { Title = "Validation error", Detail = "Amount must be > 0.", Status = StatusCodes.Status400BadRequest });

        if (string.IsNullOrWhiteSpace(r.Currency) || r.Currency.Length is < 3 or > 3)
            return (false, new ProblemDetails { Title = "Validation error", Detail = "Currency must be a 3-letter code (e.g., PLN).", Status = StatusCodes.Status400BadRequest });

        return (true, null);
    }
}
