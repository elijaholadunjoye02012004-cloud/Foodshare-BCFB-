namespace FoodShare.Models;

public static class AllergenTags
{
    public static readonly string[] KnownAllergens =
    [
        "Gluten",
        "Nuts",
        "Dairy",
        "Eggs",
        "Soy",
        "Fish",
        "Shellfish",
        "Pork"
    ];

    public static string Format(IEnumerable<string> tags) =>
        tags.Any() ? string.Join(", ", tags) : "None";

    public static List<string> Parse(string? allergenInfo)
    {
        if (string.IsNullOrWhiteSpace(allergenInfo) || allergenInfo.Equals("None", StringComparison.OrdinalIgnoreCase))
            return [];

        return allergenInfo
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }
}
