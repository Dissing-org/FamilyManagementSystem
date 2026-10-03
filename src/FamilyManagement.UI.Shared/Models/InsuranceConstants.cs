namespace FamilyManagement.UI.Shared.Models;

public static class InsuranceConstants
{
    public static readonly string[] Categories =
    {
        "Home", "Health", "Auto", "Life", "Travel", "Liability", "Pet", "Dental", "Disability", "Other"
    };

    public static readonly string[] Frequencies =
    {
        "Monthly", "Quarterly", "SemiAnnually", "Annually"
    };

    public static readonly string[] Currencies =
    {
        "DKK", "EUR", "USD", "GBP", "SEK", "NOK"
    };

    public static string FormatFrequency(string frequency) => frequency switch
    {
        "SemiAnnually" => "Semi-Annually",
        _ => frequency
    };
}
