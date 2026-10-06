using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Api.Endpoints.Children;

public class ListChildrenEndpoint : EndpointWithoutRequest<List<ChildProfileDto>>
{
    private readonly IChildProfileRepository _repository;

    public ListChildrenEndpoint(IChildProfileRepository repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/children");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all children";
            s.Description = "Retrieves all registered child profiles in the family.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var children = await _repository.ListAllAsync(ct);
        var dtos = children.Select(c => ChildProfileDto.FromDomain(c)).ToList();
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(dtos));
    }
}
