using System.Globalization;

namespace FamilyManagement.UI.Shared.Services;

public interface ILocalizationService
{
    CultureInfo Culture { get; }
    string this[string key] { get; }
    string Format(string key, params object[] args);
    string Category(string? category);
    string InsuranceCategory(string? category);
    string Frequency(string? frequency);
    string PolicyStatus(string? status);
    string MilestoneCategory(string? category);
    string MilestoneStatus(string? status);
    string ServiceType(string? serviceType);
    string FuelType(string? fuelType);
    string Gender(string? gender);
    string FormatDate(DateTime date, string format = "d. MMM yyyy");
    string FormatAge(int years, int months, int days);
    string FormatAgeMonths(int months);
    string FormatAgeWindow(int minMonths, int? maxMonths);
    string MilestoneText(string? text);
}

public sealed class LocalizationService : ILocalizationService
{
    private static readonly CultureInfo DanishCulture = new("da-DK");

    private static readonly IReadOnlyDictionary<string, string> Translations =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // App & Shell
            ["App.Title"] = "Familie Hub",
            ["App.Subtitle"] = "Nordisk husholdnings-system · Kvitteringer, Forsikringer, Børn & Biler",
            ["App.Loading"] = "Indlæser Familie Hub...",
            ["App.VerifyingSession"] = "Bekræfter arbejdsområde...",
            ["App.UnhandledError"] = "Der opstod en uventet fejl.",
            ["App.Reload"] = "Genindlæs",
            ["NotFound.Title"] = "Siden blev ikke fundet",
            ["NotFound.Message"] = "Beklager, det indhold du leder efter findes ikke.",

            // Navigation & Header
            ["Nav.Overview"] = "Overblik",
            ["Nav.Home"] = "Hjem",
            ["Nav.Receipts"] = "Kvitteringer",
            ["Nav.Insurances"] = "Forsikringer",
            ["Nav.Children"] = "Børn",
            ["Nav.Vehicles"] = "Biler",
            ["Nav.RenewalsPendingTitle"] = "{0} fornyelser afventer",
            ["Header.DefaultSubtitle"] = "Familiestyring",
            ["Header.DemoSandbox"] = "Demo-sandkasse",
            ["Header.DemoSandboxTitle"] = "Isoleret CV-demosandkasse",
            ["Header.ResetDemo"] = "Nulstil demodata",
            ["Header.ResetDemoTitle"] = "Nulstil demosandkassen til danske standarddata",
            ["Header.Resetting"] = "Nulstiller...",
            ["Header.ResetDone"] = "✓ Nulstillet!",
            ["Header.ServerSettings"] = "Serverindstillinger",
            ["Header.SignOut"] = "Log ud",

            // Common
            ["Common.All"] = "Alle",
            ["Common.None"] = "Ingen",
            ["Common.Cancel"] = "Annuller",
            ["Common.Save"] = "Gem",
            ["Common.Saving"] = "Gemmer...",
            ["Common.Delete"] = "Slet",
            ["Common.Edit"] = "Rediger",
            ["Common.Retry"] = "Prøv igen",
            ["Common.View"] = "Vis",
            ["Common.ViewAll"] = "Se alle",
            ["Common.ConfirmQuestion"] = "Bekræft?",
            ["Common.Date"] = "Dato",
            ["Common.Category"] = "Kategori",
            ["Common.Notes"] = "Noter",
            ["Common.NotesOptional"] = "Noter (valgfrit)",
            ["Common.Actions"] = "Handlinger",
            ["Common.Optional"] = "Valgfrit",

            // Login View
            ["Login.WorkspaceTitle"] = "Log ind på dit arbejdsområde",
            ["Login.WorkspaceSubtitle"] = "Beskyttet af Cloudflare Access for husstandens ejer, eller log ind med CV-demoadgang nedenfor.",
            ["Login.EmailLabel"] = "E-mailadresse",
            ["Login.PasswordLabel"] = "Adgangskode",
            ["Login.SigningIn"] = "Logger ind...",
            ["Login.SignInButton"] = "Log ind på Familie Hub",
            ["Login.SandboxBadge"] = "Rekruttering & CV-sandkasse",
            ["Login.FillDemoCredentials"] = "✨ Udfyld CV-demoadgang",
            ["Login.SandboxFooter"] = "Demotilstand kører i en isoleret SQLite-sandkasse (demo.db) med realistiske danske husholdningsdata. Du kan frit tilføje, redigere eller nulstille data når som helst.",
            ["Login.InvalidCredentials"] = "Ugyldige loginoplysninger. Prøv at klikke på 'Udfyld CV-demoadgang'.",

            // API Settings Modal
            ["Settings.Title"] = "Serverforbindelse",
            ["Settings.Description"] = "Konfigurer URL til backend-serveren. Brug https://10.0.2.2:7193 til Android-emulator. På fysisk telefon via Wi-Fi bruges din PC's LAN-IP.",
            ["Settings.BackendUrlLabel"] = "Backend API-URL",
            ["Settings.AndroidEmulator"] = "Android-emulator",
            ["Settings.Localhost"] = "Localhost",
            ["Settings.SaveAndReconnect"] = "Gem og genopret forbindelse",

