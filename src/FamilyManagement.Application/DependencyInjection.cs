using Microsoft.Extensions.DependencyInjection;
using FamilyManagement.Application.UseCases.Receipts.Commands;
using FamilyManagement.Application.UseCases.Receipts.Queries;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<UploadReceiptCommandHandler>();
        services.AddScoped<DeleteReceiptCommandHandler>();
        services.AddScoped<GetReceiptByIdQueryHandler>();
        services.AddScoped<ListReceiptsQueryHandler>();
        services.AddScoped<GetDistinctMerchantsQueryHandler>();

        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Commands.CreateInsurancePolicyCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Commands.UpdateInsurancePolicyCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Commands.DeleteInsurancePolicyCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Queries.GetInsurancePoliciesQueryHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Queries.GetInsurancePolicyByIdQueryHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Insurances.Queries.GetInsuranceOverviewQueryHandler>();

        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.CreateChildProfileCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.UpdateChildSizesCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.RecordGrowthMeasurementCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.DeleteGrowthMeasurementCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.CreateMilestoneCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.MarkMilestoneAchievedCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Commands.SeedStandardMilestonesCommandHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Queries.GetChildDashboardQueryHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Queries.GetGrowthHistoryQueryHandler>();
        services.AddScoped<FamilyManagement.Application.UseCases.Children.Queries.GetMilestonesQueryHandler>();

        services.AddScoped<CreateVehicleCommandHandler>();
        services.AddScoped<UpdateVehicleDetailsCommandHandler>();
        services.AddScoped<LogMileageCommandHandler>();
        services.AddScoped<RecordVehicleServiceCommandHandler>();
        services.AddScoped<DeleteVehicleCommandHandler>();
        services.AddScoped<DeleteMileageLogCommandHandler>();
        services.AddScoped<DeleteVehicleServiceRecordCommandHandler>();
        services.AddScoped<GetAllVehiclesQueryHandler>();
        services.AddScoped<GetVehicleByIdQueryHandler>();
        services.AddScoped<GetMileageLogsQueryHandler>();
        services.AddScoped<GetServiceRecordsQueryHandler>();
        services.AddScoped<GetVehicleSummaryQueryHandler>();

        return services;
    }
}
