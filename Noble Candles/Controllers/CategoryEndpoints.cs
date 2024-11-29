using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Controllers
{

	/*
		id	INT (PK, AI)	Identificador único da categoria.
		name	VARCHAR(100)	Nome da categoria.
		description	TEXT	Descrição da categoria.
	 */

	public class CategoryCreateModel
	{
		[Required(ErrorMessage = "Name is required.")]
		[MaxLength(100)]
		public required string Name { get; set; }

		[Required(ErrorMessage = "Description is required.")]
		public required string Description { get; set; }
	}

	public static class CategoryEndpoints
	{

		public static IEndpointRouteBuilder MapCategoriesEndpoints(this IEndpointRouteBuilder app)
		{
			//categories
			app.MapGet("/Categories", GetCategories);

			//category by id
			app.MapGet("/Categories/{id}", GetCategoryInformation);

			//Create category
			app.MapPost("/Categories/Create", CreateCategory);

			//Delete category
			app.MapDelete("/Categories/Delete/{id}", DeleteCategory);

			//Update category
			app.MapPut("/Categories/Update/{id}", UpdateCategory);


			return app;
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCategories([FromServices] ApplicationDbContext dbContext)
		{
			var categories = await dbContext.Categories.ToListAsync<Category>();
			if (categories.Count != 0)
			{
				return Results.Ok(new { message = "Categories found", data = categories });
			}
			else
			{
				return Results.NotFound("No categories found");
			}
		}

		[AllowAnonymous]
		private static async Task<IResult> GetCategoryInformation([FromServices] ApplicationDbContext dbContext, int id)
		{
			var category = await dbContext.Categories.FindAsync(id);
			if (category != null)
			{
				return Results.Ok(category);
			}
			else
			{
				return Results.NotFound("Category not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> CreateCategory([FromServices] ApplicationDbContext dbContext, CategoryCreateModel categoryCreateModel)
		{
			// Check if the model is valid
			if (!Validator.TryValidateObject(categoryCreateModel, new ValidationContext(categoryCreateModel), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			if (categoryCreateModel == null)
			{
				return Results.BadRequest("Invalid category");
			}

			var category = new Category
			{
				Name = categoryCreateModel.Name,
				Description = categoryCreateModel.Description
			};


			await dbContext.Categories.AddAsync(category);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Categories/{category.Id}", category);
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> DeleteCategory([FromServices] ApplicationDbContext dbContext, int id)
		{
			var category = await dbContext.Categories.FindAsync(id);
			if (category != null)
			{
				dbContext.Categories.Remove(category);
				await dbContext.SaveChangesAsync();
				return Results.Ok("Category deleted");
			}
			else
			{
				return Results.NotFound("Category not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> UpdateCategory([FromServices] ApplicationDbContext dbContext, int id, CategoryCreateModel categoryCreateModel)
		{
			var categoryToUpdate = await dbContext.Categories.FindAsync(id);

			// Check if the model is valid
			if (!Validator.TryValidateObject(categoryCreateModel, new ValidationContext(categoryCreateModel), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			if (categoryToUpdate != null)
			{
				categoryToUpdate.Name = categoryCreateModel.Name;
				categoryToUpdate.Description = categoryCreateModel.Description;

				await dbContext.SaveChangesAsync();
				return Results.Ok("Category updated");
			}
			else
			{
				return Results.NotFound("Category not found");
			}
		}

	}
}
