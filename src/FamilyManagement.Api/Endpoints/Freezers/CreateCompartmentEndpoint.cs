using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class CreateCompartmentRequest
{
    public Guid FreezerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
}

public class CreateCompartmentValidator : Validator<CreateCompartmentRequest>
{
    public CreateCompartmentValidator()
    {
        RuleFor(x => x.FreezerId)
            .NotEmpty().WithMessage("Freezer ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Compartment name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}

public class CreateCompartmentEndpoint : Endpoint<CreateCompartmentRequest, FreezerCompartmentDto>
{
    private readonly CreateCompartmentCommandHandler _handler;

    public CreateCompartmentEndpoint(CreateCompartmentCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/freezers/{freezerId}/compartments");
        Summary(s =>
        {
            s.Summary = "Add a compartment to a freezer";
            s.Description = "Creates a new drawer, shelf, or section inside a freezer.";
        });
    }

    public override async Task HandleAsync(CreateCompartmentRequest req, CancellationToken ct)
    {
        try
        {
            var command = new CreateCompartmentCommand(req.FreezerId, req.Name, req.DisplayOrder);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/freezers/{req.FreezerId}/compartments/{result.Id}", result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Freezer '{req.FreezerId}' not found." }));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
