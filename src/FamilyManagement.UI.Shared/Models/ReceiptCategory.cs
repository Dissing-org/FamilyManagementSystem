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
        Groceries => "bg-wada-ice-100 text-wada-ice-800 border-wada-ice-300 dark:bg-wada-ice-950/60 dark:text-wada-ice-300 dark:border-wada-ice-800",
        Dining => "bg-wada-ecru-100 text-wada-ecru-800 border-wada-ecru-300 dark:bg-wada-ecru-950/60 dark:text-wada-ecru-300 dark:border-wada-ecru-800",
        Utilities => "bg-wada-marine-100 text-wada-marine-800 border-wada-marine-300 dark:bg-wada-marine-950/60 dark:text-wada-marine-300 dark:border-wada-marine-800",
        Entertainment => "bg-purple-100 text-purple-800 border-purple-300 dark:bg-purple-900/40 dark:text-purple-300",
        Health => "bg-rose-100 text-rose-800 border-rose-300 dark:bg-rose-900/40 dark:text-rose-300",
        Shopping => "bg-wada-marine-100 text-wada-marine-800 border-wada-marine-300 dark:bg-wada-marine-950/60 dark:text-wada-marine-300 dark:border-wada-marine-800",
        _ => "bg-slate-100 text-slate-800 border-slate-300 dark:bg-slate-800 dark:text-slate-300"
    };
}
