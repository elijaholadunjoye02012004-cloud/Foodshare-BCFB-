using FoodShare.Models;

namespace FoodShare.Services;

public class DietarySafetyService
{
    public List<string> GetConflicts(string? dietaryRequirements, FoodItem foodItem)
    {
        var conflicts = new List<string>();
        if (string.IsNullOrWhiteSpace(dietaryRequirements))
            return conflicts;

        var diet = dietaryRequirements;
        var allergens = AllergenTags.Parse(foodItem.AllergenInfo);
        var allergenText = foodItem.AllergenInfo ?? "";

        if (ContainsKeyword(diet, "nut") && (allergens.Contains("Nuts") || foodItem.HasAllergen("nut")))
            conflicts.Add($"{foodItem.Name} contains nuts.");

        if (ContainsKeyword(diet, "gluten") && (allergens.Contains("Gluten") || foodItem.HasAllergen("gluten")))
            conflicts.Add($"{foodItem.Name} contains gluten.");

        if (ContainsKeyword(diet, "dairy") && (allergens.Contains("Dairy") || foodItem.HasAllergen("dairy")))
            conflicts.Add($"{foodItem.Name} contains dairy.");

        if (ContainsKeyword(diet, "egg") && (allergens.Contains("Eggs") || foodItem.HasAllergen("egg")))
            conflicts.Add($"{foodItem.Name} contains eggs.");

        if (ContainsKeyword(diet, "halal") &&
            (allergens.Contains("Pork") || foodItem.HasAllergen("pork") || foodItem.Name.Contains("ham", StringComparison.OrdinalIgnoreCase)))
            conflicts.Add($"{foodItem.Name} may not be Halal — verify before including.");

        if (ContainsKeyword(diet, "vegetarian") &&
            (allergens.Contains("Fish") || allergens.Contains("Shellfish") || allergens.Contains("Pork") ||
             foodItem.Name.Contains("ham", StringComparison.OrdinalIgnoreCase) ||
             foodItem.Name.Contains("meat", StringComparison.OrdinalIgnoreCase)))
            conflicts.Add($"{foodItem.Name} may not be suitable for vegetarians.");

        if (allergenText.Contains("Contains", StringComparison.OrdinalIgnoreCase) &&
            diet.Contains("allergen", StringComparison.OrdinalIgnoreCase))
            conflicts.Add($"{foodItem.Name} has allergen information — review carefully.");

        return conflicts;
    }

    public bool HasConflict(string? dietaryRequirements, FoodItem foodItem) =>
        GetConflicts(dietaryRequirements, foodItem).Count > 0;

    private static bool ContainsKeyword(string text, string keyword) =>
        text.Contains(keyword, StringComparison.OrdinalIgnoreCase);
}
