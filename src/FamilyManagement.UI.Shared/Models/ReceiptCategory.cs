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
        Groceries => "bg-emerald-50 text-emerald-800 border-emerald-200/60",
        Dining => "bg-amber-50 text-amber-800 border-amber-200/60",
        Utilities => "bg-cyan-50 text-cyan-800 border-cyan-200/60",
        Entertainment => "bg-purple-50 text-purple-700 border-purple-200/60",
        Health => "bg-rose-50 text-rose-700 border-rose-200/60",
        Shopping => "bg-sky-50 text-sky-700 border-sky-200/60",
        _ => "bg-slate-50 text-slate-700 border-slate-200/60"
    };
}
