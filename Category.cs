namespace FoodShare.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
