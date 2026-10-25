using FastEndpoints;
using FluentValidation;
using System.Text.Json.Serialization;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class UpsertFreezerCatalogItemRequest
{
    public string Name { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FreezerCategory Category { get; set; } = FreezerCategory.Other;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FreezerUnit Unit { get; set; } = FreezerUnit.Pieces;

    public decimal? MinimumQuantity { get; set; }
}

public class UpsertFreezerCatalogItemValidator : Validator<UpsertFreezerCatalogItemRequest>
{
    public UpsertFreezerCatalogItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(150).WithMessage("Item name cannot exceed 150 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Valid freezer category is required.");

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Valid freezer unit is required.");

        RuleFor(x => x.MinimumQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Minimum quantity cannot be negative.")
            .When(x => x.MinimumQuantity.HasValue);
    }
}

public class UpsertFreezerCatalogItemEndpoint : Endpoint<UpsertFreezerCatalogItemRequest, FreezerCatalogItemDto>
{
    private readonly UpsertFreezerCatalogItemCommandHandler _handler;

    public UpsertFreezerCatalogItemEndpoint(UpsertFreezerCatalogItemCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/freezers/catalog");
        Summary(s =>
        {
            s.Summary = "Upsert freezer catalog item";
            s.Description = "Creates or updates default category/unit for a named freezer item in the catalog.";
        });
    }

    public override async Task HandleAsync(UpsertFreezerCatalogItemRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpsertFreezerCatalogItemCommand(
                req.Name,
                req.Category,
                req.Unit,
                req.MinimumQuantity);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
