using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using SemanticKernel.Plugins.Tests.Common.Models;

namespace SemanticKernel.Plugins.Tests.ConversationSummaryPluginTests;

[Parallelizable]
public abstract class ConversationSummaryPluginTestsBase
{
    protected abstract Action<IKernelBuilder, string> ConfigureModel { get; }
    protected Settings Settings { get; }

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected ConversationSummaryPluginTestsBase()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<Settings>()
            .Build();
        Settings = config.Get<Settings>()!;
    }

    [Parallelizable]
    public virtual async Task ShouldReturnActionItems(
        PromptRepresentation prompt)
    {
        var builder = Kernel.CreateBuilder();
        builder.Services.AddHttpClient();
        ConfigureModel(builder, prompt.Model);
        builder.Plugins.AddFromType<ConversationSummaryPlugin>();

        var kernel = builder.Build();
        var result = await kernel.InvokeAsync(
            "ConversationSummaryPlugin",
            "GetConversationActionItems",
            new KernelArguments { { "input", prompt.Prompt } });
        var value = result.GetValue<string>()!;
        
        TestContext.WriteLine($"Kernel invocation result: {value}");
        
        var response =  DeserializeResponse(value);
        
        TestContext.WriteLine($"Deserialized response: {JsonSerializer.Serialize(response)}");
        
        var actionItemCountLow = prompt.ActionItems - 1;
        var actionItemCountHigh = prompt.ActionItems + 1;

        response.Should().NotBeNull();
        response.ActionItems.Should().NotBeNull();
        response.ActionItems.Should().HaveCountGreaterThanOrEqualTo(actionItemCountLow);
        response.ActionItems.Should().HaveCountLessThanOrEqualTo(actionItemCountHigh);
    }

    private ActionItemsResponse DeserializeResponse(string response)
    {
        // All is good
        if (response.Trim().StartsWith('{'))
            return JsonSerializer.Deserialize<ActionItemsResponse>(
                response.Replace("\"action_items\"", "\"actionItems\""),
                _options)!;

        // Something went wrong
        // See if there is a json code block (```json ```)
        var match = Regex.Match(response, @"```json\s*(?<json>.+?)\s*```", RegexOptions.Singleline);

        // if we find the match, extract the json and deserialize it
        if (match.Success)
            return JsonSerializer.Deserialize<ActionItemsResponse>(
                match.Groups["json"].Value.Replace("\"action_items\"", "\"actionItems\""),
                _options)!;
        
        // If we don't find the match, just return a new response
        return new ActionItemsResponse();
    }

    protected static IEnumerable<(string prompt, int actionItems)> GetPrompts()
    {
        yield return
        (
            """
            Hey there! Just wanted to remind you that we have to go to the dentist  
            tomorrow at 3 PM. Also, can you make sure to water the plants in the 
            living room? They’re looking a bit droopy. By the way, I noticed we’re 
            almost out of laundry detergent, so could you grab some when you’re out 
            today? And if you have time, maybe swing by the post office to pick up 
            those packages we’ve been waiting for. Thanks a bunch!
            """, 4);
        yield return
        (
            """
            Hi darling, hope your day is going smoothly. I’ve been thinking about 
            organizing a movie night this Friday. What do you think? Also, we need 
            to get the car serviced soon; it’s been making a weird noise. Don’t 
            forget to call the mechanic and set up an appointment. Oh, and could 
            you please pick up some cat food on your way back home? Fluffy’s running 
            low. Love you!
            """, 3);
        yield return
        (
            """
            Good afternoon! Just a quick note to say I miss you. By the way, can you 
            pick up our prescription refill from the pharmacy today? And we should 
            probably plan a grocery shopping trip this weekend; the fridge is 
            looking pretty empty. Also, I was thinking about repainting the living 
            room. What color do you think would look nice? Let’s chat about it 
            tonight. Take care!
            """, 3);
        yield return
        (
            """
            Hey! I was talking to Sarah about having a BBQ at our place next weekend. 
            Would you be up for it? Also, don’t forget we have to return those library 
            books by Thursday. And I was hoping you could take a look at the washing 
            machine; it’s been acting up again. By the way, I’ll be working late 
            tonight, so I won’t be home for dinner. See you later!
            """, 3);
        yield return
        (
            """
            Hi sweetie, just a heads up that we need to renew our car insurance 
            policy soon. Can you take care of that? Also, I was thinking we could 
            use a new coffee table for the living room. Maybe we can go shopping 
            for one this weekend? And don’t forget to walk the dog this evening; 
            she’s been cooped up all day. Hope you’re having a great day!
            """, 3);
    }
}

public record PromptRepresentation(string Model, string Prompt, int ActionItems);