using FinancialServices.API.Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mocked data from the PDF: register CardService in DI as a singleton
builder.Services.AddSingleton<CardService>();

var app = builder.Build();

// Global exception handler (ProblemDetails)
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async ctx =>
    {
        var feature = ctx.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        var traceId = Activity.Current?.Id ?? ctx.TraceIdentifier;
        var status = ex switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Title = status == 500 ? "Unexpected error" : ex?.GetType().Name,
            Status = status,
            Type = $"https://httpstatuses.com/{status}",
            // In production do not expose ex.Message;
            Detail = ctx.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                     ? ex?.Message
                     : $"An error occurred. TraceId: {traceId}"
        };

        // Optional RFC7807 extensions (custom fields)
        problem.Extensions["traceId"] = traceId;

        // Logging (short form), in real requires configuration (e.g. add email conf)
        var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>()
                                        .CreateLogger("GlobalExceptionHandler");
        logger.LogError(ex, "Unhandled exception, traceId={TraceId}", traceId);

        ctx.Response.ContentType = MediaTypeNames.Application.Json;
        ctx.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        await ctx.Response.WriteAsJsonAsync(problem);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect to HTTPS only outside of Docker
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

// ===== Feature mapping (no business logic here) =====
var actions = app.MapGroup("/api/card-actions").WithTags("CardActions");
FinancialServices.API.Features.CardActions.GetAllowedActions.Endpoint.Map(actions);

app.Run();
