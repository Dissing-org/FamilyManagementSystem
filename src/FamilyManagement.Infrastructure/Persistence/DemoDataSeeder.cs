using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Storage;

namespace FamilyManagement.Infrastructure.Persistence;

public class DemoDataSeeder : IDemoDataSeeder
{
    private static readonly SemaphoreSlim ResetLock = new(1, 1);

    private readonly IConfiguration _configuration;
    private readonly GoogleDriveOptions _driveOptions;
    private readonly ILogger<DemoDataSeeder> _logger;

    public DemoDataSeeder(
        IConfiguration configuration,
        IOptions<GoogleDriveOptions> driveOptions,
        ILogger<DemoDataSeeder> logger)
    {
        _configuration = configuration;
        _driveOptions = driveOptions.Value;
        _logger = logger;
    }

    public async Task EnsureSeededAsync(CancellationToken ct = default)
    {
        await ResetLock.WaitAsync(ct);
        try
        {
            await using var db = CreateDemoDbContext();
            await SqliteSchemaInitializer.EnsureSchemaAndMigrateAsync(db, _logger, ct);

            var hasReceipts = await db.Receipts.AnyAsync(ct);
            var hasVehicles = await db.Vehicles.AnyAsync(ct);
            var hasChildren = await db.ChildProfiles.AnyAsync(ct);
            var hasInsurances = await db.InsurancePolicies.AnyAsync(ct);

            if (!hasReceipts && !hasVehicles && !hasChildren && !hasInsurances)
            {
                await SeedDataCoreAsync(db, ct);
                _logger.LogInformation("Initialized demo.db with Danish family showcase dataset.");
            }
        }
        finally
        {
            ResetLock.Release();
        }
    }