            // Family Home
            ["Home.Subtitle"] = "Dagligt overblik & tidslinje",
            ["Home.GreetingMorning"] = "Godmorgen",
            ["Home.GreetingAfternoon"] = "God eftermiddag",
            ["Home.GreetingEvening"] = "Godaften",
            ["Home.SnapshotSubtitle"] = "Her er dagens overblik over familiens husholdning.",
            ["Home.RenewalsPending"] = "{0} fornyelser afventer",
            ["Home.AllPoliciesUpToDate"] = "Alle policer er opdaterede",
            ["Home.Action.LogExpense"] = "Registrer udgift",
            ["Home.Action.RecordMileage"] = "Registrer km-stand",
            ["Home.Action.MeasureChild"] = "Mål barn",
            ["Home.Action.NewPolicy"] = "Ny police",
            ["Home.SyncingRecords"] = "Synkroniserer familiedata...",
            ["Home.UpcomingRenewalTitle"] = "Kommende policefornyelse",
            ["Home.ExpiresInDays"] = "udløber om",
            ["Home.Days"] = "dage",
            ["Home.ReviewInInsurances"] = "Se under Forsikringer",
            ["Home.RecentActivity"] = "Seneste aktivitet i husstanden",
            ["Home.LiveEventStream"] = "Aktivitetsstrøm",
            ["Home.ArchivedToDrive"] = "Arkiveret i Google Drev",
            ["Home.NoTransactionsYet"] = "Ingen registrerede transaktioner endnu.",
            ["Home.InspectionSchedule"] = "Synsplan",
            ["Home.BilsynDue"] = "Bilsyn senest {0}",
            ["Home.DomainSnapshots"] = "Domæneoverblik",
            ["Home.FourPillars"] = "4 områder",
            ["Home.ChildrenAndSizes"] = "Børn & størrelser",
            ["Home.ProfilesLink"] = "Profiler",
            ["Home.ClothesShort"] = "Tøj",
            ["Home.ShoesShort"] = "Sko",
            ["Home.NoChildrenYet"] = "Ingen børn registreret endnu.",
            ["Home.VehiclesFleet"] = "Bilpark",
            ["Home.GarageLink"] = "Garage",
            ["Home.NoVehiclesTracked"] = "Ingen biler registreret.",
            ["Home.FamilyInsurances"] = "Familieforsikringer",
            ["Home.PoliciesActive"] = "aktive policer",
            ["Home.PerMonthShort"] = "md.",
            ["Home.PurchaseReceipts"] = "Købskvitteringer",
            ["Home.TotalFiles"] = "filer i alt",
            ["Home.ThisMonthAdded"] = "+{0} denne måned",

            // Receipts
            ["Receipts.Subtitle"] = "Arkiveret i Google Drev",
            ["Receipts.Upload"] = "Upload",
            ["Receipts.ThisMonth"] = "Denne måned",
            ["Receipts.TotalReceipts"] = "Kvitteringer i alt",
            ["Receipts.TopCategory"] = "Topkategori",
            ["Receipts.SearchPlaceholder"] = "Søg efter butik eller forhandler...",
            ["Receipts.ConnectionErrorTitle"] = "Kunne ikke oprette forbindelse til Familie Hub API",
            ["Receipts.CheckServerUrl"] = "Tjek server-URL",
            ["Receipts.NoReceiptsTitle"] = "Ingen kvitteringer fundet",
            ["Receipts.EmptyStateNoReceipts"] = "Du har ikke uploadet nogen kvitteringer endnu. Tilføj din første købskvittering!",
            ["Receipts.EmptyStateNoMatch"] = "Ingen kvitteringer matcher dit søgefilter.",
            ["Receipts.UploadFirstReceipt"] = "Upload første kvittering",
            ["Receipts.ViewInDrive"] = "Åbn i Google Drev",
            ["Receipts.ViewReceipt"] = "Vis kvittering",
            ["Receipts.ArchiveReceiptTitle"] = "Arkiver kvittering",
            ["Receipts.Modal.Title"] = "Upload købskvittering",
            ["Receipts.Modal.FileLabel"] = "Kvitteringsdokument eller foto *",
            ["Receipts.Modal.SelectedFile"] = "Valgt: {0} ({1} KB)",
            ["Receipts.Modal.MerchantLabel"] = "Butik / Forhandler *",
            ["Receipts.Modal.MerchantPlaceholder"] = "f.eks. Bilka, Netto, Matas",
            ["Receipts.Modal.PurchaseDateLabel"] = "Købsdato",
            ["Receipts.Modal.NotesPlaceholder"] = "f.eks. Ugentlig familieindkøb",
            ["Receipts.Modal.Archiving"] = "Arkiverer...",
            ["Receipts.Modal.SaveAndArchive"] = "Gem og arkiver",
            ["Receipts.Validation.MerchantRequired"] = "Butik/forhandler er påkrævet.",
            ["Receipts.Validation.FileRequired"] = "Vælg venligst en kvitteringsfil eller et billede.",

            // Insurances
            ["Insurances.Subtitle"] = "Administration af policer & dækning",
            ["Insurances.AddPolicy"] = "Tilføj police",
            ["Insurances.TotalMonthlyCost"] = "Samlet månedlig pris",
            ["Insurances.AnnualEquivalent"] = "Årligt beløb",
            ["Insurances.ActivePolicies"] = "Aktive policer",
            ["Insurances.CostBreakdownByCurrency"] = "Prisfordeling pr. valuta:",
            ["Insurances.PerMonth"] = "/ md.",
            ["Insurances.PerYear"] = "/ år",
            ["Insurances.UpcomingRenewals"] = "Kommende policefornyelser",
            ["Insurances.InDays"] = "om {0} dage",
            ["Insurances.SearchPlaceholder"] = "Søg efter selskab, policenr. eller forsikrede...",
            ["Insurances.Loading"] = "Indlæser familiens forsikringer...",
            ["Insurances.EmptyTitle"] = "Ingen forsikringspolicer fundet",
            ["Insurances.EmptyFilterMatch"] = "Ingen policer matcher dine aktive søgefiltre. Prøv at rydde filtrene.",
            ["Insurances.EmptyDefault"] = "Hold styr på hus-, bil-, sundheds- og familieforsikringer, præmier og fornyelser.",
            ["Insurances.Card.Insured"] = "Forsikret: {0}",
            ["Insurances.Card.Monthly"] = "Månedlig",
            ["Insurances.Card.Premium"] = "Præmie ({0})",
            ["Insurances.Card.Deductible"] = "Selvrisiko:",
            ["Insurances.Card.ExpiredOn"] = "Udløbet ({0})",
            ["Insurances.Card.RenewsInDays"] = "Fornyes om {0} d. ({1})",
            ["Insurances.Card.RenewalDate"] = "Fornyelse: {0}",
            ["Insurances.Card.NoRenewalDate"] = "Ingen fornyelsesdato",
            ["Insurances.Card.EditTitle"] = "Rediger police",
            ["Insurances.Card.DeleteTitle"] = "Slet police",
            ["Insurances.Modal.EditTitle"] = "Rediger forsikringspolice",
            ["Insurances.Modal.AddTitle"] = "Tilføj forsikringspolice",
            ["Insurances.Modal.InsurerLabel"] = "Forsikringsselskab *",
            ["Insurances.Modal.InsurerPlaceholder"] = "f.eks. Topdanmark, Tryg, Alm. Brand",
            ["Insurances.Modal.PolicyNumberLabel"] = "Policenummer",
            ["Insurances.Modal.PolicyNumberPlaceholder"] = "f.eks. POL-98421",
            ["Insurances.Modal.CategoryLabel"] = "Kategori *",
            ["Insurances.Modal.InsuredPartyLabel"] = "Forsikret person / genstand *",
            ["Insurances.Modal.InsuredPartyPlaceholder"] = "f.eks. Hus & Indbo, Mathias, Tesla",
            ["Insurances.Modal.PremiumLabel"] = "Præmie *",
            ["Insurances.Modal.CurrencyLabel"] = "Valuta",
            ["Insurances.Modal.FrequencyLabel"] = "Betalingsfrekvens",
            ["Insurances.Modal.StartDateLabel"] = "Startdato",
            ["Insurances.Modal.RenewalDateLabel"] = "Fornyelsesdato (valgfrit)",
            ["Insurances.Modal.DeductibleLabel"] = "Selvrisiko (valgfrit)",
            ["Insurances.Modal.DeductiblePlaceholder"] = "f.eks. 2500",
            ["Insurances.Modal.NotesPlaceholder"] = "f.eks. Inkluderer vejhjælp og glasdækning",
            ["Insurances.Modal.UpdatePolicy"] = "Opdater police",
            ["Insurances.Modal.SavePolicy"] = "Gem police",
            ["Insurances.Validation.InsurerRequired"] = "Forsikringsselskab er påkrævet.",
            ["Insurances.Validation.InsuredPartyRequired"] = "Forsikret person eller genstand er påkrævet.",
            ["Insurances.Validation.PremiumNegative"] = "Præmiebeløbet kan ikke være negativt.",

