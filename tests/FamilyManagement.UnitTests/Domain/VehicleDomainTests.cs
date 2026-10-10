using FluentAssertions;
using FamilyManagement.Application.Vehicles;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;
using Xunit;

namespace FamilyManagement.UnitTests.Domain;

public class VehicleDomainTests
{
    [Fact]
    public void CreateVehicle_WithValidData_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var vehicle = Vehicle.Create(
            "Skoda",
            "Octavia Combi",
            2021,
            "AB 12 345",
            FuelType.Diesel,
            45000,
            "TMBJJ7NE8L0123456",
            15000,
            12,
            new DateTime(2027, 4, 1, 0, 0, 0, DateTimeKind.Utc));

        // Assert
        vehicle.Id.Value.Should().NotBeEmpty();
        vehicle.Make.Should().Be("Skoda");
        vehicle.Model.Should().Be("Octavia Combi");
        vehicle.Year.Should().Be(2021);
        vehicle.LicensePlate.Should().Be("AB 12 345");
        vehicle.FuelType.Should().Be(FuelType.Diesel);
        vehicle.CurrentMileageKm.Should().Be(45000);
        vehicle.Vin.Should().Be("TMBJJ7NE8L0123456");
        vehicle.ServiceIntervalKm.Should().Be(15000);
        vehicle.ServiceIntervalMonths.Should().Be(12);
        vehicle.NextInspectionDate.Should().Be(new DateTime(2027, 4, 1, 0, 0, 0, DateTimeKind.Utc));
        vehicle.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("", "Model", "AB12345")]
    [InlineData("Make", "", "AB12345")]
    [InlineData("Make", "Model", "")]
    [InlineData("   ", "Model", "AB12345")]
    public void CreateVehicle_WithInvalidIdentityFields_ShouldThrowArgumentException(string make, string model, string licensePlate)
    {
        var act = () => Vehicle.Create(make, model, 2020, licensePlate, FuelType.Gasoline, 10000);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateVehicle_WithYearPriorTo1900_ShouldThrowArgumentException()
    {
        var act = () => Vehicle.Create("Ford", "Model T", 1899, "XY 123", FuelType.Gasoline, 0);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Year must be greater than 1900*");
    }

    [Fact]
    public void CreateVehicle_WithNegativeMileage_ShouldThrowArgumentException()
    {
        var act = () => Vehicle.Create("VW", "Golf", 2020, "AB 12 345", FuelType.Gasoline, -10);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Current mileage cannot be negative*");
    }

    [Fact]
    public void UpdateCurrentMileage_WithHigherValue_ShouldUpdateMileageAndTimestamp()
    {
        var vehicle = Vehicle.Create("Tesla", "Model 3", 2022, "EV 99 999", FuelType.Electric, 20000);

        vehicle.UpdateCurrentMileage(25500);

        vehicle.CurrentMileageKm.Should().Be(25500);
        vehicle.UpdatedAt.Should().NotBeNull();
        vehicle.UpdatedAt.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateCurrentMileage_WithLowerValueWithoutAllowDecrease_ShouldThrowArgumentException()
    {
        var vehicle = Vehicle.Create("Tesla", "Model 3", 2022, "EV 99 999", FuelType.Electric, 30000);

        var act = () => vehicle.UpdateCurrentMileage(25000, allowDecrease: false);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*cannot be less than current mileage*");
    }

    [Fact]
    public void UpdateCurrentMileage_WithLowerValueWithAllowDecrease_ShouldSucceed()
    {
        var vehicle = Vehicle.Create("Tesla", "Model 3", 2022, "EV 99 999", FuelType.Electric, 30000);

        vehicle.UpdateCurrentMileage(29000, allowDecrease: true);

        vehicle.CurrentMileageKm.Should().Be(29000);
    }

    [Fact]
    public void CreateMileageLogEntry_WithValidData_ShouldCreateEntry()
    {
        var vehicleId = VehicleId.New();
        var date = DateTime.UtcNow.Date;

        var entry = MileageLogEntry.Create(vehicleId, date, 45200, "Trip to Aarhus");

        entry.Id.Value.Should().NotBeEmpty();
        entry.VehicleId.Should().Be(vehicleId);
        entry.MileageKm.Should().Be(45200);
        entry.RecordedDate.Date.Should().Be(date);
        entry.Notes.Should().Be("Trip to Aarhus");
    }

    [Fact]
    public void CreateMileageLogEntry_WithFarFutureDate_ShouldThrowArgumentException()
    {
        var vehicleId = VehicleId.New();
        var futureDate = DateTime.UtcNow.Date.AddDays(5);

        var act = () => MileageLogEntry.Create(vehicleId, futureDate, 45200);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*cannot be in the future*");
    }

    [Fact]
    public void CreateVehicleServiceRecord_WithValidData_ShouldCreateRecord()
    {
        var vehicleId = VehicleId.New();
        var date = DateTime.UtcNow.Date.AddDays(-10);

        var record = VehicleServiceRecord.Create(
            vehicleId,
            date,
            60000,
            ServiceType.RegularService,
            "Major 60,000 km Service",
            "Skoda Service Gladsaxe",
            4200.50m,
            "All filters replaced");

        record.Id.Value.Should().NotBeEmpty();
        record.VehicleId.Should().Be(vehicleId);
        record.ServiceDate.Date.Should().Be(date);
        record.MileageKm.Should().Be(60000);
        record.Type.Should().Be(ServiceType.RegularService);
        record.Title.Should().Be("Major 60,000 km Service");
        record.Workshop.Should().Be("Skoda Service Gladsaxe");
        record.Cost.Should().Be(4200.50m);
        record.Notes.Should().Be("All filters replaced");
    }

    [Fact]
    public void CreateVehicleServiceRecord_WithNegativeCost_ShouldThrowArgumentException()
    {
        var vehicleId = VehicleId.New();

        var act = () => VehicleServiceRecord.Create(
            vehicleId,
            DateTime.UtcNow.Date,
            10000,
            ServiceType.OilChange,
            "Oil Change",
            null,
            -100m);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*Cost cannot be negative*");
    }

    [Fact]
    public void VehicleSummary_Calculations_ShouldDetectServiceDueAndInspectionDue()
    {
        // Arrange
        var nextInsp = DateTime.UtcNow.Date.AddDays(20);
        var vehicle = Vehicle.Create(
            "VW", "Passat", 2019, "CF 44 221", FuelType.Diesel, 124500, null,
            serviceIntervalKm: 15000,
            serviceIntervalMonths: 12,
            nextInspectionDate: nextInsp);

        var lastServiceDate = DateTime.UtcNow.Date.AddMonths(-11);
        var serviceRecords = new List<VehicleServiceRecord>
        {
            VehicleServiceRecord.Create(
                vehicle.Id,
                lastServiceDate,
                110000,
                ServiceType.RegularService,
                "Last Oil Service",
                "QuickPoint",
                2500m)
        };

        var mileageLogs = new List<MileageLogEntry>
        {
            MileageLogEntry.Create(vehicle.Id, DateTime.UtcNow.Date, 124500)
        };

        // Act
        var summary = VehicleSummaryDto.Create(vehicle, mileageLogs, serviceRecords);

        // Assert
        summary.LatestMileageKm.Should().Be(124500);
        summary.KmUntilNextService.Should().Be(500); // 110,000 + 15,000 - 124,500 = 500 km remaining (within 1000km threshold)
        summary.ServiceDue.Should().BeTrue();
        summary.InspectionDue.Should().BeTrue(); // Inspection is in 20 days (<= 30 days threshold)
        summary.StatusAlerts.Should().Contain("ServiceDue");
        summary.StatusAlerts.Should().Contain("InspectionDue");
        summary.TotalMaintenanceCost.Should().Be(2500m);
        summary.TotalServiceRecordsCount.Should().Be(1);
        summary.TotalMileageLogsCount.Should().Be(1);
    }
}
