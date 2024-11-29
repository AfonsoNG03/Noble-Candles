using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Noble_Candles.Controllers
{

	public class CandleCreateModel
	{
		[Required(ErrorMessage = "Name is required.")]
		public required string Name { get; set; }

		public string? Description { get; set; }

		[Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
		public decimal Price { get; set; }

		[Required(ErrorMessage = "CategoryId is required.")]
		public required int CategoryId { get; set; }

		[Required(ErrorMessage = "ColorId is required.")]
		public required int ColorId { get; set; }

		[Required(ErrorMessage = "FragranceId is required.")]
		public required int FragranceId { get; set; }
	}

	public static class CandlesEndpoints
	{
		public static IEndpointRouteBuilder MapCandlesEndpoints(this IEndpointRouteBuilder app)
		{
			//candles
			app.MapGet("/Candles", GetCandles);

			//candle by id
			app.MapGet("/Candles/{id}", GetCandleInformation);

			//candles by category
			app.MapGet("/Candles/Category/{category}", GetCandlesByCategory);

			//candles by color
			app.MapGet("/Candles/Color/{color}", GetCandlesByColor);

			//candles by fragrance
			app.MapGet("/Candles/Fragrance/{fragrance}", GetCandlesByFragrance);

			//Create candle
			app.MapPost("/Candles/Create", CreateCandle);

			//Delete candle
			app.MapDelete("/Candles/Delete/{id}", DeleteCandle);

			//Update candle
			app.MapPut("/Candles/Update/{id}", UpdateCandle);


			return app;
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCandles([FromServices] ApplicationDbContext dbContext)
		{

			var candles = await dbContext.Candles.ToListAsync<Candle>();
			if (candles.Count != 0)
			{
				return Results.Ok(new { message = "Candles found", data = candles });
			}
			else
			{
				return Results.NotFound("No candles found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCandleInformation([FromServices] ApplicationDbContext dbContext, int id)
		{
			var candle = await dbContext.Candles.FindAsync(id);
			if (candle != null)
			{
				return Results.Ok(candle);
			}
			else
			{
				return Results.NotFound("Candle not found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCandlesByCategory([FromServices] ApplicationDbContext dbContext, string category)
		{
			var candles = await dbContext.Candles.Where(c => c.Category.Name == category).ToListAsync<Candle>();
			if (candles.Count != 0)
			{
				return Results.Ok(candles);
			}
			else
			{
				return Results.NotFound("No candles found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCandlesByColor([FromServices] ApplicationDbContext dbContext, string color)
		{
			var candles = await dbContext.Candles.Where(c => c.Color.Name == color).ToListAsync<Candle>();
			if (candles.Count != 0)
			{
				return Results.Ok(candles);
			}
			else
			{
				return Results.NotFound("No candles found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCandlesByFragrance([FromServices] ApplicationDbContext dbContext, string fragrance)
		{
			var candles = await dbContext.Candles.Where(c => c.Fragrance.Name == fragrance).ToListAsync<Candle>();
			if (candles.Count != 0)
			{
				return Results.Ok(candles);
			}
			else
			{
				return Results.NotFound("No candles found");
			}
		}

		[Authorize(Roles="Admin")]
		private static async Task<IResult> CreateCandle([FromServices] ApplicationDbContext dbContext, [FromBody] CandleCreateModel candle)
		{
			// Check if the model is valid
			if (!Validator.TryValidateObject(candle, new ValidationContext(candle), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			// Ensure category, color, and fragrance exist
			var category = await dbContext.Categories.FindAsync(candle.CategoryId);
			var color = await dbContext.Colors.FindAsync(candle.ColorId);
			var fragrance = await dbContext.Fragrances.FindAsync(candle.FragranceId);

			if (category == null || color == null || fragrance == null)
			{
				return Results.BadRequest("Invalid Category, Color, or Fragrance ID.");
			}

			var newCandle = new Candle
			{
				Name = candle.Name,
				Description = candle.Description ?? string.Empty,
				Price = candle.Price,
				CategoryId = candle.CategoryId,
				ColorId = candle.ColorId,
				FragranceId = candle.FragranceId
			};

			dbContext.Candles.Add(newCandle);
			await dbContext.SaveChangesAsync();

			var inventory = new Inventory
			{
				CandleId = newCandle.Id,
				Quantity = 0
			};
			dbContext.Inventory.Add(inventory);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Candles/{newCandle.Id}", newCandle);
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> DeleteCandle([FromServices] ApplicationDbContext dbContext, int id)
		{
			var candle = await dbContext.Candles.FindAsync(id);
			if (candle != null)
			{
				dbContext.Candles.Remove(candle);
				await dbContext.SaveChangesAsync();

				var inventory = await dbContext.Inventory.Where(i => i.CandleId == id).FirstOrDefaultAsync();
				if (inventory != null) {
					dbContext.Inventory.Remove(inventory);
					await dbContext.SaveChangesAsync();
				}

				return Results.NoContent();
			}
			else
			{
				return Results.NotFound("Candle not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> UpdateCandle([FromServices] ApplicationDbContext dbContext, int id, [FromBody] CandleCreateModel candle)
		{
			// Check if the model is valid
			if (!Validator.TryValidateObject(candle, new ValidationContext(candle), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			// Ensure category, color, and fragrance exist
			var category = await dbContext.Categories.FindAsync(candle.CategoryId);
			var color = await dbContext.Colors.FindAsync(candle.ColorId);
			var fragrance = await dbContext.Fragrances.FindAsync(candle.FragranceId);
			if (category == null || color == null || fragrance == null)
			{
				return Results.BadRequest("Invalid Category, Color, or Fragrance ID.");
			}

			var candleToUpdate = await dbContext.Candles.FindAsync(id);
			if (candleToUpdate != null)
			{
				candleToUpdate.Name = candle.Name;
				candleToUpdate.Description = candle.Description ?? string.Empty;
				candleToUpdate.Price = candle.Price;
				candleToUpdate.CategoryId = candle.CategoryId;
				candleToUpdate.ColorId = candle.ColorId;
				candleToUpdate.FragranceId = candle.FragranceId;
				candleToUpdate.UpdatedAt = DateTime.Now;

				await dbContext.SaveChangesAsync();
				return Results.Ok(candleToUpdate);
			}
			else
			{
				return Results.NotFound("Candle not found.");
			}
		}
	}
}