            // Children
            ["Children.Subtitle"] = "Børns udvikling & milepæle",
            ["Children.AddChild"] = "Tilføj barn",
            ["Children.Loading"] = "Indlæser børneoverblik...",
            ["Children.WelcomeTitle"] = "Velkommen til Børneoverblikket",
            ["Children.WelcomeDescription"] = "Følg dit barns alder, vækst, sko- og tøjstørrelser, minder og udviklingsmål.",
            ["Children.RegisterChildButton"] = "Registrer dit barn",
            ["Children.AddAnother"] = "+ Tilføj barn",
            ["Children.Tab.Growth"] = "Vækst & mål",
            ["Children.Tab.Roadmap"] = "Udviklingsplan (Milepæle)",
            ["Children.Tab.Timeline"] = "Minder & præstationer",
            ["Children.Modal.RegisterTitle"] = "Registrer børneprofil",
            ["Children.Modal.FirstName"] = "Fornavn",
            ["Children.Modal.FirstNamePlaceholder"] = "f.eks. Oliver",
            ["Children.Modal.LastName"] = "Efternavn",
            ["Children.Modal.DateOfBirth"] = "Fødselsdato",
            ["Children.Modal.Gender"] = "Køn",
            ["Children.Modal.InitialSizes"] = "Nuværende størrelser (valgfrit)",
            ["Children.Modal.ClothesSize"] = "Tøjstørrelse",
            ["Children.Modal.ClothesSizePlaceholder"] = "f.eks. 86",
            ["Children.Modal.ShoeSize"] = "Skostørrelse",
            ["Children.Modal.ShoeSizePlaceholder"] = "f.eks. 22",
            ["Children.Modal.HatSize"] = "Hattestørrelse",
            ["Children.Modal.HatSizePlaceholder"] = "f.eks. 50 cm",
            ["Children.Modal.DiaperSize"] = "Blestørrelse",
            ["Children.Modal.DiaperSizePlaceholder"] = "f.eks. Str. 4 eller bukseble",
            ["Children.Modal.RegisterSubmit"] = "Registrer",
            ["Children.Header.Born"] = "Født {0}",
            ["Children.Header.CurrentSizes"] = "Nuværende størrelser",
            ["Children.Header.Clothes"] = "Tøj:",
            ["Children.Header.Shoes"] = "Sko:",
            ["Children.Header.Hat"] = "Hat:",
            ["Children.Header.EditSizesTitle"] = "Rediger størrelser",
            ["Children.Header.EditWardrobeTitle"] = "Rediger garderobestørrelser",
            ["Children.Header.ClothesPlaceholderLong"] = "f.eks. 86 / 92",
            ["Children.Header.ShoesPlaceholderLong"] = "f.eks. 23 EU",

            // Growth Tab
            ["Growth.Height"] = "Højde",
            ["Growth.Weight"] = "Vægt",
            ["Growth.HeadCircumference"] = "Hovedomfang",
            ["Growth.HeadCircShort"] = "Hovedomf.",
            ["Growth.RecordedOn"] = "Målt d. {0}",
            ["Growth.NoRecordsYet"] = "Ingen målinger endnu",
            ["Growth.LogTitle"] = "Vækstlog",
            ["Growth.LogSubtitle"] = "Historiske registreringer af højde og vægt",
            ["Growth.RecordMeasurement"] = "Registrer måling",
            ["Growth.Loading"] = "Indlæser målinger...",
            ["Growth.EmptyTitle"] = "Ingen vækstmålinger registreret endnu",
            ["Growth.EmptySubtitle"] = "Start med at registrere dit barns nuværende højde, vægt eller hovedomfang.",
            ["Growth.Modal.Title"] = "Registrer vækstmåling",
            ["Growth.Mode.All"] = "Alle / Valgfri",
            ["Growth.Mode.WeightOnly"] = "Kun vægt",
            ["Growth.Mode.HeightOnly"] = "Kun højde",
            ["Growth.Mode.HeadOnly"] = "Kun hovedomf.",
            ["Growth.HeightCmLabel"] = "Højde (cm)",
            ["Growth.WeightKgLabel"] = "Vægt (kg)",
            ["Growth.HeadCmOptionalLabel"] = "Hovedomfang (cm, valgfrit)",
            ["Growth.HeadCmLabel"] = "Hovedomfang (cm) *",
            ["Growth.NotesPlaceholder"] = "f.eks. 1-års undersøgelse hos lægen",
            ["Growth.RecordButton"] = "Registrer",
            ["Growth.Validation.AtLeastOne"] = "Indtast venligst mindst én måling (vægt, højde eller hovedomfang).",

