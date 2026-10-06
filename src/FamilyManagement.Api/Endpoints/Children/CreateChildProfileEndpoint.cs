using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;
using FamilyManagement.Domain.Enums;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Children;

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

public class CreateChildProfileValidator : Validator<CreateChildProfileRequest>
{
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
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Date of birth cannot be in the future.");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("A valid gender must be selected.");
    }
}

public class CreateChildProfileEndpoint : Endpoint<CreateChildProfileRequest, ChildProfileDto>
{
    private readonly CreateChildProfileCommandHandler _handler;

    public CreateChildProfileEndpoint(CreateChildProfileCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/children");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a child profile";
            s.Description = "Creates a new child record with date of birth, gender, and wardrobe sizes.";
        });
    }

    public override async Task HandleAsync(CreateChildProfileRequest req, CancellationToken ct)
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
}
