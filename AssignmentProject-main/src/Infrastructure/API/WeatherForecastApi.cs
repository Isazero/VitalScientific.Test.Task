using Assignment.Application.Common.Interfaces;

namespace Assignment.Infrastructure.API;
public class WeatherForecastApi : IWeatherForecastApi
{
    public async Task<int> GetTemperature(string cityName, DateTime time)
    {
        return await Task.FromResult(new Random().Next(-10, 35));
    }
}
