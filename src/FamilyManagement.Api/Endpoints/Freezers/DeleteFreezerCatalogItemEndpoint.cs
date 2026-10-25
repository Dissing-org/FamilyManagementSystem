using FastEndpoints;
using FluentValidation;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class DeleteFreezerCatalogItemRequest
{
    public Guid Id { get; set; }
}

public class DeleteFreezerCatalogItemValidator : Validator<DeleteFreezerCatalogItemRequest>
{
    public DeleteFreezerCatalogItemValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class DeleteFreezerCatalogItemEndpoint : Endpoint<DeleteFreezerCatalogItemRequest>
{
    private readonly DeleteFreezerCatalogItemCommandHandler _handler;

    public DeleteFreezerCatalogItemEndpoint(DeleteFreezerCatalogItemCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/freezers/catalog/{id}");
        Summary(s =>
        {
            s.Summary = "Delete item from freezer catalog";
            s.Description = "Removes a suggestion from the catalog. Existing freezer items are unaffected.";
        });
    }

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
