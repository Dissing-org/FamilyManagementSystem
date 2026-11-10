using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.Commands;
using FamilyManagement.Application.UseCases.Gas.DTOs;

namespace FamilyManagement.Api.Endpoints.Gas;

public class UpdateGasReadingRequest
{
    public Guid Id { get; set; }
    public DateTime RecordedDate { get; set; }
    public decimal ReadingM3 { get; set; }
    public string? Notes { get; set; }
}

public class UpdateGasReadingValidator : Validator<UpdateGasReadingRequest>
{
    public UpdateGasReadingValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Reading ID is required.");
        RuleFor(x => x.ReadingM3).GreaterThanOrEqualTo(0).WithMessage("Gas reading cannot be negative.");
        RuleFor(x => x.RecordedDate).NotEmpty().WithMessage("Recorded date is required.")
            .Must(d => d.Date <= DateTime.UtcNow.Date.AddDays(1)).WithMessage("Recorded date cannot be in the future.");
    }
}

public class UpdateGasReadingEndpoint : Endpoint<UpdateGasReadingRequest, GasReadingDto>
{
    private readonly UpdateGasReadingCommandHandler _handler;

    public UpdateGasReadingEndpoint(UpdateGasReadingCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Put("/api/gas/readings/{id}");
        Summary(s =>
        {
            s.Summary = "Update an existing gas meter reading";
        });
    }

    public override async Task HandleAsync(UpdateGasReadingRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpdateGasReadingCommand(req.Id, req.RecordedDate, req.ReadingM3, req.Notes);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
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
