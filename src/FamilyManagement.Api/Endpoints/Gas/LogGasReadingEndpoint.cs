using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.Commands;
using FamilyManagement.Application.UseCases.Gas.DTOs;

namespace FamilyManagement.Api.Endpoints.Gas;

public class LogGasReadingRequest
{
    public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
    public decimal ReadingM3 { get; set; }
    public string? Notes { get; set; }
}

public class LogGasReadingValidator : Validator<LogGasReadingRequest>
{
    public LogGasReadingValidator()
    {
        RuleFor(x => x.ReadingM3)
            .GreaterThanOrEqualTo(0).WithMessage("Gas reading cannot be negative.");

        RuleFor(x => x.RecordedDate)
            .NotEmpty().WithMessage("Recorded date is required.")
            .Must(d => d.Date <= DateTime.UtcNow.Date.AddDays(1)).WithMessage("Recorded date cannot be in the future.");
    }
}

public class LogGasReadingEndpoint : Endpoint<LogGasReadingRequest, GasReadingDto>
{
    private readonly LogGasReadingCommandHandler _handler;

    public LogGasReadingEndpoint(LogGasReadingCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/gas/readings");
        Summary(s =>
        {
            s.Summary = "Log a new gas meter reading";
            s.Description = "Records a cumulative gas reading in cubic meters (m3).";
        });
    }

    public override async Task HandleAsync(LogGasReadingRequest req, CancellationToken ct)
    {
        try
        {
            var command = new LogGasReadingCommand(req.RecordedDate, req.ReadingM3, req.Notes);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/gas/readings/{result.Id}", result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
