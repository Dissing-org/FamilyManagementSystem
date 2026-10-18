using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class CreateFreezerItemRequest
{
    public Guid CompartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public FreezerCategory Category { get; set; } = FreezerCategory.Other;
    public decimal Quantity { get; set; }
    public FreezerUnit Unit { get; set; } = FreezerUnit.Pieces;
    public DateTime FreezeDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public decimal? MinimumQuantity { get; set; }
    public string? Notes { get; set; }
}

public class CreateFreezerItemValidator : Validator<CreateFreezerItemRequest>
{
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

public class CreateFreezerItemEndpoint : Endpoint<CreateFreezerItemRequest, FreezerItemDto>
{
    private readonly CreateFreezerItemCommandHandler _handler;

    public CreateFreezerItemEndpoint(CreateFreezerItemCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/freezer-items");
        Summary(s =>
        {
            s.Summary = "Add a freezer item";
            s.Description = "Records a newly frozen item or batch into a compartment.";
        });
    }

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
