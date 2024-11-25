using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Noble_Candles.Models;
using System.Security.Claims;

namespace Noble_Candles.Controllers
{
	public static class AccountEndpoints
	{

		public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapGet("/UserProfile", GetUserProfile);

			return app;
		}

		[Authorize]
		private static async Task<IResult> GetUserProfile(ClaimsPrincipal user, UserManager<User> userManager)
		{
			string? userID = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			if (userID == null)
			{
				throw new ArgumentNullException(nameof(userID), "User ID cannot be null");
			}
			var userDetails = await userManager.FindByIdAsync(userID);

			return Results.Ok(
				new
				{
					Email = userDetails?.Email,
					PhoneNumber = userDetails?.PhoneNumber,
					Morada = userDetails?.Morada,
					UserName = userDetails?.UserName
				});
		}
	}
}
