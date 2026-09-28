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
        Groceries => "bg-emerald-100 text-emerald-800 border-emerald-300 dark:bg-emerald-900/40 dark:text-emerald-300",
        Dining => "bg-amber-100 text-amber-800 border-amber-300 dark:bg-amber-900/40 dark:text-amber-300",
        Utilities => "bg-blue-100 text-blue-800 border-blue-300 dark:bg-blue-900/40 dark:text-blue-300",
        Entertainment => "bg-purple-100 text-purple-800 border-purple-300 dark:bg-purple-900/40 dark:text-purple-300",
        Health => "bg-rose-100 text-rose-800 border-rose-300 dark:bg-rose-900/40 dark:text-rose-300",
        Shopping => "bg-indigo-100 text-indigo-800 border-indigo-300 dark:bg-indigo-900/40 dark:text-indigo-300",
        _ => "bg-slate-100 text-slate-800 border-slate-300 dark:bg-slate-800 dark:text-slate-300"
    };
}
