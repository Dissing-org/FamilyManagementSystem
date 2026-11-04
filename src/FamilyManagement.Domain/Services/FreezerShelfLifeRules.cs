using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Domain.Services;

public static class FreezerShelfLifeRules
{
    /// <summary>
    /// Gets the recommended shelf life duration for a given freezer food category.
    /// </summary>
    /// <param name="category">The freezer food category.</param>
    /// <returns>A <see cref="TimeSpan"/> representing the recommended storage duration in the freezer.</returns>
    public static TimeSpan GetRecommendedShelfLife(FreezerCategory category) => category switch
    {
        FreezerCategory.MeatAndPoultry => TimeSpan.FromDays(180),     // 6 months
        FreezerCategory.Seafood => TimeSpan.FromDays(120),            // 4 months
        FreezerCategory.VegetablesAndFruits => TimeSpan.FromDays(300),// 10 months
        FreezerCategory.PreparedMeals => TimeSpan.FromDays(90),       // 3 months
        FreezerCategory.Bakery => TimeSpan.FromDays(90),              // 3 months
        FreezerCategory.DessertsAndIceCream => TimeSpan.FromDays(180),// 6 months
        _ => TimeSpan.FromDays(180)
    };

    /// <summary>
    /// Calculates the default expiration date for a food item given its category and freeze date.
    /// </summary>
    /// <param name="category">The freezer food category.</param>
    /// <param name="freezeDate">The date and time when the item was frozen.</param>
    /// <returns>The calculated expiration <see cref="DateTime"/> based on recommended category shelf life.</returns>
    public static DateTime CalculateDefaultExpiry(FreezerCategory category, DateTime freezeDate) =>
        freezeDate.Add(GetRecommendedShelfLife(category));
}
