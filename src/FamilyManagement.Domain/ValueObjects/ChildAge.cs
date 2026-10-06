namespace FamilyManagement.Domain.ValueObjects;

public record ChildAge(
    int Years,
    int Months,
    int Days,
    int TotalMonths,
    int TotalWeeks,
    int TotalDays)
{
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
