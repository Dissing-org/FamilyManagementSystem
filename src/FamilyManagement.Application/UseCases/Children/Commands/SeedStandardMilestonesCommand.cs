using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

public record SeedStandardMilestonesCommand(Guid ChildId);

public class SeedStandardMilestonesCommandHandler
{
    private readonly IChildMilestoneRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    public SeedStandardMilestonesCommandHandler(
        IChildMilestoneRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

    public async Task<int> HandleAsync(
        SeedStandardMilestonesCommand command,
        CancellationToken cancellationToken = default)
    {
        var child = await _childRepository.GetByIdAsync(ChildId.From(command.ChildId), cancellationToken);
        if (child is null)
        {
            throw new KeyNotFoundException($"Child with ID '{command.ChildId}' was not found.");
        }

        var existing = await _repository.GetByChildIdAsync(child.Id, cancellationToken: cancellationToken);
        var existingTitles = existing.Select(e => e.Title).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var guidelines = GetStandardGuidelines(child.Id);
        var toAdd = guidelines.Where(g => !existingTitles.Contains(g.Title)).ToList();

        if (toAdd.Count > 0)
        {
            await _repository.AddRangeAsync(toAdd, cancellationToken);
        }

        return toAdd.Count;
    }

    private static List<ChildMilestone> GetStandardGuidelines(ChildId childId) => new()
    {
        ChildMilestone.CreateExpected(childId, "Smiler til mennesker", MilestoneCategory.SocialEmotional, 2, 3, "Kan kortvarigt berolige sig selv og kigger på forældre", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Kurrer og laver pludrelyde", MilestoneCategory.LanguageSpeech, 2, 3, "Drejer hovedet mod lyde", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Løfter hovedet på maven", MilestoneCategory.GrossMotor, 2, 3, "Skubber sig op, når barnet ligger på maven", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Klukker og griner", MilestoneCategory.LanguageSpeech, 4, 5, "Begynder at pludre med udtryk", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Rækker ud efter legetøj med én hånd", MilestoneCategory.FineMotor, 4, 6, "Bruger hænder og øjne sammen", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Triller fra mave til ryg", MilestoneCategory.GrossMotor, 4, 6, "Kan trille fra mave til ryg", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Første mælketand bryder frem", MilestoneCategory.Dental, 6, 10, "Typisk de nederste fortænder i midten", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Sidder uden støtte", MilestoneCategory.GrossMotor, 6, 8, "Sidder stabilt med hænderne fri til at lege", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Flytter ting fra den ene hånd til den anden", MilestoneCategory.FineMotor, 6, 8, "Undersøger ting ved at putte dem i munden", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Reagerer på eget navn", MilestoneCategory.LanguageSpeech, 6, 7, "Drejer hovedet, når navnet bliver kaldt", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Kravler på hænder og knæ", MilestoneCategory.GrossMotor, 9, 11, "Sætter sig selv op og kravler", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Trækker sig op at stå", MilestoneCategory.GrossMotor, 9, 11, "Trækker sig op ved møbler", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Pincetgreb (tommel- og pegefinger)", MilestoneCategory.FineMotor, 9, 12, "Samler små stykker mad op", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Leger borte-tit-tit", MilestoneCategory.SocialEmotional, 9, 10, "Har yndlingslegetøj og leder efter gemte ting", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Første selvstændige skridt / Går selv", MilestoneCategory.GrossMotor, 12, 15, "Tager et par skridt uden at holde fast", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Siger første tydelige ord (f.eks. Mor, Far)", MilestoneCategory.LanguageSpeech, 12, 14, "Bruger enkle fagter som at vinke farvel", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Drikker af åben kop / tudkop", MilestoneCategory.HealthSelfCare, 12, 14, "Hjælper til, når barnet får tøj på", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Siger 10 eller flere ord", MilestoneCategory.LanguageSpeech, 18, 20, "Peger for at vise, hvad barnet vil have", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Går op ad trapper og løber", MilestoneCategory.GrossMotor, 18, 20, "Trækker legetøj efter sig, mens det går", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Spiser med ske", MilestoneCategory.FineMotor, 18, 20, "Kan tegne kruseduller på papir", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Siger sætninger på 2-4 ord", MilestoneCategory.LanguageSpeech, 24, 28, "Gentager ord hørt i samtaler", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Sparker en bold fremad", MilestoneCategory.GrossMotor, 24, 28, "Hopper med begge fødder fri af gulvet", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Bygger tårn af 4+ klodser", MilestoneCategory.FineMotor, 24, 28, "Sorterer former og farver", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Renlig om dagen", MilestoneCategory.HealthSelfCare, 36, 42, "Holder sig tør om dagen og bruger toilet med hjælp", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Kører på trehjulet cykel", MilestoneCategory.GrossMotor, 36, 42, "Træder i pedalerne med god balance", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Fører en samtale på 2-3 sætninger", MilestoneCategory.LanguageSpeech, 36, 40, "Taler tydeligt nok til, at fremmede kan forstå det", isStandardGuideline: true),

        ChildMilestone.CreateExpected(childId, "Taber første mælketand", MilestoneCategory.Dental, 72, 84, "Den første mælketand falder naturligt ud", isStandardGuideline: true)
    };
}
