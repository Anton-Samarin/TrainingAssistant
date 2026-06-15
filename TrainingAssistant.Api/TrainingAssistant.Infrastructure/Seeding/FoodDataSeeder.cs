using TrainingAssistant.Domain.Entities;

namespace TrainingAssistant.Infrastructure.Seeding;

/// <summary>
/// Стартовый каталог продуктов для сида БД
/// </summary>
public static class FoodDataSeeder
{
    /// <summary>
    /// Начальный набор блюд для подбора питания
    /// </summary>
    /// <returns>Список продуктов с КБЖУ на 100 г</returns>
    public static IReadOnlyList<FoodItem> GetFoods() =>
    [
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Name = "Овсянка", CaloriesPer100g = 389, ProteinPer100g = 13, FatPer100g = 7, CarbsPer100g = 66, DefaultPortionGrams = 80 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Name = "Яйцо куриное", CaloriesPer100g = 157, ProteinPer100g = 13, FatPer100g = 11, CarbsPer100g = 1, DefaultPortionGrams = 110 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Name = "Творог 5%", CaloriesPer100g = 121, ProteinPer100g = 17, FatPer100g = 5, CarbsPer100g = 2, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Name = "Куриная грудка", CaloriesPer100g = 113, ProteinPer100g = 23, FatPer100g = 1, CarbsPer100g = 0, DefaultPortionGrams = 180 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111105"), Name = "Гречка", CaloriesPer100g = 343, ProteinPer100g = 13, FatPer100g = 3, CarbsPer100g = 72, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111106"), Name = "Рис", CaloriesPer100g = 344, ProteinPer100g = 7, FatPer100g = 1, CarbsPer100g = 78, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111107"), Name = "Лосось", CaloriesPer100g = 208, ProteinPer100g = 20, FatPer100g = 13, CarbsPer100g = 0, DefaultPortionGrams = 160 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111108"), Name = "Говядина", CaloriesPer100g = 250, ProteinPer100g = 26, FatPer100g = 15, CarbsPer100g = 0, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111109"), Name = "Брокколи", CaloriesPer100g = 34, ProteinPer100g = 3, FatPer100g = 0, CarbsPer100g = 7, DefaultPortionGrams = 200 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111110"), Name = "Огурец", CaloriesPer100g = 15, ProteinPer100g = 1, FatPer100g = 0, CarbsPer100g = 3, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Банан", CaloriesPer100g = 89, ProteinPer100g = 1, FatPer100g = 0, CarbsPer100g = 23, DefaultPortionGrams = 120 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Name = "Яблоко", CaloriesPer100g = 52, ProteinPer100g = 0, FatPer100g = 0, CarbsPer100g = 14, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Name = "Греческий йогурт", CaloriesPer100g = 59, ProteinPer100g = 10, FatPer100g = 0, CarbsPer100g = 4, DefaultPortionGrams = 180 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Name = "Миндаль", CaloriesPer100g = 579, ProteinPer100g = 21, FatPer100g = 50, CarbsPer100g = 22, DefaultPortionGrams = 30 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Name = "Оливковое масло", CaloriesPer100g = 884, ProteinPer100g = 0, FatPer100g = 100, CarbsPer100g = 0, DefaultPortionGrams = 10 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), Name = "Индейка грудка", CaloriesPer100g = 114, ProteinPer100g = 24, FatPer100g = 1, CarbsPer100g = 0, DefaultPortionGrams = 170 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), Name = "Треска", CaloriesPer100g = 78, ProteinPer100g = 18, FatPer100g = 1, CarbsPer100g = 0, DefaultPortionGrams = 180 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111118"), Name = "Тунец в собственном соку", CaloriesPer100g = 96, ProteinPer100g = 21, FatPer100g = 1, CarbsPer100g = 0, DefaultPortionGrams = 160 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111119"), Name = "Скумбрия", CaloriesPer100g = 191, ProteinPer100g = 18, FatPer100g = 13, CarbsPer100g = 0, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111120"), Name = "Сыр твёрдый", CaloriesPer100g = 350, ProteinPer100g = 25, FatPer100g = 27, CarbsPer100g = 2, DefaultPortionGrams = 40 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111121"), Name = "Кефир 1%", CaloriesPer100g = 40, ProteinPer100g = 3, FatPer100g = 1, CarbsPer100g = 5, DefaultPortionGrams = 250 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111122"), Name = "Молоко 2.5%", CaloriesPer100g = 52, ProteinPer100g = 3, FatPer100g = 2, CarbsPer100g = 5, DefaultPortionGrams = 200 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111123"), Name = "Творог 0%", CaloriesPer100g = 71, ProteinPer100g = 16, FatPer100g = 0, CarbsPer100g = 3, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111124"), Name = "Картофель", CaloriesPer100g = 77, ProteinPer100g = 2, FatPer100g = 0, CarbsPer100g = 17, DefaultPortionGrams = 200 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111125"), Name = "Макароны", CaloriesPer100g = 344, ProteinPer100g = 12, FatPer100g = 2, CarbsPer100g = 71, DefaultPortionGrams = 140 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111126"), Name = "Хлеб цельнозерновой", CaloriesPer100g = 247, ProteinPer100g = 9, FatPer100g = 3, CarbsPer100g = 48, DefaultPortionGrams = 70 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111127"), Name = "Булгур", CaloriesPer100g = 342, ProteinPer100g = 12, FatPer100g = 1, CarbsPer100g = 76, DefaultPortionGrams = 140 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111128"), Name = "Киноа", CaloriesPer100g = 368, ProteinPer100g = 14, FatPer100g = 6, CarbsPer100g = 64, DefaultPortionGrams = 130 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111129"), Name = "Помидоры", CaloriesPer100g = 18, ProteinPer100g = 1, FatPer100g = 0, CarbsPer100g = 4, DefaultPortionGrams = 180 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111130"), Name = "Перец болгарский", CaloriesPer100g = 27, ProteinPer100g = 1, FatPer100g = 0, CarbsPer100g = 6, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111131"), Name = "Шпинат", CaloriesPer100g = 23, ProteinPer100g = 3, FatPer100g = 0, CarbsPer100g = 4, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111132"), Name = "Морковь", CaloriesPer100g = 41, ProteinPer100g = 1, FatPer100g = 0, CarbsPer100g = 10, DefaultPortionGrams = 120 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111133"), Name = "Капуста белокочанная", CaloriesPer100g = 28, ProteinPer100g = 2, FatPer100g = 0, CarbsPer100g = 5, DefaultPortionGrams = 180 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111134"), Name = "Апельсин", CaloriesPer100g = 43, ProteinPer100g = 1, FatPer100g = 0, CarbsPer100g = 9, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111135"), Name = "Груша", CaloriesPer100g = 57, ProteinPer100g = 0, FatPer100g = 0, CarbsPer100g = 15, DefaultPortionGrams = 150 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111136"), Name = "Киви", CaloriesPer100g = 50, ProteinPer100g = 1, FatPer100g = 1, CarbsPer100g = 11, DefaultPortionGrams = 120 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111137"), Name = "Арахис", CaloriesPer100g = 567, ProteinPer100g = 26, FatPer100g = 49, CarbsPer100g = 10, DefaultPortionGrams = 25 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111138"), Name = "Грецкий орех", CaloriesPer100g = 654, ProteinPer100g = 15, FatPer100g = 65, CarbsPer100g = 14, DefaultPortionGrams = 25 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111139"), Name = "Авокадо", CaloriesPer100g = 160, ProteinPer100g = 2, FatPer100g = 15, CarbsPer100g = 9, DefaultPortionGrams = 80 },
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111140"), Name = "Чечевица варёная", CaloriesPer100g = 116, ProteinPer100g = 9, FatPer100g = 0, CarbsPer100g = 20, DefaultPortionGrams = 150 }
    ];
}
