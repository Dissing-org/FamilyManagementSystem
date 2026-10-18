using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class CreateFreezerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class CreateFreezerValidator : Validator<CreateFreezerRequest>
{
    public CreateFreezerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Freezer name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");
    }
}

public class CreateFreezerEndpoint : Endpoint<CreateFreezerRequest, FreezerDto>
{
    private readonly CreateFreezerCommandHandler _handler;

    public CreateFreezerEndpoint(CreateFreezerCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/freezers");
        Summary(s =>
        {
            s.Summary = "Create a new freezer";
            s.Description = "Creates a new freezer unit in the household.";
        });
    }

    public override async Task HandleAsync(CreateFreezerRequest req, CancellationToken ct)
    {
        try
        {
            var command = new CreateFreezerCommand(req.Name, req.Location, req.DisplayOrder);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/freezers/{result.Id}", result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
