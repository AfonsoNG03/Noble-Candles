using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Noble_Candles.Controllers
{
	/*
		id	INT (PK, AI)	Identificador único do pedido.
		user_id	string (FK)	Referência ao usuário que fez o pedido.
		total_price	DECIMAL(10, 2)	Valor total do pedido.
		status_id	INT (FK)	Status do pedido.
		created_at	DATETIME	Data do pedido.
		updated_at	DATETIME	Data da última atualização.
	 */

	public class UpdateOrderModel
	{

		[Required]
		public required decimal TotalPrice { get; set; }

		[Required]
		public required List<UpdateOrderItemModel> OrderItems { get; set; }
	}

	public class UpdateOrderItemModel
	{
		public int Id { get; set; } // Existing items will have an ID, new ones will not

		[Required]
		public required int CandleId { get; set; }

		[Required]
		public required int Quantity { get; set; }

		[Required]
		public required decimal Price { get; set; }
	}


	public class CreateOrderModel
	{
		[Required]
		public required decimal TotalPrice { get; set; }

		[Required]
		public required List<CreateOrderItemModel> OrderItems { get; set; }
	}

	public class CreateOrderItemModel
	{
		[Required]
		public required int CandleId { get; set; }

		[Required]
		public required int Quantity { get; set; }

		[Required]
		public required decimal Price { get; set; }
	}


	public static class OrderEndpoints
	{
		public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
		{
			//Orders
			app.MapGet("/Orders", GetOrders);

			//Orders by user
			app.MapGet("/Orders/User", GetOrdersByUser);

			//Single Order by user
			app.MapGet("/Orders/User/{id}", GetOrderByUser);

			//Orders by candle
			app.MapGet("/Orders/Status/{status}", GetOrdersByStatus);

			//Create Order
			app.MapPost("/Orders/Create", CreateOrder);

			//Delete Order
			app.MapDelete("/Orders/Delete/{id}", DeleteOrder);

			//Update Order
			app.MapPut("/Orders/Update/{id}", UpdateOrder);

			//Change Order Status
			app.MapPut("/Orders/Status/{id}", ChangeOrderStatus);

			return app;
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetOrders([FromServices] ApplicationDbContext dbContext)
		{

			var orders = await dbContext.Orders.ToListAsync<Order>();
			if (orders.Count != 0)
			{
				return Results.Ok(new { message = "Orders found", data = orders });
			}
			else
			{
				return Results.NotFound("No Orders found");
			}
		}

		private static async Task<IResult> GetOrdersByUser([FromServices] ApplicationDbContext dbContext, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			var orders = await dbContext.Orders
			.Where(o => o.UserId == userId)
			.Select(o => new
			{
				o.Id,
				o.TotalPrice,
				o.StatusId,
				o.CreatedAt,
				o.UpdatedAt
			})
			.ToListAsync();

			if (orders.Count == 0)
			{
				return Results.NotFound("No Orders found.");
			}

			return Results.Ok(orders);
		}

		private static async Task<Object> GetOrderByUser([FromServices] ApplicationDbContext dbContext, ClaimsPrincipal user, int id)
		{
			// Extract the currently logged-in user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			// Fetch only the logged-in user's Orders
			var order = await dbContext.Orders.FindAsync(id);

			if (order == null)
			{
				return Results.NotFound("No Orders found.");
			}

			var orderItems = await dbContext.OrderItems
				.Where(i => i.OrderId == order.Id)
				.ToListAsync();

			if (orderItems.Count == 0)
			{
				return Results.NotFound("No Order Items found.");
			}

			var result = new
			{
				order,
				orderItems
			};

			return Results.Ok(result);
		}


		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetOrdersByStatus([FromServices] ApplicationDbContext dbContext, int status)
		{
			var orders = await dbContext.Orders.Where(c => c.StatusId == status).ToListAsync<Order>();
			if (orders.Count != 0)
			{
				return Results.Ok(orders);
			}
			else
			{
				return Results.NotFound("No Orders found");
			}
		}

		private static async Task<IResult> CreateOrder([FromServices] ApplicationDbContext dbContext, [FromBody] CreateOrderModel createOrderModel, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			using var transaction = await dbContext.Database.BeginTransactionAsync();

			try
			{
				// Add the Order
				var newOrder = new Order
				{
					UserId = userId,
					TotalPrice = createOrderModel.TotalPrice,
					StatusId = 1, // Default status is "Pending"
				};

				dbContext.Orders.Add(newOrder);
				await dbContext.SaveChangesAsync();

				// Fetch inventory for all CandleIds in the order
				var candleIds = createOrderModel.OrderItems.Select(i => i.CandleId).Distinct().ToList();
				var inventories = await dbContext.Inventory.Where(i => candleIds.Contains(i.CandleId)).ToListAsync();

				// Validate inventory
				foreach (var item in createOrderModel.OrderItems)
				{
					var inventory = inventories.FirstOrDefault(i => i.CandleId == item.CandleId);
					if (inventory == null || inventory.Quantity < item.Quantity)
					{
						return Results.BadRequest($"Insufficient inventory for CandleId {item.CandleId}");
					}
				}

				// Update inventory and create OrderItems
				foreach (var item in createOrderModel.OrderItems)
				{
					var inventory = inventories.First(i => i.CandleId == item.CandleId);
					inventory.Quantity -= item.Quantity;

					dbContext.OrderItems.Add(new OrderItem
					{
						OrderId = newOrder.Id,
						CandleId = item.CandleId,
						Quantity = item.Quantity,
						Price = item.Price
					});
				}


				await dbContext.SaveChangesAsync();
				await transaction.CommitAsync();

				return Results.Created($"/Orders/{newOrder.Id}", newOrder);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Results.Problem($"An error occurred: {ex.Message}");
			}
		}



		private static async Task<IResult> DeleteOrder([FromServices] ApplicationDbContext dbContext, int id, ClaimsPrincipal user)
		{
			using var transaction = await dbContext.Database.BeginTransactionAsync();

			try
			{
				// Get the current user's ID
				var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
				if (userId == null)
				{
					return Results.Unauthorized();
				}

				// Find the order
				var order = await dbContext.Orders.FindAsync(id);
				if (order == null)
				{
					return Results.NotFound("Order not found.");
				}

				// Ensure the order belongs to the logged-in user
				if (order.UserId != userId)
				{
					return Results.Forbid();
				}

				// Find and remove associated order items
				var orderItems = await dbContext.OrderItems.Where(oi => oi.OrderId == id).ToListAsync();
				dbContext.OrderItems.RemoveRange(orderItems);

				// Remove the order
				dbContext.Orders.Remove(order);
				await dbContext.SaveChangesAsync();

				await transaction.CommitAsync();

				return Results.NoContent();
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Results.Problem($"An error occurred while deleting the order: {ex.Message}");
			}
		}



		//TO-DO - Update Order so that it's automatic when the payment is confirmed or it's delivered / sent
		private static async Task<IResult> UpdateOrder([FromServices] ApplicationDbContext dbContext, int id, [FromBody] UpdateOrderModel updateOrderModel, ClaimsPrincipal user)
		{
			if (!Validator.TryValidateObject(updateOrderModel, new ValidationContext(updateOrderModel), null, true))
			{
				return Results.BadRequest("Invalid data provided.");
			}

			using var transaction = await dbContext.Database.BeginTransactionAsync();

			// Get the current user's ID
			var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Results.Unauthorized();
			}

			try
			{
				// Find the order
				var order = await dbContext.Orders.FindAsync(id);
				if (order == null)
				{
					return Results.NotFound("Order not found.");
				}

				if (order.UserId != userId)
				{
					return Results.Forbid();
				}

				if (order.StatusId != 1) // Only pending orders can be updated
				{
					return Results.BadRequest("Only pending orders can be updated.");
				}

				order.TotalPrice = updateOrderModel.TotalPrice;
				order.UpdatedAt = DateTime.Now;

				// Update OrderItems
				var existingOrderItems = await dbContext.OrderItems.Where(oi => oi.OrderId == id).ToListAsync();
				var updatedOrderItems = updateOrderModel.OrderItems;

				// Handle removed items
				var removedItems = existingOrderItems
					.Where(ei => !updatedOrderItems.Any(ui => ui.Id == ei.Id))
					.ToList();

				dbContext.OrderItems.RemoveRange(removedItems);

				// Handle added or updated items
				foreach (var item in updatedOrderItems)
				{
					if (item.Id == 0) // New item
					{
						var inventory = await dbContext.Inventory.FirstOrDefaultAsync(i => i.CandleId == item.CandleId);
						if (inventory == null || inventory.Quantity < item.Quantity)
						{
							return Results.BadRequest($"Insufficient inventory for CandleId {item.CandleId}");
						}

						var newItem = new OrderItem
						{
							OrderId = id,
							CandleId = item.CandleId,
							Quantity = item.Quantity,
							Price = item.Price
						};

						dbContext.OrderItems.Add(newItem);
						inventory.Quantity -= item.Quantity;
					}
					else // Update existing item
					{
						var existingItem = existingOrderItems.FirstOrDefault(ei => ei.Id == item.Id);
						if (existingItem != null)
						{
							existingItem.Quantity = item.Quantity;
							existingItem.Price = item.Price;
						}
					}
				}

				await dbContext.SaveChangesAsync();
				await transaction.CommitAsync();

				return Results.Ok(order);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return Results.Problem($"An error occurred while updating the order: {ex.Message}");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> ChangeOrderStatus([FromServices] ApplicationDbContext dbContext, int id, int status)
		{
			var order = await dbContext.Orders.FindAsync(id);
			if (order == null)
			{
				return Results.NotFound("Order not found.");
			}

			order.StatusId = status;
			await dbContext.SaveChangesAsync();

			return Results.Ok(order);


		}
	}
}
