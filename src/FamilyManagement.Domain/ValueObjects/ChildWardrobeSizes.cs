namespace FamilyManagement.Domain.ValueObjects;

public record ChildWardrobeSizes(
    string? ClothesSize = null,
    string? ShoeSize = null,
    string? HatSize = null,
    string? DiaperSize = null)
{
    public static ChildWardrobeSizes Empty => new();
}
