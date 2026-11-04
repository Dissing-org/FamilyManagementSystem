using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;
using FamilyManagement.Domain.Enums;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request payload for creating a child profile.
/// </summary>
public class CreateChildProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public Gender Gender { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ClothesSize { get; set; }
    public string? ShoeSize { get; set; }
    public string? HatSize { get; set; }
    public string? DiaperSize { get; set; }
}

/// <summary>
/// Validator for <see cref="CreateChildProfileRequest"/> enforcing required fields and birth date rules.
/// </summary>
public class CreateChildProfileValidator : Validator<CreateChildProfileRequest>
{
    /// <summary>
    /// Initializes validation rules for child profile creation.
    /// </summary>
    public CreateChildProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(d => d.Date <= DateTime.UtcNow.Date.AddDays(1)).WithMessage("Date of birth cannot be in the future.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("A valid gender must be selected.");
    }
}

/// <summary>
/// Endpoint for registering a new child profile in the family management system.
/// </summary>
public class CreateChildProfileEndpoint : Endpoint<CreateChildProfileRequest, ChildProfileDto>
{
    private readonly CreateChildProfileCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateChildProfileEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process child profile creation.</param>
    public CreateChildProfileEndpoint(CreateChildProfileCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Post("/api/children");
        Summary(s =>
        {
            s.Summary = "Register a child profile";
            s.Description = "Creates a new child record with date of birth, gender, and wardrobe sizes.";
        });
    }

    /// <summary>
    /// Handles the incoming child profile creation request asynchronously.
    /// </summary>
    /// <param name="req">The child profile creation request model.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CreateChildProfileRequest req, CancellationToken ct)
    {
        try
        {
            var command = new CreateChildProfileCommand(
                req.FirstName,
                req.LastName,
                req.DateOfBirth,
                req.Gender,
                req.AvatarUrl,
                req.ClothesSize,
                req.ShoeSize,
                req.HatSize,
                req.DiaperSize);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/children/{result.Id}", result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