            // Milestones Roadmap & Timeline
            ["Roadmap.Title"] = "Udviklingsplan",
            ["Roadmap.Subtitle"] = "Forventede milepæle efter aldersvindue (f.eks. \"ved 12 måneder forventes de første skridt\")",
            ["Roadmap.LoadGuidelines"] = "Indlæs standardmilepæle",
            ["Roadmap.AddTarget"] = "Tilføj mål",
            ["Roadmap.Loading"] = "Indlæser udviklingsplan...",
            ["Roadmap.EmptyTitle"] = "Ingen udviklingsmål konfigureret",
            ["Roadmap.EmptySubtitle"] = "Indlæs Sundhedsstyrelsens / WHO's standardmilepæle eller opret dine egne mål.",
            ["Roadmap.CompletedCount"] = "{0} / {1} gennemført",
            ["Roadmap.TargetAge"] = "Mål: ~{0}",
            ["Roadmap.UpToAge"] = "(op til {0})",
            ["Roadmap.AchievedOn"] = "• Opnået d. {0}",
            ["Roadmap.Done"] = "Opnået",
            ["Roadmap.MarkDone"] = "Marker som nået",
            ["Roadmap.Modal.AchieveTitle"] = "Marker milepæl som opnået",
            ["Roadmap.Modal.DateAchieved"] = "Dato opnået",
            ["Roadmap.Modal.MemoryNote"] = "Minde / note (valgfrit)",
            ["Roadmap.Modal.MemoryPlaceholder"] = "f.eks. Gik 5 skridt tværs over stuegulvet!",
            ["Roadmap.Modal.CompleteButton"] = "Fuldfør",
            ["Roadmap.Modal.AddTitle"] = "Tilføj udviklingsmål",
            ["Roadmap.Modal.MilestoneTitle"] = "Milepælens titel",
            ["Roadmap.Modal.MilestonePlaceholder"] = "f.eks. Hoppe på ét ben eller tælle til 10",
            ["Roadmap.Modal.ExpectedAgeMonths"] = "Forventet alder (måneder)",
            ["Roadmap.Modal.GuidelineDetails"] = "Beskrivelse (valgfrit)",
            ["Roadmap.Modal.GuidelinePlaceholder"] = "f.eks. Kan gå gennem rummet uden støtte",
            ["Roadmap.Bracket.0_3"] = "0 – 3 måneder (Spædbarn)",
            ["Roadmap.Bracket.4_6"] = "4 – 6 måneder (Trille & pludre)",
            ["Roadmap.Bracket.7_9"] = "7 – 9 måneder (Sidde & kravle)",
            ["Roadmap.Bracket.10_12"] = "10 – 12 måneder (Første skridt & ord)",
            ["Roadmap.Bracket.13_18"] = "13 – 18 måneder (Tumling)",
            ["Roadmap.Bracket.19_24"] = "19 – 24 måneder (Små sætninger)",
            ["Roadmap.Bracket.24_36"] = "2 – 3 år (Leg & løb)",
            ["Roadmap.Bracket.36_48"] = "3 – 4 år (Børnehavefærdigheder)",
            ["Roadmap.Bracket.48Plus"] = "5+ år (Skoleklar)",

            ["Timeline.Title"] = "Minder & præstationer",
            ["Timeline.Subtitle"] = "Dyrebare øjeblikke og udviklingsmæssige milepæle",
            ["Timeline.AddMemory"] = "Tilføj minde / milepæl",
            ["Timeline.Loading"] = "Indlæser minder...",
            ["Timeline.EmptyTitle"] = "Ingen minder registreret endnu",
            ["Timeline.EmptySubtitle"] = "Fejr dit barns første smil, første skridt, tabte mælketand eller sjove øjeblikke.",
            ["Timeline.RecordFirstMemory"] = "Registrer første minde",
            ["Timeline.AtAge"] = "ved {0}",
            ["Timeline.Modal.Title"] = "Registrer opnået milepæl / minde",
            ["Timeline.Modal.TitleLabel"] = "Titel",
            ["Timeline.Modal.TitlePlaceholder"] = "f.eks. Gik selv for første gang eller tabte første tand",
            ["Timeline.Modal.StoryLabel"] = "Minder / historie (valgfrit)",
            ["Timeline.Modal.StoryPlaceholder"] = "Hvor skete det? Var der en sjov reaktion?",
            ["Timeline.Modal.SaveMemory"] = "Gem minde",

