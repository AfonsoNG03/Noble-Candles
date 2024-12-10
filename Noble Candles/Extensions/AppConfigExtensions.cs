using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;

namespace Noble_Candles.Extensions
{
	public static class AppConfigExtensions
	{
		public static WebApplication ConfigureCORS(this WebApplication app, IConfiguration config)
		{
			// Apply the CORS policy
			app.UseCors("AllowFrontend"); // Matches the policy name defined below
			return app;
		}

		public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration config)
		{
			// Add CORS policy
			services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontend", policy =>
				{
					policy.WithOrigins("http://localhost:5173") // Frontend origin
						  .AllowAnyHeader() // Allow all headers
						  .AllowAnyMethod(); // Allow all HTTP methods
				});
			});

			// Add AppSettings configuration
			services.Configure<AppSettings>(config.GetSection("AppSettings"));
			return services;
		}
	}
}
