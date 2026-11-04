using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.Vehicles;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class RecordVehicleServiceRequest
{
    public Guid Id { get; set; }
    public DateTime ServiceDate { get; set; } = DateTime.UtcNow;
    public int MileageKm { get; set; }
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public ServiceType Type { get; set; } = ServiceType.RegularService;
    public string Title { get; set; } = string.Empty;
    public string? Workshop { get; set; }
    public decimal? Cost { get; set; }
    public string? Notes { get; set; }
    public Guid? ReceiptId { get; set; }
}

/// <summary>
/// Validator for <see cref="RecordVehicleServiceRequest"/>.
/// </summary>
public class RecordVehicleServiceValidator : Validator<RecordVehicleServiceRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecordVehicleServiceValidator"/> class and configures validation rules.
    /// </summary>
    public RecordVehicleServiceValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Vehicle ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Service title is required.")
            .MaximumLength(200).WithMessage("Service title cannot exceed 200 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("A valid service type must be selected.");

        RuleFor(x => x.MileageKm)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");

        RuleFor(x => x.ServiceDate)
            .NotEmpty().WithMessage("Service date is required.")
            .Must(d => d.Date <= DateTime.UtcNow.Date.AddDays(1)).WithMessage("Service date cannot be in the future.");

        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost cannot be negative.")
            .When(x => x.Cost.HasValue);
    }
}

/// <summary>
/// Endpoint for recording a vehicle service event.
/// </summary>
public class RecordVehicleServiceEndpoint : Endpoint<RecordVehicleServiceRequest, VehicleServiceRecordDto>
{
    private readonly RecordVehicleServiceCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordVehicleServiceEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler for recording vehicle service events.</param>
    public RecordVehicleServiceEndpoint(RecordVehicleServiceCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for recording a vehicle service event.
    /// </summary>
    public override void Configure()
    {
        Post("/api/vehicles/{id}/services");
        Summary(s =>
        {
            s.Summary = "Record a vehicle service event";
            s.Description = "Logs a service or repair maintenance event, workshop details, cost, optional receipt link, and updates vehicle mileage.";
        });
    }

    /// <summary>
    /// Handles the HTTP POST request to record a new vehicle service event.
    /// </summary>
    /// <param name="req">The request payload containing service record details.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(RecordVehicleServiceRequest req, CancellationToken ct)
    {
        try
        {
            var command = new RecordVehicleServiceCommand(
                req.Id,
                req.ServiceDate,
                req.MileageKm,
                req.Type,
                req.Title,
                req.Workshop,
                req.Cost,
                req.Notes,
                req.ReceiptId);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/vehicles/{req.Id}/services/{result.Id}", result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
