namespace ResumeBuilder.Application.Interfaces;

/// <summary>
/// Interface for AI service abstraction.
/// Implementations can swap between Azure OpenAI, OpenAI API, or other models.
/// Ensures no controller/application logic is tightly coupled to a specific AI provider.
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Calls the chat completion model with a system prompt and user prompt.
    /// </summary>
    Task<string> CallChatModelAsync(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calls the model and expects structured JSON response that can be deserialized.
    /// </summary>
    Task<T> CallChatModelAsync<T>(string systemPrompt, string userPrompt, CancellationToken cancellationToken = default);
}
