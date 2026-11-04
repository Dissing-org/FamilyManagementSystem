namespace FamilyManagement.Application.DTOs.Insurances;

/// <summary>
/// Data transfer object representing an upcoming insurance policy renewal alert.
/// </summary>
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
