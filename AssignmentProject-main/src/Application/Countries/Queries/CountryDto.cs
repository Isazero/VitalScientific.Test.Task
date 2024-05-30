using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assignment.Domain.Entities;

namespace Assignment.Application.Countries.Queries;
public class CountryDto
{
    public CountryDto()
    {
        Cities = Array.Empty<CityDto>();
    }

    public int Id { get; init; }

    public string? Name { get; init; }

    public IList<CityDto> Cities { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Country, CountryDto>();
        }
    }
}
