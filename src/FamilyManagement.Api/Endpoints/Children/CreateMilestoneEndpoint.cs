using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;
using FamilyManagement.Domain.Enums;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request payload for creating a developmental milestone for a child.
/// </summary>
public class CreateMilestoneRequest
{
    public Guid ChildId { get; set; }
    public string Title { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public MilestoneCategory Category { get; set; }
    public bool IsAchieved { get; set; }
    public int? ExpectedAgeMonths { get; set; }
    public int? ExpectedWindowMaxMonths { get; set; }
    public DateTime? AchievedDate { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public string? PhotoUrl { get; set; }
}

/// <summary>
/// Validator for <see cref="CreateMilestoneRequest"/> validating required milestone data and dates.
/// </summary>
public class CreateMilestoneValidator : Validator<CreateMilestoneRequest>
{
    /// <summary>
    /// Initializes validation rules for milestone creation.
    /// </summary>
    public CreateMilestoneValidator()
    {
        RuleFor(x => x.ChildId)
            .NotEmpty().WithMessage("Child ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Milestone title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("A valid milestone category must be selected.");

        RuleFor(x => x.ExpectedAgeMonths)
            .GreaterThanOrEqualTo(0).WithMessage("Expected age cannot be negative.")
            .When(x => !x.IsAchieved && x.ExpectedAgeMonths.HasValue);

        RuleFor(x => x.AchievedDate)
            .Must(d => d!.Value.Date <= DateTime.UtcNow.Date.AddDays(1)).WithMessage("Achieved date cannot be in the future.")
            .When(x => x.IsAchieved && x.AchievedDate.HasValue);
    }
}

/// <summary>
/// Endpoint for adding a new developmental milestone to a child profile.
/// </summary>
public class CreateMilestoneEndpoint : Endpoint<CreateMilestoneRequest, ChildMilestoneDto>
{
    private readonly CreateMilestoneCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMilestoneEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process milestone creation.</param>
    public CreateMilestoneEndpoint(CreateMilestoneCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Post("/api/children/{childId}/milestones");
        Summary(s =>
        {
            s.Summary = "Add a child milestone";
            s.Description = "Creates a new developmental expectation or records an achieved milestone.";
        });
    }

    /// <summary>
    /// Handles the incoming milestone creation request asynchronously.
    /// </summary>
    /// <param name="req">The milestone creation request model.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CreateMilestoneRequest req, CancellationToken ct)
    {
        try
        {
            var command = new CreateMilestoneCommand(
                req.ChildId,
                req.Title,
                req.Category,
                req.IsAchieved,
                req.ExpectedAgeMonths,
                req.ExpectedWindowMaxMonths,
                req.AchievedDate,
                req.Description,
                req.Notes,
                req.PhotoUrl);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/children/{req.ChildId}/milestones/{result.Id}", result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
