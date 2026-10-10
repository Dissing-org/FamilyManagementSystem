using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.Vehicles;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class CreateVehicleRequest
{
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public FuelType FuelType { get; set; } = FuelType.Gasoline;
    public int CurrentMileageKm { get; set; } = 0;
    public string? Vin { get; set; }
    public int? ServiceIntervalKm { get; set; }
    public int? ServiceIntervalMonths { get; set; }
    public DateTime? NextInspectionDate { get; set; }
}

public class CreateVehicleValidator : Validator<CreateVehicleRequest>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.Make)
            .NotEmpty().WithMessage("Make is required.")
            .MaximumLength(100).WithMessage("Make cannot exceed 100 characters.");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters.");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .MaximumLength(20).WithMessage("License plate cannot exceed 20 characters.");

        RuleFor(x => x.Year)
            .GreaterThan(1900).WithMessage("Year must be greater than 1900.");

        RuleFor(x => x.FuelType)
            .IsInEnum().WithMessage("A valid fuel type must be selected.");

        RuleFor(x => x.CurrentMileageKm)
            .GreaterThanOrEqualTo(0).WithMessage("Current mileage cannot be negative.");

        RuleFor(x => x.ServiceIntervalKm)
            .GreaterThan(0).WithMessage("Service interval (km) must be greater than zero.")
            .When(x => x.ServiceIntervalKm.HasValue);

        RuleFor(x => x.ServiceIntervalMonths)
            .GreaterThan(0).WithMessage("Service interval (months) must be greater than zero.")
            .When(x => x.ServiceIntervalMonths.HasValue);
    }
}

public class CreateVehicleEndpoint : Endpoint<CreateVehicleRequest, VehicleDto>
{
    private readonly CreateVehicleCommandHandler _handler;

    public CreateVehicleEndpoint(CreateVehicleCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/vehicles");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a new vehicle";
            s.Description = "Creates a new vehicle record with make, model, year, license plate, fuel type, and service intervals.";
        });
    }

    public override async Task HandleAsync(CreateVehicleRequest req, CancellationToken ct)
    {
        try
        {
            var command = new CreateVehicleCommand(
                req.Make,
                req.Model,
                req.Year,
                req.LicensePlate,
                req.FuelType,
                req.CurrentMileageKm,
                req.Vin,
                req.ServiceIntervalKm,
                req.ServiceIntervalMonths,
                req.NextInspectionDate);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/vehicles/{result.Id}", result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
