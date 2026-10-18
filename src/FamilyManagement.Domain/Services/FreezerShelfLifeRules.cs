using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Domain.Services;

public static class FreezerShelfLifeRules
{
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

    public static DateTime CalculateDefaultExpiry(FreezerCategory category, DateTime freezeDate) =>
        freezeDate.Add(GetRecommendedShelfLife(category));
}