    public async Task ResetAndSeedAsync(CancellationToken ct = default)
    {
        await ResetLock.WaitAsync(ct);
        try
        {
            await using var db = CreateDemoDbContext();
            await SqliteSchemaInitializer.EnsureSchemaAndMigrateAsync(db, _logger, ct);

            await db.MileageLogs.ExecuteDeleteAsync(ct);
            await db.VehicleServices.ExecuteDeleteAsync(ct);
            await db.Vehicles.ExecuteDeleteAsync(ct);
            await db.GrowthMeasurements.ExecuteDeleteAsync(ct);
            await db.ChildMilestones.ExecuteDeleteAsync(ct);
            await db.ChildProfiles.ExecuteDeleteAsync(ct);
            await db.InsurancePolicies.ExecuteDeleteAsync(ct);
            await db.Receipts.ExecuteDeleteAsync(ct);

            var demoStorageDir = ResolveDemoStorageDirectory();
            if (Directory.Exists(demoStorageDir))
            {
                try
                {
                    Directory.Delete(demoStorageDir, recursive: true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not completely delete demo storage folder '{Dir}', cleaning files individually.", demoStorageDir);
                    foreach (var file in Directory.GetFiles(demoStorageDir, "*", SearchOption.AllDirectories))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }

            Directory.CreateDirectory(demoStorageDir);

            await SeedDataCoreAsync(db, ct);
            _logger.LogInformation("Reset and re-seeded demo.db and demo_receipts_storage to Danish family showcase state.");
        }
        finally
        {
            ResetLock.Release();
        }
    }

    private ReceiptDbContext CreateDemoDbContext()
    {
        var connStr = _configuration.GetConnectionString("DemoDatabase") ?? "Data Source=demo.db";
        var optionsBuilder = new DbContextOptionsBuilder<ReceiptDbContext>();
        optionsBuilder.UseSqlite(connStr);
        return new ReceiptDbContext(optionsBuilder.Options);
    }

    private string ResolveDemoStorageDirectory()
    {
        var dir = string.IsNullOrWhiteSpace(_driveOptions.DemoLocalStorageDirectory)
            ? "demo_receipts_storage"
            : _driveOptions.DemoLocalStorageDirectory;

        return Path.IsPathRooted(dir)
            ? dir
            : Path.Combine(AppContext.BaseDirectory, dir);
    }

    private async Task SeedDataCoreAsync(ReceiptDbContext db, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var astrid = ChildProfile.Create(
            firstName: "Astrid",
            lastName: "Lindholm",
            dateOfBirth: now.Date.AddYears(-6).AddDays(-24),
            gender: Gender.Girl,
            initialSizes: new ChildWardrobeSizes(
                ClothesSize: "116 (6 år)",
                ShoeSize: "EU 29",
                HatSize: "52 cm",
                DiaperSize: "—"));

        var felix = ChildProfile.Create(
            firstName: "Felix",
            lastName: "Lindholm",
            dateOfBirth: now.Date.AddYears(-3).AddDays(-40),
            gender: Gender.Boy,
            initialSizes: new ChildWardrobeSizes(
                ClothesSize: "98 (3 år)",
                ShoeSize: "EU 25",
                HatSize: "50 cm",
                DiaperSize: "Str. 5 (Nat)"));

        db.ChildProfiles.AddRange(astrid, felix);

        db.GrowthMeasurements.AddRange(
            GrowthMeasurement.Create(astrid.Id, now.Date.AddMonths(-22), 104.5m, 16.8m, 50.5m, "4-års børneundersøgelse hos egen læge"),
            GrowthMeasurement.Create(astrid.Id, now.Date.AddMonths(-15), 109.0m, 18.2m, 51.0m, "Målt hjemme før sommerferie"),
            GrowthMeasurement.Create(astrid.Id, now.Date.AddMonths(-8), 113.5m, 19.6m, 51.6m, "5-års børneundersøgelse og vaccination"),
            GrowthMeasurement.Create(astrid.Id, now.Date.AddMonths(-1), 117.2m, 21.1m, 52.0m, "Skolestart helbredstjek"),

            GrowthMeasurement.Create(felix.Id, now.Date.AddMonths(-20), 82.0m, 11.4m, 47.8m, "18-måneders tjek"),
            GrowthMeasurement.Create(felix.Id, now.Date.AddMonths(-13), 88.5m, 12.9m, 48.9m, "2-års undersøgelse"),
            GrowthMeasurement.Create(felix.Id, now.Date.AddMonths(-6), 93.2m, 14.1m, 49.5m, "Vuggestue opfølgning"),
            GrowthMeasurement.Create(felix.Id, now.Date.AddMonths(-1), 97.4m, 15.2m, 50.0m, "3-års børneundersøgelse — fin vækstkurve"));

        db.ChildMilestones.AddRange(
            ChildMilestone.CreateAchieved(astrid.Id, "Cykler uden støttehjul i Fælledparken", MilestoneCategory.GrossMotor, now.Date.AddMonths(-14), 58, "Lærte at holde balancen på sin røde Puky cykel"),
            ChildMilestone.CreateAchieved(astrid.Id, "Skriver sit eget navn og tæller til 100", MilestoneCategory.Cognitive, now.Date.AddMonths(-7), 65, "Klar til børnehaveklasse i august"),
            ChildMilestone.CreateAchieved(astrid.Id, "Tabte første mælketand", MilestoneCategory.Dental, now.Date.AddMonths(-3), 69, "Tandfeen lagde en 20-krone under puden"),
            ChildMilestone.CreateExpected(astrid.Id, "Svømmer 25 meter uden bælte", MilestoneCategory.GrossMotor, 74, 84, "Går til svømning i Øbro-Hallen hver lørdag", true),
            ChildMilestone.CreateExpected(astrid.Id, "Læser første korte børnebog selv", MilestoneCategory.LanguageSpeech, 76, 86, "Øver lydrette ord med Søren og Mette", true),

            ChildMilestone.CreateAchieved(felix.Id, "Sammensætter sætninger på 4-5 ord", MilestoneCategory.LanguageSpeech, now.Date.AddMonths(-8), 29, "Fortæller ivrigt om gravkøer og brandbiler"),
            ChildMilestone.CreateAchieved(felix.Id, "Hopper med samlede ben fra trappetrin", MilestoneCategory.GrossMotor, now.Date.AddMonths(-3), 34, "Elsker legepladsen i børnehaven"),
            ChildMilestone.CreateExpected(felix.Id, "Ble-fri om dagen", MilestoneCategory.HealthSelfCare, 36, 42, "Bruger potten fast derhjemme og i børnehaven", true),
            ChildMilestone.CreateExpected(felix.Id, "Klipper langs en lige linje med børnesaks", MilestoneCategory.FineMotor, 38, 44, "Krea-værksted med storesøster", true));

        var volvo = Vehicle.Create(
            make: "Volvo",
            model: "XC90 Recharge T8",
            year: 2024,
            licensePlate: "AB 40 291",
            fuelType: FuelType.Hybrid,
            currentMileageKm: 28450,
            vin: "YV1LFBMUDR1104921",
            serviceIntervalKm: 30000,
            serviceIntervalMonths: 12,
            nextInspectionDate: now.Date.AddMonths(3));

        var tesla = Vehicle.Create(
            make: "Tesla",
            model: "Model Y Long Range",
            year: 2023,
            licensePlate: "XY 88 124",
            fuelType: FuelType.Electric,
            currentMileageKm: 42100,
            vin: "XP7YGCEK8PB092341",
            serviceIntervalKm: 50000,
            serviceIntervalMonths: 24,
            nextInspectionDate: now.Date.AddMonths(8));

        db.Vehicles.AddRange(volvo, tesla);

        db.MileageLogs.AddRange(
            MileageLogEntry.Create(volvo.Id, now.Date.AddMonths(-6), 19200, "Kør-selv skiferie til Sälen i Sverige"),
            MileageLogEntry.Create(volvo.Id, now.Date.AddMonths(-3), 24100, "Sommerhusweekend i Tisvildeleje"),
            MileageLogEntry.Create(volvo.Id, now.Date.AddDays(-7), 28450, "Aflæst ved skift til sommerdæk"),

            MileageLogEntry.Create(tesla.Id, now.Date.AddMonths(-6), 31500, "Pendling København - Roskilde"),
            MileageLogEntry.Create(tesla.Id, now.Date.AddMonths(-2), 38200, "Familiebesøg i Aarhus"),
            MileageLogEntry.Create(tesla.Id, now.Date.AddDays(-4), 42100, "Månedlig kilometerstatus"));

        db.VehicleServices.AddRange(
            VehicleServiceRecord.Create(
                volvo.Id,
                now.Date.AddMonths(-9),
                15000,
                ServiceType.RegularService,
                "15.000 km autoriseret Volvo eftersyn & softwareopdatering",
                workshop: "Bilia København",
                cost: 4295m,
                notes: "Skiftet olie, pollenfilter og opdateret Google Automotive OS"),
            VehicleServiceRecord.Create(
                volvo.Id,
                now.Date.AddDays(-14),
                28100,
                ServiceType.TireChange,
                "Skift til Michelin sommerhjul & dækhotel",
                workshop: "Bilia København",
                cost: 1150m,
                notes: "Vinterdæk opbevaret på dækhotel med 6.5 mm mønster"),

            VehicleServiceRecord.Create(
                tesla.Id,
                now.Date.AddMonths(-7),
                30000,
                ServiceType.BrakeService,
                "Bremserens, smøring af kalibre & HEPA kabinefilter",
                workshop: "Tesla Service Kastrup",
                cost: 2850m,
                notes: "Forebyggende vedligeholdelse af bremser pga. regenerativ bremsning"),
            VehicleServiceRecord.Create(
                tesla.Id,
                now.Date.AddMonths(-1),
                40500,
                ServiceType.TireChange,
                "4-hjulsudmåling og hjulskifte",
                workshop: "Tesla Service Kastrup",
                cost: 1690m,
                notes: "Sporet og afbalanceret alle fire hjul"));

        db.InsurancePolicies.AddRange(
            InsurancePolicy.Create(
                insurer: "Tryg Forsikring",
                policyNumber: "POL-DK-884120",
                category: InsuranceCategory.Home,
                insuredParty: "Familien Lindholm — Murermestervilla",
                premium: Money.Create(985m, "DKK"),
                frequency: PaymentFrequency.Monthly,
                startDate: now.Date.AddYears(-2),
                renewalDate: now.Date.AddDays(18),
                deductible: Money.Create(4500m, "DKK"),
                notes: "Inkluderer udvidet rørskade, svamp/insekt og pludselig skade på indbo"),
            InsurancePolicy.Create(
                insurer: "Topdanmark",
                policyNumber: "POL-DK-331094",
                category: InsuranceCategory.Auto,
                insuredParty: "Volvo XC90 & Tesla Model Y",
                premium: Money.Create(2450m, "DKK"),
                frequency: PaymentFrequency.Quarterly,
                startDate: now.Date.AddYears(-1),
                renewalDate: now.Date.AddDays(145),
                deductible: Money.Create(3000m, "DKK"),
                notes: "Kasko, glasdækning, fri kilometer og vejhjælp i hele Europa"),
            InsurancePolicy.Create(
                insurer: "Sygeforsikringen \"danmark\" & Danica",
                policyNumber: "POL-DK-552910",
                category: InsuranceCategory.Health,
                insuredParty: "Hele Husstanden (4 personer)",
                premium: Money.Create(420m, "DKK"),
                frequency: PaymentFrequency.Monthly,
                startDate: now.Date.AddYears(-3),
                renewalDate: now.Date.AddDays(95),
                deductible: Money.Create(0m, "DKK"),
                notes: "Gruppe 5 dækning til børn og voksne samt børneulykkesforsikring"),
            InsurancePolicy.Create(
                insurer: "Europæiske ERV",
                policyNumber: "POL-DK-119042",
                category: InsuranceCategory.Travel,
                insuredParty: "Årsrejseforsikring Verden",
                premium: Money.Create(1850m, "DKK"),
                frequency: PaymentFrequency.Annually,
                startDate: now.Date.AddMonths(-5),
                renewalDate: now.Date.AddDays(210),
                deductible: Money.Create(500m, "DKK"),
                notes: "Dækker skiferie, afbestilling, bagageforsinkelse og hjemtransport"));

        var receiptSpecs = new (string Merchant, string Category, int DaysAgo, string Notes, string AmountText)[]
        {
            ("Føtex Østerbro", "Groceries", 2, "Ugentlig storindkøb og økologisk frugt", "842,50 DKK"),
            ("IKEA Gentofte", "Home & Garden", 5, "Børneværelse opbevaring og LED lamper", "1.495,00 DKK"),
            ("LEGO Store København", "Kids & Toys", 9, "Astrid fødselsdagsgave — LEGO Technic", "699,95 DKK"),
            ("Clever Lynladning", "Utilities", 12, "Hurtigladning på tværs af Storebælt", "312,00 DKK"),
            ("Matas Striberne", "Health", 15, "Solcreme faktor 50 og børnevitaminer", "428,00 DKK"),
            ("Nemlig.com", "Groceries", 19, "Levering til døren — aftensmad 4 dage", "1.120,00 DKK"),
            ("Silvan Nordhavn", "Home & Garden", 24, "Træbeskyttelse og pensler til terrasse", "789,00 DKK"),
            ("Illums Bolighus", "Other", 28, "Værtindegave og Georg Jensen lysestage", "950,00 DKK")
        };

        var baseStorageDir = ResolveDemoStorageDirectory();
        foreach (var spec in receiptSpecs)
        {
            var purchaseDate = now.AddDays(-spec.DaysAgo);
            var year = purchaseDate.Year.ToString();
            var folderCategory = GoogleDriveStorageService.SanitizeFolderName(spec.Category);
            var targetDir = Path.Combine(baseStorageDir, year, folderCategory);
            Directory.CreateDirectory(targetDir);

            var fileId = Guid.NewGuid().ToString("N");
            var safeMerchant = GoogleDriveStorageService.SanitizeFolderName(spec.Merchant).Replace(' ', '_').ToLowerInvariant();
            var savedFileName = $"{purchaseDate:yyyyMMdd}_{safeMerchant}.pdf";
            var fullPath = Path.Combine(targetDir, $"{fileId}_{savedFileName}");

            var pdfBytes = BuildReceiptPdfBytes(spec.Merchant, purchaseDate, spec.Category, spec.Notes, spec.AmountText);
            await File.WriteAllBytesAsync(fullPath, pdfBytes, ct);

            var fileRef = GoogleDriveFileReference.Create(
                fileId,
                savedFileName,
                $"/api/receipts/files/{fileId}");

            var receipt = Receipt.Create(
                merchant: spec.Merchant,
                purchaseDate: purchaseDate,
                fileReference: fileRef,
                category: spec.Category,
                notes: spec.Notes);

            db.Receipts.Add(receipt);
        }

        await db.SaveChangesAsync(ct);
    }

    private static byte[] BuildReceiptPdfBytes(
        string merchant,
        DateTime purchaseDate,
        string category,
        string notes,
        string amountText)
    {
        static string AsciiSanitize(string text)
        {
            return text
                .Replace("æ", "ae").Replace("Æ", "AE")
                .Replace("ø", "oe").Replace("Ø", "OE")
                .Replace("å", "aa").Replace("AA", "AA")
                .Replace("—", "-")
                .Replace("(", "\\(").Replace(")", "\\)");
        }

        var m = AsciiSanitize(merchant);
        var c = AsciiSanitize(category);
        var n = AsciiSanitize(notes);
        var a = AsciiSanitize(amountText);
        var d = purchaseDate.ToString("yyyy-MM-dd HH:mm");

        var contentStream =
            "BT\n" +
            "/F1 18 Tf\n" +
            "50 740 Td\n" +
            $"(KVITTERING - {m}) Tj\n" +
            "/F1 12 Tf\n" +
            "0 -30 Td\n" +
            $"(Dato: {d}) Tj\n" +
            "0 -20 Td\n" +
            $"(Kategori: {c}) Tj\n" +
            "0 -20 Td\n" +
            $"(Beloeb: {a}) Tj\n" +
            "0 -20 Td\n" +
            $"(Noter: {n}) Tj\n" +
            "0 -35 Td\n" +
            "(Familien Lindholm - Demo Sandbox Kvittering) Tj\n" +
            "ET\n";

        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");
        var obj1Offset = sb.Length;
        sb.Append("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        var obj2Offset = sb.Length;
        sb.Append("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        var obj3Offset = sb.Length;
        sb.Append("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R /Resources << /Font << /F1 5 0 R >> >> >>\nendobj\n");
        var obj4Offset = sb.Length;
        sb.Append($"4 0 obj\n<< /Length {Encoding.ASCII.GetByteCount(contentStream)} >>\nstream\n{contentStream}endstream\nendobj\n");
        var obj5Offset = sb.Length;
        sb.Append("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>\nendobj\n");
        var xrefOffset = sb.Length;
        sb.Append("xref\n0 6\n");
        sb.Append("0000000000 65535 f \n");
        sb.Append($"{obj1Offset:D10} 00000 n \n");
        sb.Append($"{obj2Offset:D10} 00000 n \n");
        sb.Append($"{obj3Offset:D10} 00000 n \n");
        sb.Append($"{obj4Offset:D10} 00000 n \n");
        sb.Append($"{obj5Offset:D10} 00000 n \n");
        sb.Append($"trailer\n<< /Size 6 /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");

        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}
