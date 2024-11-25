using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;

namespace Noble_Candles.Extensions
{
	public static class EFCoreExtensions
	{
		//Dependency Injection
		public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration config)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(config.GetConnectionString("DevConnection")));
			return services;
		}
	}
}
