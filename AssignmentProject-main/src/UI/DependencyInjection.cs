using Assignment.Application.Common.Interfaces;
using Assignment.Infrastructure.API;
using Caliburn.Micro;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Assignment.UI;

public static class DependencyInjection
{
    public static IServiceCollection AddUIServices(this IServiceCollection services)
    {
        return services.AddTransient<IUser, CurrentUser>()
            .AddTransient<IWindowManager, WindowManager>()
            .AddTransient<MainViewModel>()
            .AddTransient<TodoManagmentViewModel>().AddTransient<IWeatherForecastApi, WeatherForecastApi>()
            .AddTransient<WeatherForecastViewModel>();
    }
}
