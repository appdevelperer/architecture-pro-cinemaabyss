// src/microservices/proxy/Program.cs
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.WriteTo.Console());
builder.WebHost.UseUrls("http://*:8400");

var app = builder.Build();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error;
        await context.Response.WriteAsync($"Proxy error: {error?.Message}\n{error?.StackTrace}");
    });
});


var httpClient = new HttpClient();

var migrationPercentStr = Environment.GetEnvironmentVariable("MOVIES_MIGRATION_PERCENT") ?? "0";
if (!int.TryParse(migrationPercentStr, out var migrationPercent) || migrationPercent < 0 || migrationPercent > 100)
{
    throw new InvalidOperationException("MOVIES_MIGRATION_PERCENT must be an integer between 0 and 100");
}

var random = new Random();

// Health check — остаётся
app.MapGet("/health", () => Results.Json(new { status = "ok", service = "proxy" }));

// Middleware ДО fallback
app.Use(async (context, next) =>
{
    // ✅ Правильная проверка пути
    if (context.Request.Path.StartsWithSegments("/api/movies"))
    {
        var useMoviesService = random.Next(100) < migrationPercent;
        var targetBase = useMoviesService 
            ? "http://movies-service:8081"  
            : "http://monolith:8080";

        var targetUrl = $"{targetBase}{context.Request.Path}{context.Request.QueryString}";

        Log.Information("Routing {Path} to {Target} (Movies: {UseMovies}%)",
            context.Request.Path, useMoviesService ? "Movies Service" : "Monolith", migrationPercent);

        var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetUrl);

        foreach (var header in context.Request.Headers)
        {
            if (header.Key != "Host")
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        if (context.Request.ContentLength > 0 || !HttpMethods.IsGet(context.Request.Method))
        {
            requestMessage.Content = new StreamContent(context.Request.Body);
        }

        var response = await httpClient.SendAsync(requestMessage, context.RequestAborted);

        context.Response.StatusCode = (int)response.StatusCode;
        foreach (var header in response.Headers.Concat(response.Content.Headers))
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        await response.Content.CopyToAsync(context.Response.Body);
        return;
    }

    await next();
});


app.Run(async context =>
{
    throw new Exception("Fallback triggered — this means /api/movies did NOT match!");
});


app.Run();