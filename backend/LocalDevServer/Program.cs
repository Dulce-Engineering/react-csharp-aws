using System;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS for local React dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// Diagnostic info
app.MapGet("/", () => Results.Ok(new
{
    service = "AWS Lambda Local Development Server (.NET 8)",
    status = "healthy",
    endpoints = new[]
    {
        "POST /api/greeting - Dispatches to GreetingFunction Lambda",
        "GET /api/greeting?name=User&language=en - Query greeting",
        "POST /api/calculator - Dispatches to CalculatorFunction Lambda",
        "GET /api/calculator?a=10&b=5&operation=add - Query calculator"
    }
}));

// Route /api/greeting to GreetingFunction
app.MapMethods("/api/greeting", new[] { "GET", "POST" }, async (HttpContext http) =>
{
    string body = string.Empty;
    if (http.Request.ContentLength > 0 || http.Request.ContentType?.Contains("json") == true)
    {
        using var reader = new System.IO.StreamReader(http.Request.Body);
        body = await reader.ReadToEndAsync();
    }

    var queryParams = new Dictionary<string, string>();
    foreach (var q in http.Request.Query)
    {
        queryParams[q.Key] = q.Value.ToString();
    }

    var lambdaRequest = new APIGatewayProxyRequest
    {
        HttpMethod = http.Request.Method,
        Path = http.Request.Path,
        Body = body,
        QueryStringParameters = queryParams
    };

    var function = new GreetingFunction.Function();
    var response = function.FunctionHandler(lambdaRequest, new LocalLambdaContext("GreetingFunction"));

    return Results.Content(response.Body, "application/json", statusCode: response.StatusCode);
});

// Route /api/calculator to CalculatorFunction
app.MapMethods("/api/calculator", new[] { "GET", "POST" }, async (HttpContext http) =>
{
    string body = string.Empty;
    if (http.Request.ContentLength > 0 || http.Request.ContentType?.Contains("json") == true)
    {
        using var reader = new System.IO.StreamReader(http.Request.Body);
        body = await reader.ReadToEndAsync();
    }

    var queryParams = new Dictionary<string, string>();
    foreach (var q in http.Request.Query)
    {
        queryParams[q.Key] = q.Value.ToString();
    }

    var lambdaRequest = new APIGatewayProxyRequest
    {
        HttpMethod = http.Request.Method,
        Path = http.Request.Path,
        Body = body,
        QueryStringParameters = queryParams
    };

    var function = new CalculatorFunction.Function();
    var response = function.FunctionHandler(lambdaRequest, new LocalLambdaContext("CalculatorFunction"));

    return Results.Content(response.Body, "application/json", statusCode: response.StatusCode);
});

app.Run();

// Mock Lambda Context for local testing
public class LocalLambdaContext : ILambdaContext
{
    public LocalLambdaContext(string functionName)
    {
        FunctionName = functionName;
    }

    public string AwsRequestId { get; } = Guid.NewGuid().ToString();
    public IClientContext? ClientContext => null;
    public string FunctionName { get; }
    public string FunctionVersion { get; } = "$LATEST";
    public ICognitoIdentity? Identity => null;
    public string InvokedFunctionArn { get; } = "arn:aws:lambda:local:123456789012:function:local";
    public string LogGroupName { get; } = "/aws/lambda/local";
    public string LogStreamName { get; } = "2026/10/12/[$LATEST]local";
    public ILambdaLogger Logger { get; } = new LocalLambdaLogger();
    public int MemoryLimitInMB { get; } = 256;
    public TimeSpan RemainingTime => TimeSpan.FromMinutes(5);
}

public class LocalLambdaLogger : ILambdaLogger
{
    public void Log(string message) => Console.WriteLine($"[Lambda LOG] {message}");
    public void LogLine(string message) => Console.WriteLine($"[Lambda LOG] {message}");
}
