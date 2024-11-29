using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Controllers
{

	/*
		id	INT (PK, AI)	Identificador único do inventario.
		CandleID	INT (FK)	Foreign key reference to Candle
		Quantity	INT		Stock quantity for this candle
		LastUpdatedAt	DATETIME	Data e hora da última atualização do inventário.
	 */

	public class InventoryCreateModel
	{
		[Required(ErrorMessage = "CategoryId is required.")]
		public required int CandleId { get; set; }

		[Required(ErrorMessage = "Quantity is required.")]
		public required int Quantity { get; set; }
	}

	public static class InventoryEndpoints
	{

		public static IEndpointRouteBuilder MapInventoriesEndpoints(this IEndpointRouteBuilder app)
		{
			//Inventories
			app.MapGet("/Inventories", GetInventories);

			//Inventory by id
			app.MapGet("/Inventories/{candleId}", GetInventoryInformation);

			//Update Inventory
			app.MapPut("/Inventories/Update", UpdateInventory);


			return app;
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetInventories([FromServices] ApplicationDbContext dbContext)
		{
			var Inventories = await dbContext.Inventory.ToListAsync<Inventory>();
			if (Inventories.Count != 0)
			{
				return Results.Ok(new { message = "Inventories found", data = Inventories });
			}
			else
			{
				return Results.NotFound("No Inventories found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetInventoryInformation([FromServices] ApplicationDbContext dbContext, int candleId)
		{
			var inventory = await dbContext.Inventory.FirstOrDefaultAsync(i => i.CandleId == candleId);

			if (inventory != null)
			{
				return Results.Ok(inventory);
			}
			else
			{
				return Results.NotFound("Inventory not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> UpdateInventory([FromServices] ApplicationDbContext dbContext, InventoryCreateModel inventoryCreateModel)
		{
			var inventoryToUpdate = await dbContext.Inventory.FirstOrDefaultAsync(i => i.CandleId == inventoryCreateModel.CandleId);

			// Check if the model is valid
			if (!Validator.TryValidateObject(inventoryCreateModel, new ValidationContext(inventoryCreateModel), null, true))
			{
				return Results.BadRequest("Invalid data provided. Please ensure all required fields are filled in correctly.");
			}

			if (inventoryToUpdate != null)
			{
				inventoryToUpdate.CandleId = inventoryCreateModel.CandleId;
				inventoryToUpdate.Quantity = inventoryCreateModel.Quantity;

				await dbContext.SaveChangesAsync();
				return Results.Ok("Inventory updated");
			}
			else
			{
				return Results.NotFound("Inventory not found");
			}
		}

	}
}
