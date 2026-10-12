using System;
using System.Text.Json.Serialization;

namespace CalculatorFunction;

public class CalculatorRequest
{
    [JsonPropertyName("a")]
    public double A { get; set; }

    [JsonPropertyName("b")]
    public double B { get; set; }

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = "add";
}

public class CalculatorResponse
{
    [JsonPropertyName("a")]
    public double A { get; set; }

    [JsonPropertyName("b")]
    public double B { get; set; }

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = "add";

    [JsonPropertyName("result")]
    public double Result { get; set; }

    [JsonPropertyName("expression")]
    public string Expression { get; set; } = string.Empty;

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = "AWS Lambda (.NET 8)";
}
