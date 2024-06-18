using Microsoft.SemanticKernel;

namespace SemanticKernel.Plugins.Tests.ConversationSummaryPluginTests;

[Parallelizable]
public class ConversationSummaryPluginOpenAITests :
    ConversationSummaryPluginTestsBase
{
    protected override Action<IKernelBuilder, string> ConfigureModel =>
        (builder, modelName) =>
            builder.AddOpenAIChatCompletion(
                modelName,
                Settings.OpenAI.ApiKey);

    [Parallelizable]
    [TestCaseSource(nameof(GetModelNames))]
    public override Task ShouldReturnActionItems(
        PromptRepresentation prompt) =>
        base.ShouldReturnActionItems(prompt);

    protected static IEnumerable<PromptRepresentation> GetModelNames()
    {
        foreach (var prompt in GetPrompts())
        {
            yield return new PromptRepresentation("gpt-3.5-turbo", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("gpt-4-turbo", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("gpt-4-turbo-preview", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("gpt-4", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("gpt-4o", prompt.prompt, prompt.actionItems);
        }
    }
}