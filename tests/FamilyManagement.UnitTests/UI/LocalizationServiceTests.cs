using FamilyManagement.Application.UseCases.Children.Commands;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.UI.Shared.Services;
using FluentAssertions;
using NSubstitute;

namespace FamilyManagement.UnitTests.UI;

public class LocalizationServiceTests
{
    private readonly LocalizationService _sut = new();

    [Fact]
    public void Culture_ShouldBeDanish()
    {
        _sut.Culture.Name.Should().Be("da-DK");
    }

    [Theory]
    [InlineData("App.Title", "Familie Hub")]
    [InlineData("Home.Subtitle", "Dagligt overblik & tidslinje")]
    [InlineData("Receipts.Upload", "Upload")]
    [InlineData("Insurances.AddPolicy", "Tilføj police")]
    [InlineData("Children.AddChild", "Tilføj barn")]
    [InlineData("Vehicles.AddVehicle", "Tilføj bil")]
    [InlineData("NotFound.Title", "Siden blev ikke fundet")]
    public void Indexer_ShouldReturnDanishTranslation_ForKnownKeys(string key, string expected)
    {
        _sut[key].Should().Be(expected);
    }

    [Fact]
    public void Indexer_ShouldReturnKeyItself_WhenKeyIsUnknown()
    {
        _sut["Unknown.NonExistentKey"].Should().Be("Unknown.NonExistentKey");
    }

    [Fact]
    public void Format_ShouldInterpolateArgumentsUsingDanishCulture()
    {
        var result = _sut.Format("Vehicles.LogsRecordedFormat", 5);
        result.Should().Be("5 aflæsninger registreret");
    }

    [Theory]
    [InlineData("Groceries", "Dagligvarer")]
    [InlineData("Dining", "Restaurant & Café")]
    [InlineData("Utilities", "Forbrug & Forsyning")]
    [InlineData("Entertainment", "Underholdning")]
    [InlineData("Health", "Sundhed")]
    [InlineData("Shopping", "Shopping")]
    [InlineData("Home & Garden", "Hus & Have")]
    [InlineData("Kids & Toys", "Børn & Legetøj")]
    [InlineData("Other", "Andet")]
    [InlineData("None", "Ingen")]
    public void Category_ShouldTranslateReceiptCategoriesToDanish(string category, string expected)
    {
        _sut.Category(category).Should().Be(expected);
    }

    [Theory]
    [InlineData("Home", "Hus & Indbo")]
    [InlineData("Health", "Sundhed")]
    [InlineData("Auto", "Bil")]
    [InlineData("Life", "Livsforsikring")]
    [InlineData("Travel", "Rejse")]
    [InlineData("Liability", "Ansvar")]
    [InlineData("Pet", "Kæledyr")]
    [InlineData("Dental", "Tandlæge")]
    [InlineData("Disability", "Ulykke & Invaliditet")]
    [InlineData("Other", "Andet")]
    public void InsuranceCategory_ShouldTranslateInsuranceCategoriesToDanish(string category, string expected)
    {
        _sut.InsuranceCategory(category).Should().Be(expected);
    }

    [Theory]
    [InlineData("Monthly", "Månedlig")]
    [InlineData("Quarterly", "Kvartalsvis")]
    [InlineData("SemiAnnually", "Halvårlig")]
    [InlineData("Semi-Annually", "Halvårlig")]
    [InlineData("Annually", "Årlig")]
    public void Frequency_ShouldTranslatePaymentFrequenciesToDanish(string frequency, string expected)
    {
        _sut.Frequency(frequency).Should().Be(expected);
    }

    [Theory]
    [InlineData("Active", "Aktiv")]
    [InlineData("Expired", "Udløbet")]
    [InlineData("Cancelled", "Opsagt")]
    [InlineData("PendingRenewal", "Afventer fornyelse")]
    public void PolicyStatus_ShouldTranslateStatusesToDanish(string status, string expected)
    {
        _sut.PolicyStatus(status).Should().Be(expected);
    }

    [Theory]
    [InlineData("GrossMotor", "Grovmotorik")]
    [InlineData("FineMotor", "Finmotorik")]
    [InlineData("LanguageSpeech", "Sprog & Tale")]
    [InlineData("SocialEmotional", "Social & Følelsesmæssig")]
    [InlineData("Cognitive", "Kognitiv")]
    [InlineData("Dental", "Tænder")]
    [InlineData("HealthSelfCare", "Sundhed & Selvhjælp")]
    [InlineData("Other", "Andet")]
    public void MilestoneCategory_ShouldTranslateCategoriesToDanish(string category, string expected)
    {
        _sut.MilestoneCategory(category).Should().Be(expected);
    }

    [Theory]
    [InlineData("Expected", "Forventet")]
    [InlineData("Achieved", "Opnået")]
    public void MilestoneStatus_ShouldTranslateStatusesToDanish(string status, string expected)
    {
        _sut.MilestoneStatus(status).Should().Be(expected);
    }

    [Theory]
    [InlineData("RegularService", "Almindeligt service")]
    [InlineData("OilChange", "Olieskift")]
    [InlineData("Inspection", "Syn")]
    [InlineData("InspectionSyn", "Syn")]
    [InlineData("TireChange", "Hjulskifte")]
    [InlineData("BrakeService", "Bremseservice")]
    [InlineData("Repair", "Reparation")]
    [InlineData("BatteryReplacement", "Batteriskift")]
    [InlineData("Other", "Andet")]
    public void ServiceType_ShouldTranslateServiceTypesToDanish(string serviceType, string expected)
    {
        _sut.ServiceType(serviceType).Should().Be(expected);
    }

