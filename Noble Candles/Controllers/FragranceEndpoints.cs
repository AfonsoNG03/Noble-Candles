using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Controllers
{

	/*
		id	INT (PK, AI)	Identificador único do aroma.
		name	VARCHAR(100)	Nome do aroma.
		description	TEXT	Descrição do aroma.
	 */

	public class FragranceCreateModel
	{
		[Required(ErrorMessage = "Name is required.")]
		[MaxLength(100)]
		public required string Name { get; set; }

		[Required(ErrorMessage = "Description is required.")]
		public required string Description { get; set; }
	}

	public static class FragranceEndpoints
	{

		public static IEndpointRouteBuilder MapFragrancesEndpoints(this IEndpointRouteBuilder app)
		{
			//Fragrances
			app.MapGet("/Fragrances", GetFragrances);

			//Fragrance by id
			app.MapGet("/Fragrances/{id}", GetFragranceInformation);

			//Create Fragrance
			app.MapPost("/Fragrances/Create", CreateFragrance);

			//Delete Fragrance
			app.MapDelete("/Fragrances/Delete/{id}", DeleteFragrance);

			//Update Fragrance
			app.MapPut("/Fragrances/Update/{id}", UpdateFragrance);


			return app;
		}

		[AllowAnonymous]
		private static async Task<IResult> GetFragrances([FromServices] ApplicationDbContext dbContext)
		{
			var fragrances = await dbContext.Fragrances.ToListAsync<Fragrance>();
			if (fragrances.Count != 0)
			{
				return Results.Ok(new { message = "Fragrances found", data = fragrances });
			}
			else
			{
				return Results.NotFound("No Fragrances found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetFragranceInformation([FromServices] ApplicationDbContext dbContext, int id)
		{
			var fragrance = await dbContext.Fragrances.FindAsync(id);
			if (fragrance != null)
			{
				return Results.Ok(fragrance);
			}
			else
			{
				return Results.NotFound("Fragrance not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> CreateFragrance([FromServices] ApplicationDbContext dbContext, FragranceCreateModel fragranceCreateModel)
		{
			// Check if the model is valid
			if (!Validator.TryValidateObject(fragranceCreateModel, new ValidationContext(fragranceCreateModel), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			if (fragranceCreateModel == null)
			{
				return Results.BadRequest("Invalid Fragrance");
			}

			var fragrance = new Fragrance
			{
				Name = fragranceCreateModel.Name,
				Description = fragranceCreateModel.Description
			};


			await dbContext.Fragrances.AddAsync(fragrance);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Fragrances/{fragrance.Id}", fragrance);
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> DeleteFragrance([FromServices] ApplicationDbContext dbContext, int id)
		{
			var fragrance = await dbContext.Fragrances.FindAsync(id);
			if (fragrance != null)
			{
				dbContext.Fragrances.Remove(fragrance);
				await dbContext.SaveChangesAsync();
				return Results.Ok("Fragrance deleted");
			}
			else
			{
				return Results.NotFound("Fragrance not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> UpdateFragrance([FromServices] ApplicationDbContext dbContext, int id, FragranceCreateModel fragranceCreateModel)
		{
			var fragranceToUpdate = await dbContext.Fragrances.FindAsync(id);

			// Check if the model is valid
			if (!Validator.TryValidateObject(fragranceCreateModel, new ValidationContext(fragranceCreateModel), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			if (fragranceToUpdate != null)
			{
				fragranceToUpdate.Name = fragranceCreateModel.Name;
				fragranceToUpdate.Description = fragranceCreateModel.Description;

				await dbContext.SaveChangesAsync();
				return Results.Ok("Fragrance updated");
			}
			else
			{
				return Results.NotFound("Fragrance not found");
			}
		}

	}
}
