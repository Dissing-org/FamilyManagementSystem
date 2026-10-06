using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;
using FluentValidation;

namespace FamilyManagement.Api.Endpoints.Children;

public class RecordGrowthMeasurementRequest
{
    public Guid ChildId { get; set; }
    public DateTime RecordedDate { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeadCircumferenceCm { get; set; }
    public string? Notes { get; set; }
}

public class RecordGrowthMeasurementValidator : Validator<RecordGrowthMeasurementRequest>
{
    public RecordGrowthMeasurementValidator()
    {
        RuleFor(x => x.ChildId)
            .NotEmpty().WithMessage("Child ID is required.");

        RuleFor(x => x.RecordedDate)
            .NotEmpty().WithMessage("Recorded date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Recorded date cannot be in the future.");

        RuleFor(x => x)
            .Must(x => x.HeightCm.HasValue || x.WeightKg.HasValue || x.HeadCircumferenceCm.HasValue)
            .WithMessage("At least one measurement (height, weight, or head circumference) must be provided.");

        RuleFor(x => x.HeightCm)
            .GreaterThan(0).WithMessage("Height must be greater than zero.")
            .When(x => x.HeightCm.HasValue);

        RuleFor(x => x.WeightKg)
            .GreaterThan(0).WithMessage("Weight must be greater than zero.")
            .When(x => x.WeightKg.HasValue);

        RuleFor(x => x.HeadCircumferenceCm)
            .GreaterThan(0).WithMessage("Head circumference must be greater than zero.")
            .When(x => x.HeadCircumferenceCm.HasValue);
    }
}

public class RecordGrowthMeasurementEndpoint : Endpoint<RecordGrowthMeasurementRequest, GrowthMeasurementDto>
{
    private readonly RecordGrowthMeasurementCommandHandler _handler;

    public RecordGrowthMeasurementEndpoint(RecordGrowthMeasurementCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/children/{childId}/growth");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Record physical growth measurement";
            s.Description = "Logs weight (kg), height (cm), or head circumference (cm) for a child.";
        });
    }

    public override async Task HandleAsync(RecordGrowthMeasurementRequest req, CancellationToken ct)
    {
        try
        {
            var command = new RecordGrowthMeasurementCommand(
                req.ChildId,
                req.RecordedDate,
                req.HeightCm,
                req.WeightKg,
                req.HeadCircumferenceCm,
                req.Notes);

            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/children/growth/{result.Id}", result));
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
