namespace FamilyManagement.UI.Shared.Models;

public static class ReceiptCategories
{
    public const string Groceries = "Groceries";
    public const string Dining = "Dining";
    public const string Utilities = "Utilities";
    public const string Entertainment = "Entertainment";
    public const string Health = "Health";
    public const string Shopping = "Shopping";
    public const string Other = "Other";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Groceries,
        Dining,
        Utilities,
        Entertainment,
        Health,
        Shopping,
        Other
    };

    public static string GetBadgeClasses(string? category) => category switch
    {
        Groceries => "bg-[#FDEEE9] text-[#7B2F14] border-[#F9DDD4] dark:bg-[#C85A32]/20 dark:text-[#FDEEE9] dark:border-[#C85A32]/40",
        Dining => "bg-[#FEF3E2] text-[#B86B1E] border-[#FCE6C7] dark:bg-[#FEF3E2]/15 dark:text-[#E89E4E] dark:border-[#D9822B]/40",
        Utilities => "bg-[#EAF0EB] text-[#4A6243] border-[#D4E2D6] dark:bg-[#5B7553]/25 dark:text-[#8EAE84] dark:border-[#5B7553]/40",
        Entertainment => "bg-purple-100 text-purple-800 border-purple-300 dark:bg-purple-900/40 dark:text-purple-300",
        Health => "bg-rose-100 text-rose-800 border-rose-300 dark:bg-rose-900/40 dark:text-rose-300",
        Shopping => "bg-[#FDF6F3] text-[#993B19] border-[#EADECF] dark:bg-[#3D2F28] dark:text-[#FAF0E6] dark:border-[#523F34]",
        _ => "bg-[#FAF6F0] text-[#7F6A5D] border-[#EADECF] dark:bg-[#261D19] dark:text-[#FAF0E6] dark:border-[#3D2F28]"
    };
}
