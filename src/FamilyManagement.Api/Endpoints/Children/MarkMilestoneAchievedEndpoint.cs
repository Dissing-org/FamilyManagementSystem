using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request payload for marking a child developmental milestone as achieved.
/// </summary>
public class MarkMilestoneAchievedRequest
{
    public Guid Id { get; set; }
    public DateTime AchievedDate { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
}

/// <summary>
/// Validator for <see cref="MarkMilestoneAchievedRequest"/> ensuring required milestone ID and valid achieved date.
/// </summary>
public class MarkMilestoneAchievedValidator : Validator<MarkMilestoneAchievedRequest>
{
    /// <summary>
    /// Initializes validation rules for marking a milestone as achieved.
    /// </summary>
    public MarkMilestoneAchievedValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Milestone ID is required.");

        RuleFor(x => x.AchievedDate)
            .NotEmpty().WithMessage("Achieved date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Achieved date cannot be in the future.");
    }
}

/// <summary>
/// Endpoint for marking a milestone as achieved with completion date and optional notes.
/// </summary>
public class MarkMilestoneAchievedEndpoint : Endpoint<MarkMilestoneAchievedRequest, ChildMilestoneDto>
{
    private readonly MarkMilestoneAchievedCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkMilestoneAchievedEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to mark milestones as achieved.</param>
    public MarkMilestoneAchievedEndpoint(MarkMilestoneAchievedCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Put("/api/children/milestones/{id}/achieve");
        Summary(s =>
        {
            s.Summary = "Mark a milestone as achieved";
            s.Description = "Updates an expected developmental milestone to achieved with exact completion date and celebration note.";
        });
    }

    /// <summary>
    /// Handles the request to mark a developmental milestone as achieved asynchronously.
    /// </summary>
    /// <param name="req">The request containing the milestone ID and achievement details.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
