using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request parameters identifying the freezer catalog item to delete.
/// </summary>
public class DeleteFreezerCatalogItemRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Validator for freezer catalog item deletion requests.
/// </summary>
public class DeleteFreezerCatalogItemValidator : Validator<DeleteFreezerCatalogItemRequest>
{
    /// <summary>
    /// Initializes validation rules for deleting a catalog item.
    /// </summary>
    public DeleteFreezerCatalogItemValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

/// <summary>
/// Endpoint that deletes an item suggestion from the persistent freezer catalog.
/// </summary>
public class DeleteFreezerCatalogItemEndpoint : Endpoint<DeleteFreezerCatalogItemRequest>
{
    private readonly DeleteFreezerCatalogItemCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFreezerCatalogItemEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process catalog item deletion.</param>
    public DeleteFreezerCatalogItemEndpoint(DeleteFreezerCatalogItemCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/freezers/catalog/{id}");
        Summary(s =>
        {
            s.Summary = "Delete item from freezer catalog";
            s.Description = "Removes a suggestion from the catalog. Existing freezer items are unaffected.";
        });
    }

    /// <summary>
    /// Handles the request to remove an item from the freezer catalog.
    /// </summary>
    /// <param name="req">The delete request containing the catalog item ID.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteFreezerCatalogItemRequest req, CancellationToken ct)
    {
        try
        {
            await _handler.HandleAsync(new DeleteFreezerCatalogItemCommand(req.Id), ct);
            await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
