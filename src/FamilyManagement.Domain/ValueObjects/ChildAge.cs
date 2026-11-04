namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Represents the calculated age breakdown of a child across various time units.
/// </summary>
/// <param name="Years">The full years component of the age.</param>
/// <param name="Months">The remaining months component of the age.</param>
/// <param name="Days">The remaining days component of the age.</param>
/// <param name="TotalMonths">The total cumulative age in whole months.</param>
/// <param name="TotalWeeks">The total cumulative age in whole weeks.</param>
/// <param name="TotalDays">The total cumulative age in days.</param>
public record ChildAge(
    int Years,
    int Months,
    int Days,
    int TotalMonths,
    int TotalWeeks,
    int TotalDays)
{
    /// <summary>
    /// Gets a human-readable formatted representation of the age adapted to the child's stage.
    /// </summary>
    public string Formatted
    {
        get
        {
            if (Years >= 2)
            {
                return Months > 0 ? $"{Years} yrs {Months} mos" : $"{Years} years";
            }
            if (Years == 1)
            {
                return Months > 0 ? $"1 yr {Months} mos" : "1 year";
            }
            if (TotalMonths >= 3)
            {
                return Days > 0 ? $"{TotalMonths} mos {Days} d" : $"{TotalMonths} months";
            }
            if (TotalWeeks >= 1)
            {
                return $"{TotalWeeks} weeks";
            }
            return $"{TotalDays} days";
        }
    }

    /// <summary>
    /// Calculates the age breakdown for a child given their date of birth and a target reference date.
    /// </summary>
    /// <param name="dateOfBirth">The child's birth date.</param>
    /// <param name="asOfDate">The reference date to calculate the age as of.</param>
    /// <returns>A <see cref="ChildAge"/> representing the elapsed time breakdown.</returns>
    public static ChildAge Calculate(DateTime dateOfBirth, DateTime asOfDate)
    {
        if (asOfDate < dateOfBirth)
        {
            return new ChildAge(0, 0, 0, 0, 0, 0);
        }

        var totalDays = (int)(asOfDate.Date - dateOfBirth.Date).TotalDays;
        var totalWeeks = totalDays / 7;

        var years = asOfDate.Year - dateOfBirth.Year;
        var months = asOfDate.Month - dateOfBirth.Month;

        if (asOfDate.Day < dateOfBirth.Day)
        {
            months--;
        }

        if (months < 0)
        {
            years--;
            months += 12;
        }

        var lastAnchor = dateOfBirth.AddYears(years).AddMonths(months);
        var days = (int)(asOfDate.Date - lastAnchor.Date).TotalDays;
        var totalMonths = (years * 12) + months;

        return new ChildAge(years, months, days, totalMonths, totalWeeks, totalDays);
    }
}
