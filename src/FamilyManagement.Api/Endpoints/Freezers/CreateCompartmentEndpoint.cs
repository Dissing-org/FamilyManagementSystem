using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request payload for creating a new compartment inside a freezer.
/// </summary>
public class CreateCompartmentRequest
{
    public Guid FreezerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Validator for compartment creation requests.
/// </summary>
public class CreateCompartmentValidator : Validator<CreateCompartmentRequest>
{
    /// <summary>
    /// Initializes validation rules for creating a freezer compartment.
    /// </summary>
    public CreateCompartmentValidator()
    {
        RuleFor(x => x.FreezerId)
            .NotEmpty().WithMessage("Freezer ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Compartment name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}

/// <summary>
/// Endpoint that adds a compartment to an existing freezer.
/// </summary>
public class CreateCompartmentEndpoint : Endpoint<CreateCompartmentRequest, FreezerCompartmentDto>
{
    private readonly CreateCompartmentCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCompartmentEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process compartment creation.</param>
    public CreateCompartmentEndpoint(CreateCompartmentCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/freezers/{freezerId}/compartments");
        Summary(s =>
        {
            s.Summary = "Add a compartment to a freezer";
            s.Description = "Creates a new drawer, shelf, or section inside a freezer.";
        });
    }

    /// <summary>
    /// Handles the request to create a new freezer compartment.
    /// </summary>
    /// <param name="req">The compartment creation request.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
