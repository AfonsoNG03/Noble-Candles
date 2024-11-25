using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;

namespace Noble_Candles.Extensions
{
	public static class AppConfigExtensions
	{

		public static WebApplication ConfigureCORS(this WebApplication app, IConfiguration config)
		{
			app.UseCors();

			return app;
		}

		public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration config)
		{
			services.Configure<AppSettings>(config.GetSection("AppSettings"));
			return services;
		}

	}
}
