namespace Assignment.Domain.Entities;
public class Country: BaseAuditableEntity
{
    public required string Name { get; set; }

    public IList<City> Cities { get; set; } = [];
}
