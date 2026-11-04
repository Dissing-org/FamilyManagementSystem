using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Endpoint for listing all registered children in the family.
/// </summary>
public class ListChildrenEndpoint : EndpointWithoutRequest<List<ChildProfileDto>>
{
    private readonly IChildProfileRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListChildrenEndpoint"/> class.
    /// </summary>
    /// <param name="repository">The child profile repository used to retrieve profiles.</param>
    public ListChildrenEndpoint(IChildProfileRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/children");
        Summary(s =>
        {
            s.Summary = "List all children";
            s.Description = "Retrieves all registered child profiles in the family.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve all registered child profiles asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var children = await _repository.ListAllAsync(ct);
        var dtos = children.Select(c => ChildProfileDto.FromDomain(c)).ToList();
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(dtos));
    }
}
