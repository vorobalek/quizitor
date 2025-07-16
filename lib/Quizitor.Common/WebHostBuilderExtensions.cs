using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Quizitor.Common;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder AddGlobalCancellationToken(this IWebHostBuilder builder)
    {
        return builder.ConfigureServices(services => services
            .AddSingleton<IGlobalCancellationTokenSource, GlobalCancellationTokenSource>());
    }

    public static IServiceCollection AddJsonSerializer<TValue, TSerializer>(this IServiceCollection services)
        where TSerializer : class, IJsonSerializer<TValue>
    {
        return services.AddScoped<IJsonSerializer<TValue>, TSerializer>();
    }
}