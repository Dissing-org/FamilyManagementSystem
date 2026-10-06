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
        // 2 Months
        ChildMilestone.CreateExpected(childId, "Smiles at people", MilestoneCategory.SocialEmotional, 2, 3, "Can briefly calm himself, looks at parents", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Coos, makes gurgling sounds", MilestoneCategory.LanguageSpeech, 2, 3, "Turns head toward sounds", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Holds head up when on tummy", MilestoneCategory.GrossMotor, 2, 3, "Pushes up when lying on tummy", isStandardGuideline: true),

        // 4 Months
        ChildMilestone.CreateExpected(childId, "Chuckles and laughs", MilestoneCategory.LanguageSpeech, 4, 5, "Begins to babble with expression", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Reaches for toy with one hand", MilestoneCategory.FineMotor, 4, 6, "Uses hands and eyes together", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Rolls over from tummy to back", MilestoneCategory.GrossMotor, 4, 6, "Can roll from front to back", isStandardGuideline: true),

        // 6 Months
        ChildMilestone.CreateExpected(childId, "First baby tooth emerges", MilestoneCategory.Dental, 6, 10, "Usually lower central incisors", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Sits without support", MilestoneCategory.GrossMotor, 6, 8, "Sits steadily with hands free to play", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Passes things from one hand to another", MilestoneCategory.FineMotor, 6, 8, "Explores objects by putting in mouth", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Responds to own name", MilestoneCategory.LanguageSpeech, 6, 7, "Turns head when name is called", isStandardGuideline: true),

        // 9 Months
        ChildMilestone.CreateExpected(childId, "Crawling on hands and knees", MilestoneCategory.GrossMotor, 9, 11, "Gets into sitting position and crawls", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Pulls up to stand", MilestoneCategory.GrossMotor, 9, 11, "Pulls up on furniture", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Pincer grasp (thumb and index finger)", MilestoneCategory.FineMotor, 9, 12, "Picks up small pieces of food", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Plays peek-a-boo", MilestoneCategory.SocialEmotional, 9, 10, "Has favorite toys and looks for hidden items", isStandardGuideline: true),

        // 12 Months
        ChildMilestone.CreateExpected(childId, "First independent steps / Walking", MilestoneCategory.GrossMotor, 12, 15, "Takes a few steps without holding on", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Says first clear word (e.g. Mama, Dada)", MilestoneCategory.LanguageSpeech, 12, 14, "Uses simple gestures like waving bye-bye", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Drinks from an open cup / sippy cup", MilestoneCategory.HealthSelfCare, 12, 14, "Helps when being dressed", isStandardGuideline: true),

        // 18 Months
        ChildMilestone.CreateExpected(childId, "Says 10 or more words", MilestoneCategory.LanguageSpeech, 18, 20, "Points to show someone what he wants", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Walks up steps and runs", MilestoneCategory.GrossMotor, 18, 20, "Pulls toys while walking", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Eats with a spoon", MilestoneCategory.FineMotor, 18, 20, "Can scribble on paper", isStandardGuideline: true),

        // 24 Months (2 Years)
        ChildMilestone.CreateExpected(childId, "Says 2-4 word sentences", MilestoneCategory.LanguageSpeech, 24, 28, "Repeats words overheard in conversations", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Kicks a ball forward", MilestoneCategory.GrossMotor, 24, 28, "Jumps with both feet off the ground", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Builds tower of 4+ blocks", MilestoneCategory.FineMotor, 24, 28, "Sorts shapes and colors", isStandardGuideline: true),

        // 3 Years
        ChildMilestone.CreateExpected(childId, "Daytime potty trained", MilestoneCategory.HealthSelfCare, 36, 42, "Stays dry during day, uses toilet with help", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Rides a tricycle", MilestoneCategory.GrossMotor, 36, 42, "Pedals tricycle with good balance", isStandardGuideline: true),
        ChildMilestone.CreateExpected(childId, "Carries on conversation of 2-3 sentences", MilestoneCategory.LanguageSpeech, 36, 40, "Speaks clearly enough for strangers to understand", isStandardGuideline: true),

        // 6 Years
        ChildMilestone.CreateExpected(childId, "Loses first baby tooth", MilestoneCategory.Dental, 72, 84, "Primary central incisor naturally falls out", isStandardGuideline: true)
    };
}
