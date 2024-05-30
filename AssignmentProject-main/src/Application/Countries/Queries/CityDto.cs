using Assignment.Domain.Entities;

namespace Assignment.Application.Countries.Queries;
public class CityDto
{
    public int Id { get; init; }

    public int CountryId { get; init; }

    public required string Name { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<City, CityDto>();
        }
    }
}
