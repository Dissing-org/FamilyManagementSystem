using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class UpdateFreezerItemRequest
{
    public Guid Id { get; set; }
    public Guid CompartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FreezerCategory Category { get; set; }
    public decimal Quantity { get; set; }
    public FreezerUnit Unit { get; set; }
    public DateTime FreezeDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? MinimumQuantity { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFreezerItemValidator : Validator<UpdateFreezerItemRequest>
{
    public UpdateFreezerItemValidator()
    {
        RuleFor(x => x.CompartmentId)
            .NotEmpty().WithMessage("Compartment ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");

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

public class UpdateFreezerItemEndpoint : Endpoint<UpdateFreezerItemRequest, FreezerItemDto>
{
    private readonly UpdateFreezerItemCommandHandler _handler;

    public UpdateFreezerItemEndpoint(UpdateFreezerItemCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Put("/api/freezer-items/{id}");
        Summary(s =>
        {
            s.Summary = "Update freezer item details";
            s.Description = "Modifies an item's details, location, quantity, or dates.";
        });
    }

    public override async Task HandleAsync(UpdateFreezerItemRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpdateFreezerItemCommand(
                req.Id,
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
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Item '{req.Id}' not found." }));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
