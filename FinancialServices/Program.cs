using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinancialServices.API.Domain;

var builder = WebApplication.CreateBuilder(args);

// Us³ugi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mock danych z PDF: w DI jako Singleton
builder.Services.AddSingleton<CardService>();

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

// ===== Mapowanie feature'u (bez logiki tutaj) =====
var actions = app.MapGroup("/api/card-actions").WithTags("CardActions");
FinancialServices.API.Features.CardActions.GetAllowedActions.Endpoint.Map(actions);

app.Run();