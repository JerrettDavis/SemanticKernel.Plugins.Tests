namespace SemanticKernel.Plugins.Tests.Common.Models;

public class Settings
{
    public OpenAISettings OpenAI { get; set; } = null!;
    public OllamaSettings Ollama { get; set; } = null!;
}

public class OpenAISettings
{
    public string ApiKey { get; set; } = null!;
}

public class OllamaSettings
{
    public string Url { get; set; } = null!;
}