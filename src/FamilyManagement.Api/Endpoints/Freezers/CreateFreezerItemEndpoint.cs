using FastEndpoints;
using FluentValidation;
using System.Text.Json.Serialization;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request payload for creating and storing a new freezer inventory item.
/// </summary>
public class CreateFreezerItemRequest
{
    public Guid CompartmentId { get; set; }
    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FreezerCategory Category { get; set; } = FreezerCategory.Other;

    public decimal Quantity { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FreezerUnit Unit { get; set; } = FreezerUnit.Pieces;

    public DateTime FreezeDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public decimal? MinimumQuantity { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Validator for freezer item creation requests.
/// </summary>
public class CreateFreezerItemValidator : Validator<CreateFreezerItemRequest>
{
    /// <summary>
    /// Initializes validation rules for adding a freezer item.
    /// </summary>
    public CreateFreezerItemValidator()
    {
        RuleFor(x => x.CompartmentId)
            .NotEmpty().WithMessage("Compartment ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Valid freezer category is required.");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Valid freezer unit is required.");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum quantity cannot be negative.")
            .When(x => x.MinimumQuantity.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
    }
}

/// <summary>
/// Endpoint that adds a newly frozen item or batch into a compartment.
/// </summary>
public class CreateFreezerItemEndpoint : Endpoint<CreateFreezerItemRequest, FreezerItemDto>
{
    private readonly CreateFreezerItemCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFreezerItemEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process freezer item creation.</param>
    public CreateFreezerItemEndpoint(CreateFreezerItemCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/freezer-items");
        Summary(s =>
        {
            s.Summary = "Add a freezer item";
            s.Description = "Records a newly frozen item or batch into a compartment.";
        });
    }

    /// <summary>
    /// Handles the request to add a new freezer item into the specified compartment.
    /// </summary>
    /// <param name="req">The freezer item creation request.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CreateFreezerItemRequest req, CancellationToken ct)
    {
        try
        {
            var command = new CreateFreezerItemCommand(
                req.CompartmentId,
                req.Name,
                req.Category,
                req.Quantity,
                req.Unit,
                req.FreezeDate,
                req.ExpiryDate,
                req.MinimumQuantity,
                req.Notes);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/freezer-items/{result.Id}", result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
