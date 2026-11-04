namespace FamilyManagement.Domain.ValueObjects;

/// <summary>
/// Represents clothing and shoe sizes for a child.
/// </summary>
/// <param name="ClothesSize">The current clothing size.</param>
/// <param name="ShoeSize">The current shoe size.</param>
/// <param name="HatSize">The current hat size.</param>
/// <param name="DiaperSize">The current diaper size.</param>
public record ChildWardrobeSizes(
    string? ClothesSize = null,
    string? ShoeSize = null,
    string? HatSize = null,
    string? DiaperSize = null)
{
    /// <summary>
    /// Gets an empty <see cref="ChildWardrobeSizes"/> instance where all sizes are unset.
    /// </summary>
    public static ChildWardrobeSizes Empty => new();
}
