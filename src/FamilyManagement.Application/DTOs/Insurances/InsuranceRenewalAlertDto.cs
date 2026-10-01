namespace FamilyManagement.Application.DTOs.Insurances;

public record InsuranceRenewalAlertDto(
    Guid Id,
    string Insurer,
    string? PolicyNumber,
    string Category,
    string InsuredParty,
    DateTime RenewalDate,
    int DaysUntilRenewal,
    decimal EstimatedRenewalAmount,
    string Currency);
