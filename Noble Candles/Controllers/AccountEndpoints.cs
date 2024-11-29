using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.Security.Claims;

namespace Noble_Candles.Controllers
{
	public static class AccountEndpoints
	{

		public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapGet("/UserProfile", GetUserProfile);

			app.MapGet("/Users", GetAllUsers);

			return app;
		}

		private static async Task<IResult> GetUserProfile(ClaimsPrincipal user, UserManager<User> userManager)
		{
			string? userID = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
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

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetAllUsers(UserManager<User> userManager)
		{
			var users = await userManager.Users
			.Select(user => new
			{
				user.UserName,
				user.Email,
			})
			.ToListAsync();

			return Results.Ok(users);
		}
	}
}
