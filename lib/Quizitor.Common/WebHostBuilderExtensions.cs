using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Quizitor.Common;

public static class WebHostBuilderExtensions
{
    extension(IWebHostBuilder builder)
    {
        public IWebHostBuilder AddGlobalCancellationToken()
        {
            return builder.ConfigureServices(services => services
                .AddSingleton<IGlobalCancellationTokenSource, GlobalCancellationTokenSource>());
        }
    }

    extension(IServiceCollection services)
    {
        public IServiceCollection AddJsonSerializer<TValue, TSerializer>()
            where TSerializer : class, IJsonSerializer<TValue>
        {
            return services.AddScoped<IJsonSerializer<TValue>, TSerializer>();
        }
    }
}