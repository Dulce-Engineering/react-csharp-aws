using System;
using System.Text.Json.Serialization;

namespace GreetingFunction;

public class GreetingRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; } = "en";
}

public class GreetingResponse
{
    [JsonPropertyName("greeting")]
    public string Greeting { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = "en";

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("environment")]
    public string Environment { get; set; } = "AWS Lambda (.NET 8)";
}
