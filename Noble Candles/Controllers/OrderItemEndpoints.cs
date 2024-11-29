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
		id	INT (PK, AI)	Identificador único do item.
		order_id	INT (FK)	Referência ao pedido.
		candle_id	INT (FK)	Referência à vela comprada.
		quantity	INT	Quantidade comprada do item.
		price	DECIMAL(10, 2)	Preço do item no momento da compra.
	 */
	public static class OrderItemEndpoints
	{
		public static IEndpointRouteBuilder MapOrderItemsEndpoints(this IEndpointRouteBuilder app)
		{
			// Get all OrderItems
			app.MapGet("/OrderItems", GetOrderItems);

			// Get OrderItems by OrderId
			app.MapGet("/OrderItems/Order/{orderId}", GetOrderItemsByOrderId);

			return app;
		}

		// Get all OrderItems
		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetOrderItems([FromServices] ApplicationDbContext dbContext)
		{
			var orderItems = await dbContext.OrderItems.Include(oi => oi.Candle).ToListAsync();
			return orderItems.Count > 0
				? Results.Ok(new { message = "Order items found", data = orderItems })
				: Results.NotFound("No order items found.");
		}

		// Get OrderItems by OrderId
		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetOrderItemsByOrderId([FromServices] ApplicationDbContext dbContext, int orderId)
		{
			var orderItems = await dbContext.OrderItems
				.Where(oi => oi.OrderId == orderId)
				.Include(oi => oi.Candle)
				.ToListAsync();

			return orderItems.Count > 0
				? Results.Ok(orderItems)
				: Results.NotFound("No order items found for this order.");
		}
	}
}
