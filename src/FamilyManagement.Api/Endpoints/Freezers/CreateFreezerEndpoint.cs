using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request payload for creating a new freezer unit.
/// </summary>
public class CreateFreezerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

/// <summary>
/// Validator for freezer creation requests.
/// </summary>
public class CreateFreezerValidator : Validator<CreateFreezerRequest>
{
    /// <summary>
    /// Initializes validation rules for creating a new freezer.
    /// </summary>
    public CreateFreezerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Freezer name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");
    }
}

/// <summary>
/// Endpoint that creates a new freezer unit.
/// </summary>
public class CreateFreezerEndpoint : Endpoint<CreateFreezerRequest, FreezerDto>
{
    private readonly CreateFreezerCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFreezerEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process freezer creation.</param>
    public CreateFreezerEndpoint(CreateFreezerCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/freezers");
        Summary(s =>
        {
            s.Summary = "Create a new freezer";
            s.Description = "Creates a new freezer unit in the household.";
        });
    }

    /// <summary>
    /// Handles the request to create a new freezer unit.
    /// </summary>
    /// <param name="req">The freezer creation request.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
