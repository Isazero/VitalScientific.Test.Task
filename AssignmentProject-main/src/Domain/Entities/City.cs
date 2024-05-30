namespace Assignment.Domain.Entities;
public class City : BaseAuditableEntity
{
    public required string Name { get; set; }

    public int CountryId { get; set; }

    public Country Country { get; set; } = null!;
}
