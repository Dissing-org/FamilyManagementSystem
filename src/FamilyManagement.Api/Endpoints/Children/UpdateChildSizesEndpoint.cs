using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Commands;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request payload for updating wardrobe and shoe sizes for a child.
/// </summary>
public class UpdateChildSizesRequest
{
    public Guid Id { get; set; }
    public string? ClothesSize { get; set; }
    public string? ShoeSize { get; set; }
    public string? HatSize { get; set; }
    public string? DiaperSize { get; set; }
}

/// <summary>
/// Endpoint for updating wardrobe sizes (clothes, shoes, hat, diaper) for a child.
/// </summary>
public class UpdateChildSizesEndpoint : Endpoint<UpdateChildSizesRequest, ChildProfileDto>
{
    private readonly UpdateChildSizesCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateChildSizesEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to update child wardrobe sizes.</param>
    public UpdateChildSizesEndpoint(UpdateChildSizesCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Put("/api/children/{id}/sizes");
        Summary(s =>
        {
            s.Summary = "Update child wardrobe sizes";
            s.Description = "Quickly updates clothes size, shoe size, hat size, or diaper size.";
        });
    }

    /// <summary>
    /// Handles the request to update a child's wardrobe sizes asynchronously.
    /// </summary>
    /// <param name="req">The request containing the child ID and size specifications.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