            // Vehicles
            ["Common.SaveChanges"] = "Gem ændringer",
            ["Vehicles.Subtitle"] = "Bilpark & kilometerlog",
            ["Vehicles.AddVehicle"] = "Tilføj bil",
            ["Vehicles.Loading"] = "Indlæser biloverblik...",
            ["Vehicles.WelcomeTitle"] = "Velkommen til Biloverblikket",
            ["Vehicles.WelcomeDescription"] = "Hold familiens biler i topform: følg kilometerstand, bilsyn, serviceintervaller og værkstedsregninger.",
            ["Vehicles.WelcomeSubtitle"] = "Hold familiens biler i topform: følg kilometerstand, bilsyn, serviceintervaller og værkstedsregninger.",
            ["Vehicles.RegisterFirstVehicle"] = "Registrer din første bil",
            ["Vehicles.RegisterFirst"] = "Registrer din første bil",
            ["Vehicles.AddAnother"] = "+ Tilføj bil",
            ["Vehicles.Tab.Mileage"] = "Overblik & km-stand",
            ["Vehicles.TabMileage"] = "Overblik & km-stand",
            ["Vehicles.Tab.Services"] = "Servicehistorik",
            ["Vehicles.TabService"] = "Servicehistorik",
            ["Vehicles.Tab.Settings"] = "Plan & specifikationer",
            ["Vehicles.TabSettings"] = "Plan & specifikationer",
            ["Vehicles.Settings.Title"] = "Rediger serviceplan & specifikationer",
            ["Vehicles.EditSpecsTitle"] = "Rediger serviceplan & specifikationer",
            ["Vehicles.Field.Make"] = "Mærke",
            ["Vehicles.Make"] = "Mærke",
            ["Vehicles.MakeRequired"] = "Mærke *",
            ["Vehicles.MakePlaceholder"] = "f.eks. Volkswagen, Skoda, Tesla",
            ["Vehicles.Field.Model"] = "Model",
            ["Vehicles.Model"] = "Model",
            ["Vehicles.ModelRequired"] = "Model *",
            ["Vehicles.ModelPlaceholder"] = "f.eks. ID.4, Octavia, Model Y",
            ["Vehicles.Field.Year"] = "Årgang",
            ["Vehicles.Year"] = "Årgang",
            ["Vehicles.YearRequired"] = "Årgang *",
            ["Vehicles.Field.LicensePlate"] = "Registreringsnummer",
            ["Vehicles.LicensePlate"] = "Registreringsnummer",
            ["Vehicles.LicensePlateRequired"] = "Registreringsnummer *",
            ["Vehicles.LicensePlatePlaceholder"] = "f.eks. AB 12 345",
            ["Vehicles.Field.FuelType"] = "Brændstoftype",
            ["Vehicles.FuelType"] = "Brændstoftype",
            ["Vehicles.FuelTypeRequired"] = "Brændstoftype *",
            ["Vehicles.Field.InitialMileage"] = "Start-kilometerstand (km)",
            ["Vehicles.InitialMileageKm"] = "Start-kilometerstand (km)",
            ["Vehicles.InitialMileagePlaceholder"] = "f.eks. 45000",
            ["Vehicles.Field.ServiceIntervalKm"] = "Serviceinterval (km)",
            ["Vehicles.ServiceIntervalKm"] = "Serviceinterval (km)",
            ["Vehicles.ServiceIntervalKmPlaceholder"] = "f.eks. 15000",
            ["Vehicles.Field.ServiceIntervalMonths"] = "Serviceinterval (måneder)",
            ["Vehicles.ServiceIntervalMonths"] = "Serviceinterval (måneder)",
            ["Vehicles.ServiceIntervalMonthsPlaceholder"] = "f.eks. 12",
            ["Vehicles.Field.NextInspectionDate"] = "Næste syn (Bilsyn)",
            ["Vehicles.NextInspectionDate"] = "Næste syn (Bilsyn)",
            ["Vehicles.Field.Vin"] = "Stelnummer / VIN (valgfrit)",
            ["Vehicles.VinOptional"] = "Stelnummer / VIN (valgfrit)",
            ["Vehicles.Field.VinPlaceholder"] = "17-tegns stelnummer",
            ["Vehicles.VinPlaceholder"] = "17-tegns stelnummer",
            ["Vehicles.DeleteVehicle"] = "Slet bil",
            ["Vehicles.SaveChanges"] = "Gem ændringer",
            ["Vehicles.Modal.RegisterTitle"] = "Registrer ny bil",
            ["Vehicles.RegisterNewTitle"] = "Registrer ny bil",
            ["Vehicles.Modal.Registering"] = "Registrerer...",
            ["Vehicles.Registering"] = "Registrerer...",
            ["Vehicles.Modal.RegisterSubmit"] = "Registrer bil",
            ["Vehicles.RegisterButton"] = "Registrer bil",
            ["Vehicles.Validation.RequiredFields"] = "Mærke, model og registreringsnummer er påkrævet.",
            ["Vehicles.RequiredFieldsError"] = "Mærke, model og registreringsnummer er påkrævet.",
            ["Vehicles.Validation.CreateFailed"] = "Kunne ikke oprette bilen.",
            ["Vehicles.CreateError"] = "Kunne ikke oprette bilen.",
            ["Vehicles.Validation.UpdateFailed"] = "Kunne ikke opdatere bilens oplysninger.",
            ["Vehicles.UpdateError"] = "Kunne ikke opdatere bilens oplysninger.",

            // Vehicle Header Card
            ["Vehicles.Header.UpToDate"] = "Opdateret",
            ["Vehicles.UpToDate"] = "Opdateret",
            ["Vehicles.VinLabel"] = "VIN:",
            ["Vehicles.Header.Odometer"] = "Kilometerstand",
            ["Vehicles.Odometer"] = "Kilometerstand",
            ["Vehicles.Header.LogsRecorded"] = "{0} aflæsninger registreret",
            ["Vehicles.LogsRecordedFormat"] = "{0} aflæsninger registreret",
            ["Vehicles.Header.NextService"] = "Næste service",
            ["Vehicles.NextService"] = "Næste service",
            ["Vehicles.Header.KmOverdue"] = "+{0} km overskredet",
            ["Vehicles.KmOverdueFormat"] = "+{0} km overskredet",
            ["Vehicles.Header.KmLeft"] = "km tilbage",
            ["Vehicles.KmLeft"] = "km tilbage",
            ["Vehicles.Header.ScheduleBasedOnKm"] = "Baseret på kilometerstand",
            ["Vehicles.ScheduleBasedOnKm"] = "Baseret på kilometerstand",
            ["Vehicles.Header.NotConfigured"] = "Ikke angivet",
            ["Vehicles.NotConfigured"] = "Ikke angivet",
            ["Vehicles.Header.NoIntervalSet"] = "Intet interval sat",
            ["Vehicles.NoIntervalSet"] = "Intet interval sat",
            ["Vehicles.Header.InspectionTitle"] = "Bilsyn",
            ["Vehicles.InspectionTitle"] = "Bilsyn",
            ["Vehicles.Header.NoInspectionDate"] = "Ingen synsdato",
            ["Vehicles.NoInspectionDate"] = "Ingen synsdato",
            ["Vehicles.Header.TotalMaintenance"] = "Samlet registreret vedligeholdelse:",
            ["Vehicles.TotalMaintenanceLabel"] = "Samlet registreret vedligeholdelse:",
            ["Vehicles.Header.RecordsCount"] = "{0} registreringer",
            ["Vehicles.RecordsCountFormat"] = "{0} registreringer",
            ["Vehicles.Header.LogMileage"] = "Registrer km",
            ["Vehicles.LogMileage"] = "Registrer km",
            ["Vehicles.Header.RecordService"] = "Registrer service",
            ["Vehicles.RecordService"] = "Registrer service",
            ["Vehicles.Countdown.DaysOverdue"] = "{0} dage overskredet",
            ["Vehicles.DaysOverdueFormat"] = "{0} dage overskredet",
            ["Vehicles.Countdown.DueToday"] = "Forfalder i dag",
            ["Vehicles.DueToday"] = "Forfalder i dag",
            ["Vehicles.Countdown.DueTomorrow"] = "Forfalder i morgen",
            ["Vehicles.DueTomorrow"] = "Forfalder i morgen",
            ["Vehicles.Countdown.InDays"] = "Om {0} dage",
            ["Vehicles.InDaysFormat"] = "Om {0} dage",
            ["Vehicles.Alert.ServiceOverdue"] = "Service overskredet",
            ["Vehicles.AlertServiceOverdue"] = "Service overskredet",
            ["Vehicles.Alert.ServiceDue"] = "Service snart",
            ["Vehicles.AlertServiceDue"] = "Service snart",
            ["Vehicles.Alert.InspectionOverdue"] = "Bilsyn overskredet",
            ["Vehicles.AlertInspectionOverdue"] = "Bilsyn overskredet",
            ["Vehicles.Alert.InspectionDue"] = "Bilsyn snart",
            ["Vehicles.AlertInspectionDue"] = "Bilsyn snart",

