namespace TrainingAssistant.Domain.Entities;

/// <summary>
/// Продукт из каталога питания
/// </summary>
public class FoodItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public int DefaultPortionGrams { get; set; }
}
