using System.Text.Json;
using Microsoft.Extensions.Logging;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.Infrastructure.ExternalServices;

/// <summary>
/// Implementation of IAIService using Azure OpenAI.
/// Wraps Azure OpenAI SDK to provide a clean abstraction.
/// Configuration: Use appsettings.json with keys like "AzureOpenAI:Endpoint", "AzureOpenAI:Key", "AzureOpenAI:DeploymentName"
/// </summary>
public class AzureOpenAiService : IAIService
{
    private readonly ILogger<AzureOpenAiService> _logger;
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly string _deploymentName;
    private readonly string _apiVersion;

    public AzureOpenAiService(
        ILogger<AzureOpenAiService> logger,
        string endpoint,
        string apiKey,
        string deploymentName,
        string apiVersion = "2024-02-15-preview")
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _deploymentName = deploymentName ?? throw new ArgumentNullException(nameof(deploymentName));
        _apiVersion = apiVersion;
    }

    /// <summary>
    /// Calls Azure OpenAI's Chat Completion API and returns plain text response.
    /// TODO: Implement actual HTTP call to Azure OpenAI.
    /// Example using Azure.AI.OpenAI SDK (install NuGet package: Azure.AI.OpenAI):
    /// 
    /// var client = new OpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
    /// var chatCompletionOptions = new ChatCompletionsOptions
    /// {
    ///     Messages = new[]
    ///     {
    ///         new ChatMessage(ChatRole.System, systemPrompt),
    ///         new ChatMessage(ChatRole.User, userPrompt),
    ///     },
    ///     Temperature = 0.7f,
    ///     MaxTokens = 4096,
    ///     DeploymentName = _deploymentName,
    /// };
    /// var response = await client.GetChatCompletionsAsync(chatCompletionOptions, cancellationToken);
    /// return response.Value.Choices[0].Message.Content;
    /// </summary>
    public async Task<string> CallChatModelAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling Azure OpenAI with deployment '{DeploymentName}'", _deploymentName);

            // TODO: Implement actual Azure OpenAI call
            // For now, return a placeholder response
            await Task.Delay(100, cancellationToken);

            var mockResponse = $"Mock response from Azure OpenAI. System: {systemPrompt.Substring(0, 50)}...";
            _logger.LogInformation("Received response from Azure OpenAI");
            return mockResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure OpenAI");
            throw;
        }
    }

    /// <summary>
    /// Calls Azure OpenAI and deserializes response as JSON.
    /// Useful for structured responses.
    /// </summary>
    public async Task<T> CallChatModelAsync<T>(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default)
    {
        var response = await CallChatModelAsync(systemPrompt, userPrompt, cancellationToken);
        
        try
        {
            var result = JsonSerializer.Deserialize<T>(response);
            return result ?? throw new InvalidOperationException("Failed to deserialize response");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize Azure OpenAI response as JSON");
            throw;
        }
    }
}
