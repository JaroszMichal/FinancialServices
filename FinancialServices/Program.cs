using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinancialServices.API.Domain;

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

// Redirect to HTTPS only outside of Docker
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

// ===== Feature mapping (no business logic here) =====
var actions = app.MapGroup("/api/card-actions").WithTags("CardActions");
FinancialServices.API.Features.CardActions.GetAllowedActions.Endpoint.Map(actions);

app.Run();
