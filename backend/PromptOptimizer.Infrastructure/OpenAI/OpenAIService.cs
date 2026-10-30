using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PromptOptimizer.Application.DTOs;
using PromptOptimizer.Application.Interfaces;

namespace PromptOptimizer.Infrastructure.OpenAI;

/// <summary>
/// Calls OpenAI / Azure OpenAI Chat Completions to optimize prompts. When no API
/// key is configured the service falls back to a deterministic local
/// optimization so the application remains fully functional for demos/tests.
/// </summary>
public class OpenAIService : IOpenAIService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(
        HttpClient httpClient,
        IOptions<OpenAIOptions> options,
        ILogger<OpenAIService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<PromptOptimizationResult> GetOptimizedPromptAsync(
        string systemPrompt,
        string userPrompt,
        CancellationToken cancellationToken = default)
    {
        if (!_options.IsConfigured)
        {
            _logger.LogWarning("OpenAI API key not configured; using offline fallback optimizer.");
            return OfflineOptimizer.Optimize(userPrompt);
        }

        var (requestUri, payload) = BuildRequest(systemPrompt, userPrompt);

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = JsonContent.Create(payload)
        };

        if (_options.IsAzure)
        {
            httpRequest.Headers.Add("api-key", _options.ApiKey);
        }
        else
        {
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }

        using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("LLM request failed ({StatusCode}): {Error}", response.StatusCode, error);
            throw new HttpRequestException(
                $"The AI provider returned {(int)response.StatusCode} ({response.StatusCode}).");
        }

        var completion = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(
            JsonOptions, cancellationToken);

        var content = completion?.Choices?.FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            _logger.LogError("LLM returned an empty completion.");
            throw new InvalidOperationException("The AI provider returned an empty response.");
        }

        return ParseModelContent(content);
    }

    private (string RequestUri, object Payload) BuildRequest(string systemPrompt, string userPrompt)
    {
        var payload = new
        {
            model = _options.Model,
            temperature = _options.Temperature,
            max_tokens = _options.MaxTokens,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            }
        };

        if (_options.IsAzure)
        {
            var endpoint = _options.Endpoint?.TrimEnd('/')
                ?? throw new InvalidOperationException("Azure OpenAI endpoint is not configured.");
            var uri = $"{endpoint}/openai/deployments/{_options.Model}/chat/completions?api-version={_options.ApiVersion}";
            return (uri, payload);
        }

        return ("https://api.openai.com/v1/chat/completions", payload);
    }

    private PromptOptimizationResult ParseModelContent(string content)
    {
        var json = ExtractJson(content);

        try
        {
            var parsed = JsonSerializer.Deserialize<ModelResult>(json, JsonOptions);
            if (parsed is not null && !string.IsNullOrWhiteSpace(parsed.OptimizedPrompt))
            {
                return new PromptOptimizationResult
                {
                    OptimizedPrompt = parsed.OptimizedPrompt.Trim(),
                    Improvements = parsed.Improvements?.Where(i => !string.IsNullOrWhiteSpace(i)).ToList()
                                   ?? new List<string>()
                };
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse structured JSON from LLM; returning raw content.");
        }

        // The model did not honor the JSON contract; fall back to raw text.
        return new PromptOptimizationResult
        {
            OptimizedPrompt = content.Trim(),
            Improvements = new List<string> { "Optimized by the AI model." }
        };
    }

    /// <summary>Strips markdown code fences if the model wrapped the JSON.</summary>
    private static string ExtractJson(string content)
    {
        var trimmed = content.Trim();
        if (trimmed.StartsWith("```"))
        {
            var firstNewline = trimmed.IndexOf('\n');
            if (firstNewline >= 0)
            {
                trimmed = trimmed[(firstNewline + 1)..];
            }

            var lastFence = trimmed.LastIndexOf("```", StringComparison.Ordinal);
            if (lastFence >= 0)
            {
                trimmed = trimmed[..lastFence];
            }
        }

        return trimmed.Trim();
    }

    private sealed class ModelResult
    {
        [JsonPropertyName("optimizedPrompt")]
        public string OptimizedPrompt { get; set; } = string.Empty;

        [JsonPropertyName("improvements")]
        public List<string>? Improvements { get; set; }
    }

    private sealed class ChatCompletionResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    private sealed class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }

    private sealed class Message
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
