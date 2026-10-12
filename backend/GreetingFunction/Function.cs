using System;
using System.Collections.Generic;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GreetingFunction;

public class Function
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    /// <summary>
    /// Lambda handler designed for AWS API Gateway Proxy integration
    /// </summary>
    public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing GreetingFunction invocation via APIGatewayProxyRequest");

        // Handle CORS preflight
        if (string.Equals(request.HttpMethod, "OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCorsResponse(200, string.Empty);
        }

        string name = "Guest";
        string language = "en";

        // Try extracting parameters from body
        if (!string.IsNullOrWhiteSpace(request.Body))
        {
            try
            {
                var bodyModel = JsonSerializer.Deserialize<GreetingRequest>(request.Body, JsonOptions);
                if (!string.IsNullOrWhiteSpace(bodyModel?.Name))
                {
                    name = bodyModel.Name.Trim();
                }
                if (!string.IsNullOrWhiteSpace(bodyModel?.Language))
                {
                    language = bodyModel.Language.Trim().ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogWarning($"Failed to deserialize JSON body: {ex.Message}. Falling back to query string params.");
            }
        }

        // Fallback to query parameters if present
        if (request.QueryStringParameters != null)
        {
            if (request.QueryStringParameters.TryGetValue("name", out var queryName) && !string.IsNullOrWhiteSpace(queryName))
            {
                name = queryName.Trim();
            }
            if (request.QueryStringParameters.TryGetValue("language", out var queryLang) && !string.IsNullOrWhiteSpace(queryLang))
            {
                language = queryLang.Trim().ToLowerInvariant();
            }
        }

        var greetingResult = GenerateGreeting(name, language);
        var jsonResponse = JsonSerializer.Serialize(greetingResult, JsonOptions);

        return CreateCorsResponse(200, jsonResponse);
    }

    /// <summary>
    /// Direct invocation handler (when invoked directly via AWS SDK or test tools without API Gateway)
    /// </summary>
    public GreetingResponse DirectHandler(GreetingRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Direct invocation for: {request.Name}");
        return GenerateGreeting(request.Name ?? "Guest", request.Language ?? "en");
    }

    public static GreetingResponse GenerateGreeting(string name, string language)
    {
        string salutation = language switch
        {
            "es" => $"¡Hola, {name}!",
            "fr" => $"Bonjour, {name}!",
            "de" => $"Hallo, {name}!",
            "ja" => $"こんにちは、{name}さん！",
            _ => $"Hello, {name}!"
        };

        return new GreetingResponse
        {
            Greeting = $"{salutation} Greetings from AWS Lambda (.NET 8)!",
            Name = name,
            Language = language,
            Timestamp = DateTime.UtcNow,
            Environment = "AWS Lambda (.NET 8 on Amazon Linux 2023)"
        };
    }

    private static APIGatewayProxyResponse CreateCorsResponse(int statusCode, string body)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = statusCode,
            Body = body,
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token" },
                { "Access-Control-Allow-Methods", "GET,POST,OPTIONS" }
            }
        };
    }
}