            // Mileage Tab
            ["Mileage.Title"] = "Kilometerhistorik",
            ["Mileage.Subtitle"] = "Registrer kørte kilometer for at overvåge serviceintervaller og kørselsforbrug",
            ["Mileage.LogButton"] = "Registrer km-stand",
            ["Mileage.Loading"] = "Indlæser kilometerdata...",
            ["Mileage.EmptyTitle"] = "Ingen kilometerstand registreret endnu",
            ["Mileage.EmptySubtitle"] = "Indtast din bils kilometertæller for at følge forbruget og være på forkant med vedligeholdelse.",
            ["Mileage.LogFirstReading"] = "Registrer første aflæsning",
            ["Mileage.LogFirst"] = "Registrer første aflæsning",
            ["Mileage.TableDate"] = "Dato",
            ["Mileage.Table.Reading"] = "Aflæsning",
            ["Mileage.TableReading"] = "Aflæsning",
            ["Mileage.Table.Delta"] = "Forskel",
            ["Mileage.TableDelta"] = "Forskel",
            ["Mileage.TableNotes"] = "Noter",
            ["Mileage.TableActions"] = "Handlinger",
            ["Mileage.Table.Baseline"] = "Startværdi",
            ["Mileage.Baseline"] = "Startværdi",
            ["Mileage.DeleteTitle"] = "Slet aflæsning",
            ["Mileage.DeleteEntry"] = "Slet aflæsning",
            ["Mileage.Modal.Title"] = "Registrer kilometerstand",
            ["Mileage.ModalTitle"] = "Registrer kilometerstand",
            ["Mileage.Modal.ReadingLabel"] = "Kilometertæller (km)",
            ["Mileage.OdometerReadingKm"] = "Kilometertæller (km)",
            ["Mileage.OdometerPlaceholder"] = "f.eks. 125000",
            ["Mileage.Modal.CurrentOdometer"] = "Nuværende kilometerstand: {0} km",
            ["Mileage.CurrentOdometerFormat"] = "Nuværende kilometerstand: {0} km",
            ["Mileage.NotesOptional"] = "Noter (valgfrit)",
            ["Mileage.Modal.NotesPlaceholder"] = "f.eks. Hjemkomst fra bilferie, pendlertjek",
            ["Mileage.NotesPlaceholder"] = "f.eks. Hjemkomst fra bilferie, pendlertjek",
            ["Mileage.Modal.SaveEntry"] = "Gem aflæsning",
            ["Mileage.SaveEntry"] = "Gem aflæsning",
            ["Mileage.Validation.Negative"] = "Kilometerstand kan ikke være negativ.",
            ["Mileage.NegativeError"] = "Kilometerstand kan ikke være negativ.",
            ["Mileage.Validation.SaveFailed"] = "Kunne ikke gemme kilometerstand.",
            ["Mileage.SaveError"] = "Kunne ikke gemme kilometerstand.",

