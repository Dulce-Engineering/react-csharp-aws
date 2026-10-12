using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CalculatorFunction;

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
        context.Logger.LogInformation("Processing CalculatorFunction invocation via APIGatewayProxyRequest");

        // Handle CORS preflight
        if (string.Equals(request.HttpMethod, "OPTIONS", StringComparison.OrdinalIgnoreCase))
        {
            return CreateCorsResponse(200, string.Empty);
        }

        double a = 0;
        double b = 0;
        string operation = "add";

        // Try extracting parameters from body
        if (!string.IsNullOrWhiteSpace(request.Body))
        {
            try
            {
                var bodyModel = JsonSerializer.Deserialize<CalculatorRequest>(request.Body, JsonOptions);
                if (bodyModel != null)
                {
                    a = bodyModel.A;
                    b = bodyModel.B;
                    if (!string.IsNullOrWhiteSpace(bodyModel.Operation))
                    {
                        operation = bodyModel.Operation.Trim().ToLowerInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogWarning($"Failed to deserialize JSON body: {ex.Message}. Falling back to query parameters.");
            }
        }

        // Fallback or override from query string parameters if present
        if (request.QueryStringParameters != null)
        {
            if (request.QueryStringParameters.TryGetValue("a", out var qA) && double.TryParse(qA, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedA))
            {
                a = parsedA;
            }
            if (request.QueryStringParameters.TryGetValue("b", out var qB) && double.TryParse(qB, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedB))
            {
                b = parsedB;
            }
            if (request.QueryStringParameters.TryGetValue("operation", out var qOp) && !string.IsNullOrWhiteSpace(qOp))
            {
                operation = qOp.Trim().ToLowerInvariant();
            }
        }

        var calcResponse = Calculate(a, b, operation);
        int statusCode = calcResponse.Error != null ? 400 : 200;
        var jsonResponse = JsonSerializer.Serialize(calcResponse, JsonOptions);

        return CreateCorsResponse(statusCode, jsonResponse);
    }

    /// <summary>
    /// Direct invocation handler
    /// </summary>
    public CalculatorResponse DirectHandler(CalculatorRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Direct invocation for operation: {request.Operation}");
        return Calculate(request.A, request.B, request.Operation ?? "add");
    }

    public static CalculatorResponse Calculate(double a, double b, string operation)
    {
        double result = 0;
        string expression = string.Empty;
        string? error = null;

        switch (operation.ToLowerInvariant())
        {
            case "add":
            case "+":
                operation = "add";
                result = a + b;
                expression = $"{a} + {b} = {result}";
                break;

            case "subtract":
            case "-":
                operation = "subtract";
                result = a - b;
                expression = $"{a} - {b} = {result}";
                break;

            case "multiply":
            case "*":
                operation = "multiply";
                result = a * b;
                expression = $"{a} × {b} = {result}";
                break;

            case "divide":
            case "/":
                operation = "divide";
                if (Math.Abs(b) < double.Epsilon)
                {
                    error = "Division by zero is not allowed.";
                    expression = $"{a} ÷ 0 = Undefined";
                }
                else
                {
                    result = a / b;
                    expression = $"{a} ÷ {b} = {Math.Round(result, 4)}";
                }
                break;

            case "power":
            case "^":
                operation = "power";
                result = Math.Pow(a, b);
                expression = $"{a} ^ {b} = {result}";
                break;

            default:
                error = $"Unsupported operation: '{operation}'. Supported operations: add, subtract, multiply, divide, power.";
                break;
        }

        return new CalculatorResponse
        {
            A = a,
            B = b,
            Operation = operation,
            Result = result,
            Expression = expression,
            Error = error,
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
