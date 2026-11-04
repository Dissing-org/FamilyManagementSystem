using FastEndpoints;
using FluentValidation;
using System.Text.Json.Serialization;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request payload for consuming or recording waste/gift of a freezer item.
/// </summary>
public class ConsumeFreezerItemRequest
{
    public Guid Id { get; set; }
    public decimal QuantityToConsume { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ConsumptionReason Reason { get; set; } = ConsumptionReason.Consumed;

    public string? Notes { get; set; }
}

/// <summary>
/// Validator for verifying freezer item consumption request fields.
/// </summary>
public class ConsumeFreezerItemValidator : Validator<ConsumeFreezerItemRequest>
{
    /// <summary>
    /// Initializes validation rules for consuming or decrementing a freezer item.
    /// </summary>
    public ConsumeFreezerItemValidator()
    {
        RuleFor(x => x.QuantityToConsume)
            .GreaterThan(0).WithMessage("Quantity to consume must be greater than zero.");

        RuleFor(x => x.Reason)
            .IsInEnum().WithMessage("Valid consumption reason is required.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters.");
    }
}

/// <summary>
/// Endpoint that records consumption, spoilage, or gifting of a freezer item.
/// </summary>
public class ConsumeFreezerItemEndpoint : Endpoint<ConsumeFreezerItemRequest, FreezerItemDto>
{
    private readonly ConsumeFreezerItemCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumeFreezerItemEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process freezer item consumption.</param>
    public ConsumeFreezerItemEndpoint(ConsumeFreezerItemCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/freezer-items/{id}/consume");
        Summary(s =>
        {
            s.Summary = "Consume or decrement freezer item";
            s.Description = "Records consumption (used, wasted, gifted) and decrements the quantity.";
        });
    }

    /// <summary>
    /// Handles the freezer item consumption request, delegating to the consumption command handler.
    /// </summary>
    /// <param name="req">The consumption request parameters.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(ConsumeFreezerItemRequest req, CancellationToken ct)
    {
        try
        {
            var command = new ConsumeFreezerItemCommand(req.Id, req.QuantityToConsume, req.Reason, req.Notes);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Item '{req.Id}' not found." }));
        }
        catch (InvalidOperationException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
