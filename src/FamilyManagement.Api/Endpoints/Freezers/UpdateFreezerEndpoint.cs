using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class UpdateFreezerRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateFreezerValidator : Validator<UpdateFreezerRequest>
{
    public UpdateFreezerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Freezer name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");
    }
}

public class UpdateFreezerEndpoint : Endpoint<UpdateFreezerRequest, FreezerDto>
{
    private readonly UpdateFreezerCommandHandler _handler;

    public UpdateFreezerEndpoint(UpdateFreezerCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Put("/api/freezers/{id}");
        Summary(s =>
        {
            s.Summary = "Update freezer details";
            s.Description = "Updates the name, location, or display order of an existing freezer.";
        });
    }

    public override async Task HandleAsync(UpdateFreezerRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpdateFreezerCommand(req.Id, req.Name, req.Location, req.DisplayOrder);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Freezer '{req.Id}' not found." }));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