            // Service History Tab
            ["Service.Title"] = "Service- & vedligeholdelseslog",
            ["Service.Subtitle"] = "Hold styr på olieskift, bilsyn, hjulskifte og værkstedsfakturaer",
            ["Service.RecordButton"] = "Registrer service",
            ["Service.Loading"] = "Indlæser vedligeholdelsesdata...",
            ["Service.EmptyTitle"] = "Ingen servicehistorik registreret",
            ["Service.EmptySubtitle"] = "Hold en komplet log over alle værkstedsbesøg, udskiftede dele og omkostninger.",
            ["Service.RecordFirstService"] = "Registrer første service",
            ["Service.RecordFirst"] = "Registrer første service",
            ["Service.ReceiptBadge"] = "Kvittering",
            ["Service.Receipt"] = "Kvittering",
            ["Service.ViewInReceiptsTitle"] = "Vis under Kvitteringer",
            ["Service.ViewInReceipts"] = "Vis under Kvitteringer",
            ["Service.DeleteTitle"] = "Slet registrering",
            ["Service.DeleteRecord"] = "Slet registrering",
            ["Service.Modal.Title"] = "Registrer servicehændelse",
            ["Service.ModalTitle"] = "Registrer servicehændelse",
            ["Service.Modal.ServiceType"] = "Servicetype",
            ["Service.ServiceType"] = "Servicetype",
            ["Service.Modal.ServiceDate"] = "Servicedato",
            ["Service.ServiceDate"] = "Servicedato",
            ["Service.Modal.TitleLabel"] = "Titel / Beskrivelse",
            ["Service.TitleDescription"] = "Titel / Beskrivelse",
            ["Service.Modal.TitlePlaceholder"] = "f.eks. Stort 60.000 km service, olie + filtre, sommerdæk",
            ["Service.TitlePlaceholder"] = "f.eks. Stort 60.000 km service, olie + filtre, sommerdæk",
            ["Service.Modal.OdometerLabel"] = "Kilometerstand (km)",
            ["Service.OdometerKm"] = "Kilometerstand (km)",
            ["Service.OdometerPlaceholder"] = "f.eks. 60500",
            ["Service.Modal.CostLabel"] = "Pris (kr. / DKK)",
            ["Service.CostDkk"] = "Pris (kr. / DKK)",
            ["Service.CostPlaceholder"] = "f.eks. 3450",
            ["Service.Modal.WorkshopLabel"] = "Værksted / Udbyder (valgfrit)",
            ["Service.WorkshopOptional"] = "Værksted / Udbyder (valgfrit)",
            ["Service.Modal.WorkshopPlaceholder"] = "f.eks. QuickPoint, Semler Biler, Skorstensgaard",
            ["Service.WorkshopPlaceholder"] = "f.eks. QuickPoint, Semler Biler, Skorstensgaard",
            ["Service.Modal.LinkReceiptLabel"] = "Tilknyt kvittering (valgfrit)",
            ["Service.LinkToReceiptOptional"] = "Tilknyt kvittering (valgfrit)",
            ["Service.Modal.NoLinkedReceipt"] = "Ingen tilknyttet kvittering",
            ["Service.NoLinkedReceipt"] = "Ingen tilknyttet kvittering",
            ["Service.Modal.NotesLabel"] = "Noter / Detaljer (valgfrit)",
            ["Service.NotesOptional"] = "Noter / Detaljer (valgfrit)",
            ["Service.Modal.NotesPlaceholder"] = "Udskiftet tændrør, syntetisk 5W-30 olie, pollenfilter...",
            ["Service.NotesPlaceholder"] = "Udskiftet tændrør, syntetisk 5W-30 olie, pollenfilter...",
            ["Service.Modal.SaveRecord"] = "Gem registrering",
            ["Service.SaveRecord"] = "Gem registrering",
            ["Service.Validation.TitleRequired"] = "Servicetitel er påkrævet.",
            ["Service.TitleRequiredError"] = "Servicetitel er påkrævet.",
            ["Service.Validation.MileageNegative"] = "Kilometerstand kan ikke være negativ.",
            ["Service.Validation.SaveFailed"] = "Kunne ikke gemme serviceregistrering.",
            ["Service.SaveError"] = "Kunne ikke gemme serviceregistrering."
        };

    private static readonly IReadOnlyDictionary<string, string> LegacyMilestoneTextMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Smiles at people"] = "Smiler til mennesker",
            ["Can briefly calm himself, looks at parents"] = "Kan kortvarigt berolige sig selv og søger øjenkontakt med forældre",
            ["Coos, makes gurgling sounds"] = "Pludrer og laver kurrende lyde",
            ["Turns head toward sounds"] = "Drejer hovedet efter lyde",
            ["Holds head up when on tummy"] = "Løfter hovedet når det ligger på maven",
            ["Pushes up when lying on tummy"] = "Skubber sig op med armene i maveleje",
            ["Chuckles and laughs"] = "Klukker og griner højt",
            ["Begins to babble with expression"] = "Begynder at pludre med tonefald og udtryk",
            ["Reaches for toy with one hand"] = "Rækker ud efter legetøj med én hånd",
            ["Uses hands and eyes together"] = "Koordinerer hænder og øjne",
            ["Rolls over from tummy to back"] = "Triller fra mave til ryg",
            ["Can roll from front to back"] = "Kan vende sig fra mave til ryg",
            ["First baby tooth emerges"] = "Første mælketand bryder frem",
            ["Usually lower central incisors"] = "Som regel de nederste fortænder",
            ["Sits without support"] = "Sidder uden støtte",
            ["Sits steadily with hands free to play"] = "Sidder stabilt med hænderne fri til at lege",
            ["Passes things from one hand to another"] = "Flytter ting fra den ene hånd til den anden",
            ["Explores objects by putting in mouth"] = "Undersøger ting ved at putte dem i munden",
            ["Responds to own name"] = "Reagerer på eget navn",
            ["Turns head when name is called"] = "Kigger når navnet bliver kaldt",
            ["Crawling on hands and knees"] = "Kravler på hænder og knæ",
            ["Gets into sitting position and crawls"] = "Kan selv sætte sig op og kravle",
            ["Pulls up to stand"] = "Rejser sig op ved møbler",
            ["Pulls up on furniture"] = "Trækker sig op at stå ved møbler",
            ["Pincer grasp (thumb and index finger)"] = "Pincetgreb (tommel- og pegefinger)",
            ["Picks up small pieces of food"] = "Samler små stykker mad op med fingrene",
            ["Plays peek-a-boo"] = "Leger titte-bøh",
            ["Has favorite toys and looks for hidden items"] = "Har yndlingslegetøj og leder efter gemte ting",
            ["First independent steps / Walking"] = "Første selvstændige skridt / Går selv",
            ["Takes a few steps without holding on"] = "Tager enkelte skridt uden at holde fast",
            ["Says first clear word (e.g. Mama, Dada)"] = "Siger sit første tydelige ord (f.eks. Mor, Far)",
            ["Uses simple gestures like waving bye-bye"] = "Bruger enkle fagter som at vinke farvel",
            ["Drinks from an open cup / sippy cup"] = "Drikker af åben kop eller tudkop",
            ["Helps when being dressed"] = "Hjælper til når det får tøj på",
            ["Says 10 or more words"] = "Siger 10 eller flere ord",
            ["Points to show someone what he wants"] = "Peger for at vise hvad det gerne vil have",
            ["Walks up steps and runs"] = "Går op ad trapper og løber",
            ["Pulls toys while walking"] = "Trækker legetøj efter sig mens det går",
            ["Eats with a spoon"] = "Spiser selv med ske",
            ["Can scribble on paper"] = "Kan tegne kruseduller på papir",
            ["Says 2-4 word sentences"] = "Siger sætninger på 2–4 ord",
            ["Repeats words overheard in conversations"] = "Gentager ord det hører i samtaler",
            ["Kicks a ball forward"] = "Sparker en bold fremad",
            ["Jumps with both feet off the ground"] = "Hopper med begge fødder fri af gulvet",
            ["Builds tower of 4+ blocks"] = "Bygger tårn af 4+ klodser",
            ["Sorts shapes and colors"] = "Sorterer former og farver",
            ["Daytime potty trained"] = "Ble-fri om dagen",
            ["Stays dry during day, uses toilet with help"] = "Holder sig tør i dagtimerne og bruger potte/toilet med hjælp",
            ["Rides a tricycle"] = "Kører på trehjulet cykel",
            ["Pedals tricycle with good balance"] = "Træder i pedalerne med god balance",
            ["Carries on conversation of 2-3 sentences"] = "Fører små samtaler på 2–3 sætninger",
            ["Speaks clearly enough for strangers to understand"] = "Taler tydeligt nok til at fremmede kan forstå det",
            ["Loses first baby tooth"] = "Taber sin første mælketand",
            ["Primary central incisor naturally falls out"] = "De første mælkefortænder falder naturligt ud"
        };

    public CultureInfo Culture => DanishCulture;

    public string this[string key] =>
        Translations.TryGetValue(key, out var value) ? value : key;

    public string Format(string key, params object[] args) =>
        string.Format(DanishCulture, this[key], args);

    public string Category(string? category) => (category?.Trim() ?? string.Empty) switch
    {
        "Groceries" => "Dagligvarer",
        "Dining" => "Restaurant & Café",
        "Utilities" => "Forbrug & Forsyning",
        "Entertainment" => "Underholdning",
        "Health" => "Sundhed",
        "Shopping" => "Shopping",
        "Home & Garden" => "Hus & Have",
        "Kids & Toys" => "Børn & Legetøj",
        "Other" => "Andet",
        "None" => "Ingen",
        var c => c
    };

    public string InsuranceCategory(string? category) => (category?.Trim() ?? string.Empty) switch
    {
        "Home" => "Hus & Indbo",
        "Health" => "Sundhed",
        "Auto" => "Bil",
        "Life" => "Livsforsikring",
        "Travel" => "Rejse",
        "Liability" => "Ansvar",
        "Pet" => "Kæledyr",
        "Dental" => "Tandlæge",
        "Disability" => "Ulykke & Invaliditet",
        "Other" => "Andet",
        var c => c
    };

    public string Frequency(string? frequency) => (frequency?.Trim() ?? string.Empty) switch
    {
        "Monthly" => "Månedlig",
        "Quarterly" => "Kvartalsvis",
        "SemiAnnually" or "Semi-Annually" => "Halvårlig",
        "Annually" => "Årlig",
        var f => f
    };

    public string PolicyStatus(string? status) => (status?.Trim() ?? string.Empty) switch
    {
        "Active" => "Aktiv",
        "PendingRenewal" => "Afventer fornyelse",
        "Expired" => "Udløbet",
        "Cancelled" => "Opsagt",
        var s => s
    };

    public string MilestoneCategory(string? category) => (category?.Trim() ?? string.Empty) switch
    {
        "GrossMotor" => "Grovmotorik",
        "FineMotor" => "Finmotorik",
        "LanguageSpeech" => "Sprog & Tale",
        "SocialEmotional" => "Social & Følelsesmæssig",
        "Cognitive" => "Kognitiv",
        "HealthSelfCare" => "Sundhed & Selvhjælp",
        "Dental" => "Tænder",
        "Other" => "Andet",
        var c => c
    };

    public string MilestoneStatus(string? status) => (status?.Trim() ?? string.Empty) switch
    {
        "Expected" => "Forventet",
        "Achieved" => "Opnået",
        var s => s
    };

    public string ServiceType(string? serviceType) => (serviceType?.Trim() ?? string.Empty) switch
    {
        "RegularService" => "Almindeligt service",
        "OilChange" => "Olieskift",
        "TireChange" => "Hjulskifte",
        "BrakeService" => "Bremseservice",
        "Inspection" or "InspectionSyn" => "Syn",
        "Repair" => "Reparation",
        "BatteryReplacement" => "Batteriskift",
        "Other" => "Andet",
        var s => s
    };

    public string FuelType(string? fuelType) => (fuelType?.Trim() ?? string.Empty) switch
    {
        "Petrol" or "Gasoline" => "Benzin",
        "Diesel" => "Diesel",
        "Electric" => "El",
        "Hybrid" => "Hybrid",
        "PlugInHybrid" or "Plug-in Hybrid" => "Plug-in Hybrid",
        "Other" => "Andet",
        var f => f
    };

    public string Gender(string? gender) => (gender?.Trim() ?? string.Empty) switch
    {
        "Boy" => "Dreng",
        "Girl" => "Pige",
        "Other" => "Andet",
        var g => g
    };

    public string FormatDate(DateTime date, string format = "d. MMM yyyy") =>
        date.ToString(format, DanishCulture);

    public string FormatAge(int years, int months, int days)
    {
        if (years >= 1)
        {
            return months > 0 ? $"{years} år {months} mdr." : $"{years} år";
        }

        if (months >= 1)
        {
            return days > 0 ? $"{months} mdr. {days} d." : $"{months} mdr.";
        }

        return $"{days} d.";
    }

    public string FormatAgeMonths(int months)
    {
        if (months >= 24 && months % 12 == 0)
        {
            return $"{months / 12} år";
        }

        if (months >= 24)
        {
            return $"{months / 12} år {months % 12} mdr.";
        }

        return $"{months} mdr.";
    }

    public string FormatAgeWindow(int minMonths, int? maxMonths)
    {
        if (maxMonths.HasValue && maxMonths.Value > minMonths)
        {
            return $"{FormatAgeMonths(minMonths)} – {FormatAgeMonths(maxMonths.Value)}";
        }

        return FormatAgeMonths(minMonths);
    }

    public string MilestoneText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return LegacyMilestoneTextMap.TryGetValue(text.Trim(), out var translated)
            ? translated
            : text;
    }
}
