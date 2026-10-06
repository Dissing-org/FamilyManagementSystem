using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Children;

public class MarkMilestoneAchievedRequest
{
    public Guid Id { get; set; }
    public DateTime AchievedDate { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
}

public class MarkMilestoneAchievedValidator : Validator<MarkMilestoneAchievedRequest>
{
    public MarkMilestoneAchievedValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone ID is required.");

        RuleFor(x => x.AchievedDate)
            .NotEmpty().WithMessage("Achieved date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Achieved date cannot be in the future.");
    }
}

public class MarkMilestoneAchievedEndpoint : Endpoint<MarkMilestoneAchievedRequest, ChildMilestoneDto>
{
    private readonly MarkMilestoneAchievedCommandHandler _handler;

    public MarkMilestoneAchievedEndpoint(MarkMilestoneAchievedCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Put("/api/children/milestones/{id}/achieve");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Mark a milestone as achieved";
            s.Description = "Updates an expected developmental milestone to achieved with exact completion date and celebration note.";
        });
    }

    public override async Task HandleAsync(MarkMilestoneAchievedRequest req, CancellationToken ct)
    {
        try
        {
            var command = new MarkMilestoneAchievedCommand(
                req.Id,
                req.AchievedDate,
                req.Notes,
                req.PhotoUrl);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
