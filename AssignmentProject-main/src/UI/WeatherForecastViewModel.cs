using Assignment.Application.Common.Interfaces;
using Assignment.Application.Countries.Queries;
using Assignment.Application.TodoLists.Queries.GetTodos;
using Assignment.Domain.Entities;
using Caliburn.Micro;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.UI
{
    public class WeatherForecastViewModel : PropertyChangedBase
    {
        private readonly IWeatherForecastApi _weatherApi;
        private readonly ISender _sender;
        //Simple placeholder for now
        //Using cache for weather is good practice to avoid unnecessary calls
        private readonly ISimpleCache<int> _citiesTemperatureCache = null;


        private NotifyTaskCompletion<IList<CountryDto>> _countries;
        public NotifyTaskCompletion<IList<CountryDto>> Countries
        {
            get => _countries;
            set
            {
                _countries = value;
                NotifyOfPropertyChange(() => Countries);
            }
        }

        private CountryDto _selectedCountry;
        public CountryDto SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                _selectedCountry = value;
                Cities = _selectedCountry?.Cities ?? Enumerable.Empty<CityDto>();
            }
        }

        private IEnumerable<CityDto> _cities;
        public IEnumerable<CityDto> Cities
        {
            get => _cities;
            set
            {
                _cities = value;
                NotifyOfPropertyChange(() => Cities);
            }
        }

        private CityDto _selectedCity;
        public CityDto SelectedCity
        {
            get => _selectedCity;
            set
            {
                _selectedCity = value;
                if (_selectedCity != null)
                {
                    Temperature = new NotifyTaskCompletion<int>(LoadTemperature(_selectedCity.Name, _selectedCountry.Name));
                }
            }
        }

        private NotifyTaskCompletion<int> _temperature;
        public NotifyTaskCompletion<int> Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                NotifyOfPropertyChange(() => Temperature);
            }
        }

        public WeatherForecastViewModel(IWeatherForecastApi weatherService, ISender sender)
        {
            _weatherApi = weatherService;
            _sender = sender;
            RefreshCountries();
        }


        private void RefreshCountries()
        {
            var selectedCountryId = SelectedCountry?.Id;

            Countries = new NotifyTaskCompletion<IList<CountryDto>>(_sender.Send(new GetCountriesQuery()));

            if (selectedCountryId.HasValue && selectedCountryId.Value > 0)
            {
                SelectedCountry = Countries.Result.FirstOrDefault(c => c.Id == selectedCountryId.Value);
            }
        }

        private async Task<int> LoadTemperature(string city, string country)
        {
            var key = city + country;
            var cachedCityTemperature = await _citiesTemperatureCache.Get(key);
            if (cachedCityTemperature != default)
            {
                return cachedCityTemperature;
            }
            else
            {
                var temperature = await _weatherApi.GetTemperature(key, DateTime.Now);
                _citiesTemperatureCache.Set(key, temperature);
                return temperature;
            }
        }
    }
}
