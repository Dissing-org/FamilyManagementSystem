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
        Groceries => "bg-wada-pistachio-100 text-wada-pistachio-800 border-wada-pistachio-300 dark:bg-wada-pistachio-950/60 dark:text-wada-pistachio-300 dark:border-wada-pistachio-800",
        Dining => "bg-wada-cinnamon-100 text-wada-cinnamon-800 border-wada-cinnamon-300 dark:bg-wada-cinnamon-950/60 dark:text-wada-cinnamon-300 dark:border-wada-cinnamon-800",
        Utilities => "bg-blue-100 text-blue-800 border-blue-300 dark:bg-blue-900/40 dark:text-blue-300",
        Entertainment => "bg-purple-100 text-purple-800 border-purple-300 dark:bg-purple-900/40 dark:text-purple-300",
        Health => "bg-rose-100 text-rose-800 border-rose-300 dark:bg-rose-900/40 dark:text-rose-300",
        Shopping => "bg-indigo-100 text-indigo-800 border-indigo-300 dark:bg-indigo-950/60 dark:text-indigo-300 dark:border-indigo-800",
        _ => "bg-slate-100 text-slate-800 border-slate-300 dark:bg-slate-800 dark:text-slate-300"
    };
}
