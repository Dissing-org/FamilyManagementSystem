using Microsoft.Extensions.DependencyInjection;
using FamilyManagement.Application.UseCases.Receipts.Commands;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UploadReceiptCommandHandler>();
        services.AddScoped<DeleteReceiptCommandHandler>();
        services.AddScoped<GetReceiptByIdQueryHandler>();
        services.AddScoped<ListReceiptsQueryHandler>();

        return services;
    }
}
