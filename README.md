# SemanticKernel Plugin Tests

## Overview

This project contains various tests for Semantic Kernel Plugin. It supports both OpenAI and Ollama models for generating action items from conversational prompts.

## Prerequisites

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- An [OpenAI API key](https://beta.openai.com/signup/)
- An [Ollama URL](https://ollama.com)

## Configuration

To keep your API keys secure, use the `dotnet user-secrets` tool to manage sensitive settings. User secrets store sensitive information in a secure manner, outside of your source code.

### Setting Up User Secrets

1. Open your project in the terminal.
2. Run the following command to initialize user secrets:
    ```sh
    dotnet user-secrets init
    ```
3. Add your secrets:
    ```sh
    dotnet user-secrets set "OpenAI:ApiKey" "your_openai_api_key"
    dotnet user-secrets set "Ollama:Url" "your_ollama_url"
    ```

### Project Structure

- **Models**
    - `Settings.cs`: Contains configuration models for OpenAI and Ollama settings.
    - `ActionItemsResponse.cs`: Defines the structure for the action items response.
    - `PromptRepresentation.cs`: Represents the structure of a test prompt.

- **Tests**
    - `ConversationSummaryPluginTestsBase.cs`: The base class for testing conversation summary plugins.
    - `ConversationSummaryPluginOpenAITests.cs`: Tests for the Conversation Summary Plugin using OpenAI models.
    - `ConversationSummaryPluginOllamaTests.cs`: Tests for the Conversation Summary Plugin using Ollama models.

### Configuration

The configuration is loaded from user secrets in the constructor of the `ConversationSummaryPluginTestsBase` class:

```csharp
protected ConversationSummaryPluginTestsBase()
{
    var config = new ConfigurationBuilder()
        .AddUserSecrets<Settings>()
        .Build();
    Settings = config.Get<Settings>()!;
}
```

### Running the Tests

Ensure you have your user secrets set up as described above, then run the tests using your preferred method (e.g., Visual Studio Test Explorer, `dotnet test` CLI).

## Sample Test Case

Here's an example of a test case using the OpenAI model:

```csharp
[Parallelizable]
public class ConversationSummaryPluginOpenAITests : ConversationSummaryPluginTestsBase
{
    protected override Action<IKernelBuilder, string> ConfigureModel =>
        (builder, modelName) =>
            builder.AddOpenAIChatCompletion(
                modelName,
                Settings.OpenAI.ApiKey);

    [Parallelizable]
    [TestCaseSource(nameof(GetModelNames))]
    public override Task ShouldReturnActionItems(PromptRepresentation prompt) =>
        base.ShouldReturnActionItems(prompt);

    protected static IEnumerable<PromptRepresentation> GetModelNames()
    {
        foreach (var prompt in GetPrompts())
        {
            yield return new PromptRepresentation("gpt-3.5-turbo", prompt.prompt, prompt.actionItems);
            yield return new PromptRepresentation("gpt-4-turbo", prompt.prompt, prompt.actionItems);
        }
    }
}
```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Contributing

Contributions are welcome! Please read the [CONTRIBUTING](CONTRIBUTING.md) guidelines for more information.