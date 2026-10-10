using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class LogMileageRequest
{
    public Guid Id { get; set; }
    public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
    public int MileageKm { get; set; }
    public string? Notes { get; set; }
}

public class LogMileageValidator : Validator<LogMileageRequest>
{
    public LogMileageValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Vehicle ID is required.");

        RuleFor(x => x.MileageKm)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage cannot be negative.");

        RuleFor(x => x.RecordedDate)
            .NotEmpty().WithMessage("Recorded date is required.")
            .Must(d => d.Date <= DateTime.UtcNow.Date.AddDays(1)).WithMessage("Recorded date cannot be in the future.");
    }
}

public class LogMileageEndpoint : Endpoint<LogMileageRequest, MileageLogDto>
{
    private readonly LogMileageCommandHandler _handler;

    public LogMileageEndpoint(LogMileageCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/vehicles/{id}/mileage");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Log mileage for a vehicle";
            s.Description = "Logs a mileage reading and updates the vehicle current odometer reading if the value is higher.";
        });
    }

    public override async Task HandleAsync(LogMileageRequest req, CancellationToken ct)
    {
        try
        {
            var command = new LogMileageCommand(
                req.Id,
                req.RecordedDate,
                req.MileageKm,
                req.Notes);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/vehicles/{req.Id}/mileage/{result.Id}", result));
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
