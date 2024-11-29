using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Noble_Candles.Controllers
{

	public static class FavoriteEndpoints
	{
		public static IEndpointRouteBuilder MapFavoritesEndpoints(this IEndpointRouteBuilder app)
		{
			//Favorites
			app.MapGet("/Favorites", GetFavorites);

			//Favorites by user
			app.MapGet("/Favorites/User/{user}", GetFavoritesByUser);

			//Favorites by candle
			app.MapGet("/Favorites/Candle/{candle}", GetFavoritesByCandle);

			//Create Favorite
			app.MapPost("/Favorites/Create", CreateFavorite);

			//Delete Favorite
			app.MapDelete("/Favorites/Delete/{id}", DeleteFavorite);

			return app;
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetFavorites([FromServices] ApplicationDbContext dbContext)
		{

			var favorites = await dbContext.Favorites.ToListAsync<Favorite>();
			if (favorites.Count != 0)
			{
				return Results.Ok(new { message = "Favorites found", data = favorites });
			}
			else
			{
				return Results.NotFound("No Favorites found");
			}
		}

		private static async Task<IResult> GetFavoritesByUser([FromServices] ApplicationDbContext dbContext, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			// Fetch only the logged-in user's favorites
			var favorites = await dbContext.Favorites.Where(f => f.UserId == userId).ToListAsync();
			if (favorites.Count == 0)
			{
				return Results.NotFound("No favorites found.");
			}

			return Results.Ok(favorites);
		}


		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetFavoritesByCandle([FromServices] ApplicationDbContext dbContext, int candle)
		{
			var favorites = await dbContext.Favorites.Where(c => c.CandleId == candle).ToListAsync<Favorite>();
			if (favorites.Count != 0)
			{
				return Results.Ok(favorites);
			}
			else
			{
				return Results.NotFound("No Favorites found");
			}
		}

		private static async Task<IResult> CreateFavorite([FromServices] ApplicationDbContext dbContext, [FromBody] int candleId, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			// Ensure the candle exists
			var candle = await dbContext.Candles.FindAsync(candleId);
			if (candle == null)
			{
				return Results.BadRequest("Invalid Candle ID.");
			}

			// Check if the favorite already exists
			var existingFavorite = await dbContext.Favorites
				.FirstOrDefaultAsync(f => f.UserId == userId && f.CandleId == candleId);

			if (existingFavorite != null)
			{
				return Results.BadRequest("This candle is already in your favorites.");
			}

			// Add the favorite
			var newFavorite = new Favorite
			{
				UserId = userId,
				CandleId = candleId,
			};

			dbContext.Favorites.Add(newFavorite);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Favorites/{newFavorite.Id}", newFavorite);
		}


		private static async Task<IResult> DeleteFavorite([FromServices] ApplicationDbContext dbContext, int id, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			// Find the favorite and ensure it belongs to the user
			var favorite = await dbContext.Favorites.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
			if (favorite == null)
			{
				return Results.NotFound("Favorite not found or you do not have permission to delete it.");
			}

			dbContext.Favorites.Remove(favorite);
			await dbContext.SaveChangesAsync();

			return Results.NoContent();
		}

	}
}
