using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;

namespace FamilyManagement.Api.Endpoints.Children;

public class UpdateChildSizesRequest
{
    public Guid Id { get; set; }
    public string? ClothesSize { get; set; }
    public string? ShoeSize { get; set; }
    public string? HatSize { get; set; }
    public string? DiaperSize { get; set; }
}

public class UpdateChildSizesEndpoint : Endpoint<UpdateChildSizesRequest, ChildProfileDto>
{
    private readonly UpdateChildSizesCommandHandler _handler;

    public UpdateChildSizesEndpoint(UpdateChildSizesCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Put("/api/children/{id}/sizes");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Update child wardrobe sizes";
            s.Description = "Quickly updates clothes size, shoe size, hat size, or diaper size.";
        });
    }

    public override async Task HandleAsync(UpdateChildSizesRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpdateChildSizesCommand(
                req.Id,
                req.ClothesSize,
                req.ShoeSize,
                req.HatSize,
                req.DiaperSize);

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
