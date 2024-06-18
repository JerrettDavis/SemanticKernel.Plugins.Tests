namespace SemanticKernel.Plugins.Tests.Common.Models;

public record ActionItemsResponse
{
    public IEnumerable<Item> ActionItems { get; init; } = new List<Item>();
}

public record Item
{
    public string Owner { get; init; } = null!;
    public string ActionItem { get; init; } = null!;
    public string DueDate { get; init; } = null!;
    public string Status { get; init; } = null!;
    public string Notes { get; init; } = null!;
} 