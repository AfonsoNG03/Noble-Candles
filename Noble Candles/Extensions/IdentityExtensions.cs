using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Noble_Candles.Models;
using System.Text;

namespace Noble_Candles.Extensions
{
	public static class IdentityExtensions
	{

		public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
		{
			services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<ApplicationDbContext>();
			return services;
		}

		public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
		{
			services.Configure<IdentityOptions>(options =>
			{
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 áàâäãåçéèêëíìîïñóòôöõúùûüýÿÁÀÂÄÃÅÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸ ^~´`";
				options.User.RequireUniqueEmail = true;
			});
			return services;
		}

		//Auth = Authentication + Authorization
		public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
		{
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(Y =>
			{
				Y.SaveToken = false;
				Y.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["AppSettings:JWTSecret"]!)),
					ValidateIssuer = false,
					ValidateAudience = false,
				};
			});
			services.AddAuthorization(options =>
			{
				options.FallbackPolicy = new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser()
					.Build();
			});
			return services;
		}

		public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
		{
			app.UseAuthentication();
			app.UseAuthorization();
			return app;
		}

	}
}
