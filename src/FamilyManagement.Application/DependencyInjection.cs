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

        // Insurance use cases
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Commands.CreateInsurancePolicyCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Commands.UpdateInsurancePolicyCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Commands.DeleteInsurancePolicyCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Queries.GetInsurancePoliciesQueryHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Queries.GetInsurancePolicyByIdQueryHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Queries.GetInsuranceOverviewQueryHandler>();

        return services;
    }
}
