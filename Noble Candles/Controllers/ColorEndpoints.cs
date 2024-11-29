using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Controllers
{
	public static class ColorEndpoints
	{

		public static IEndpointRouteBuilder MapColorsEndpoints(this IEndpointRouteBuilder app)
		{
			//colors
			app.MapGet("/Colors", GetColors);

			//color by id
			app.MapGet("/Colors/{id}", GetColorInformation);

			//Create color
			app.MapPost("/Colors/Create", CreateColor);

			//Delete Color
			app.MapDelete("/Colors/Delete/{id}", DeleteColor);

			//Update Color
			app.MapPut("/Colors/Update/{id}", UpdateColor);


			return app;
		}

		[AllowAnonymous]
		private static async Task<IResult> GetColors([FromServices] ApplicationDbContext dbContext)
		{
			var colors = await dbContext.Colors.ToListAsync<Color>();
			if (colors.Count != 0)
			{
				return Results.Ok(new { message = "Colors found", data = colors });
			}
			else
			{
				return Results.NotFound("No colors found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetColorInformation([FromServices] ApplicationDbContext dbContext, int id)
		{
			var color = await dbContext.Colors.FindAsync(id);
			if (color != null)
			{
				return Results.Ok(color);
			}
			else
			{
				return Results.NotFound("Color not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> CreateColor([FromServices] ApplicationDbContext dbContext, string name)
		{

			if (name == null)
			{
				return Results.BadRequest("Invalid Color");
			}

			var color = new Color
			{
				Name = name
			};

			await dbContext.Colors.AddAsync(color);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Colors/{color.Id}", color);
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> DeleteColor([FromServices] ApplicationDbContext dbContext, int id)
		{
			var color = await dbContext.Colors.FindAsync(id);
			if (color != null)
			{
				dbContext.Colors.Remove(color);
				await dbContext.SaveChangesAsync();
				return Results.Ok("Color deleted");
			}
			else
			{
				return Results.NotFound("Color not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> UpdateColor([FromServices] ApplicationDbContext dbContext, int id, string name)
		{
			var colorToUpdate = await dbContext.Colors.FindAsync(id);

			if (name == null)
			{
				return Results.BadRequest("Invalid Color");
			}

			if (colorToUpdate != null)
			{
				colorToUpdate.Name = name;

				await dbContext.SaveChangesAsync();
				return Results.Ok("Color updated");
			}
			else
			{
				return Results.NotFound("Color not found");
			}
		}

	}
}
