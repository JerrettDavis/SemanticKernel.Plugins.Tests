using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Plugins.Tests.ConversationSummaryPluginTests;

public class ConversationSummaryPluginOllamaTests :
    ConversationSummaryPluginTestsBase
{
    protected override Action<IKernelBuilder, string> ConfigureModel =>
        (builder, modelName) =>
            builder.AddOllamaChatCompletion(
                modelName,
                Settings.Ollama.Url);

    [Parallelizable]
    [TestCaseSource(nameof(GetModelNames))]
    public override Task ShouldReturnActionItems(
        PromptRepresentation prompt) =>
        base.ShouldReturnActionItems(prompt);

    protected static IEnumerable<PromptRepresentation> GetModelNames()
    {
        foreach (var prompt in GetPrompts())
        {
            yield return new PromptRepresentation("llama2:7b", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("llama3:8b", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("mistral:7b", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("qwen2:7b", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("phi3:3.8b", prompt.prompt, prompt.actionItems);    
        }
    }
}