    [Theory]
    [InlineData("Petrol", "Benzin")]
    [InlineData("Gasoline", "Benzin")]
    [InlineData("Diesel", "Diesel")]
    [InlineData("Electric", "El")]
    [InlineData("Hybrid", "Hybrid")]
    [InlineData("PlugInHybrid", "Plug-in Hybrid")]
    [InlineData("Other", "Andet")]
    public void FuelType_ShouldTranslateFuelTypesToDanish(string fuelType, string expected)
    {
        _sut.FuelType(fuelType).Should().Be(expected);
    }

    [Theory]
    [InlineData("Boy", "Dreng")]
    [InlineData("Girl", "Pige")]
    [InlineData("Other", "Andet")]
    public void Gender_ShouldTranslateGendersToDanish(string gender, string expected)
    {
        _sut.Gender(gender).Should().Be(expected);
    }

    [Theory]
    [InlineData(0, "0 mdr.")]
    [InlineData(1, "1 mdr.")]
    [InlineData(8, "8 mdr.")]
    [InlineData(12, "12 mdr.")]
    [InlineData(24, "2 år")]
    [InlineData(27, "2 år 3 mdr.")]
    public void FormatAgeMonths_ShouldFormatCorrectlyInDanish(int months, string expected)
    {
        _sut.FormatAgeMonths(months).Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 3, 5, "2 år 3 mdr.")]
    [InlineData(1, 0, 10, "1 år")]
    [InlineData(0, 8, 4, "8 mdr. 4 d.")]
    [InlineData(0, 5, 0, "5 mdr.")]
    [InlineData(0, 0, 14, "14 d.")]
    public void FormatAge_ShouldFormatYearsMonthsDaysInDanish(int years, int months, int days, string expected)
    {
        _sut.FormatAge(years, months, days).Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 3, "2 mdr. – 3 mdr.")]
    [InlineData(6, 6, "6 mdr.")]
    [InlineData(24, null, "2 år")]
    public void FormatAgeWindow_ShouldFormatMilestoneWindowInDanish(int start, int? end, string expected)
    {
        _sut.FormatAgeWindow(start, end).Should().Be(expected);
    }

    [Fact]
    public void FormatDate_ShouldFormatUsingDanishCulture()
    {
        var date = new DateTime(2026, 5, 14);
        var formatted = _sut.FormatDate(date, "d. MMM yyyy");
        formatted.Should().Contain("14.").And.Contain("maj").And.Contain("2026");
    }

    [Fact]
    public void MilestoneText_ShouldTranslateLegacyEnglishStandardMilestoneStrings()
    {
        _sut.MilestoneText("Smiles at people").Should().Be("Smiler til mennesker");
        _sut.MilestoneText("First baby tooth emerges").Should().Be("Første mælketand bryder frem");
    }

    [Fact]
    public async Task SeedStandardMilestonesCommandHandler_ShouldSeedDanishMilestones()
    {
        var milestoneRepo = Substitute.For<IChildMilestoneRepository>();
        var childRepo = Substitute.For<IChildProfileRepository>();
        var child = ChildProfile.Create("Freja", null, new DateTime(2025, 1, 15), Gender.Girl);

        childRepo.GetByIdAsync(child.Id, Arg.Any<CancellationToken>()).Returns(child);
        milestoneRepo.GetByChildIdAsync(child.Id, Arg.Any<MilestoneStatus?>(), Arg.Any<MilestoneCategory?>(), Arg.Any<CancellationToken>())
            .Returns(new List<ChildMilestone>());

        List<ChildMilestone>? addedMilestones = null;
        await milestoneRepo.AddRangeAsync(
            Arg.Do<IEnumerable<ChildMilestone>>(items => addedMilestones = items.ToList()),
            Arg.Any<CancellationToken>());

        var handler = new SeedStandardMilestonesCommandHandler(milestoneRepo, childRepo);
        var count = await handler.HandleAsync(new SeedStandardMilestonesCommand(child.Id.Value));

        count.Should().Be(27);
        addedMilestones.Should().NotBeNull();
        addedMilestones!.Select(m => m.Title).Should().Contain(new[]
        {
            "Smiler til mennesker",
            "Første mælketand bryder frem",
            "Sidder uden støtte",
            "Første selvstændige skridt / Går selv",
            "Renlig om dagen",
            "Taber første mælketand"
        });
        addedMilestones.Should().OnlyContain(m => !string.IsNullOrWhiteSpace(m.Description));
    }

    [Fact]
    public void AllRazorTranslationKeys_ShouldExistInDictionary()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
        var razorDirs = new[]
        {
            Path.Combine(repoRoot, "src", "FamilyManagement.UI.Shared"),
            Path.Combine(repoRoot, "src", "FamilyManagement.Web")
        };

        var regex = new System.Text.RegularExpressions.Regex(@"\bL(?:\[\s*""([^""]+)""\s*\]|\.Format\(\s*""([^""]+)""\s*,)");
        var missingKeys = new List<string>();

        foreach (var dir in razorDirs.Where(Directory.Exists))
        {
            foreach (var file in Directory.GetFiles(dir, "*.razor", SearchOption.AllDirectories))
            {
                var content = File.ReadAllText(file);
                foreach (System.Text.RegularExpressions.Match match in regex.Matches(content))
                {
                    var key = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                    if (_sut[key] == key)
                    {
                        missingKeys.Add($"{Path.GetFileName(file)}: {key}");
                    }
                }
            }
        }

        missingKeys.Should().BeEmpty();
    }
}
