using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;

namespace Noble_Candles.Controllers
{
	public static class StatusEndpoints
	{

		public static IEndpointRouteBuilder MapStatusesEndpoints(this IEndpointRouteBuilder app)
		{
			//Statuses
			app.MapGet("/Statuses", GetStatuses);

			//Status by id
			app.MapGet("/Statuses/{id}", GetStatusInformation);

			//Create Status
			app.MapPost("/Statuses/Create", CreateStatus);

			//Delete Status
			app.MapDelete("/Statuses/Delete/{id}", DeleteStatus);

			//Update Status
			app.MapPut("/Statuses/Update/{id}", UpdateStatus);


			return app;
		}

		[Authorize(Roles ="Admin")]
		private static async Task<IResult> GetStatuses([FromServices] ApplicationDbContext dbContext)
		{
			var Statuses = await dbContext.Statuses.ToListAsync<Status>();
			if (Statuses.Count != 0)
			{
				return Results.Ok(new { message = "Statuses found", data = Statuses });
			}
			else
			{
				return Results.NotFound("No Statuses found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetStatusInformation([FromServices] ApplicationDbContext dbContext, int id)
		{
			var Status = await dbContext.Statuses.FindAsync(id);
			if (Status != null)
			{
				return Results.Ok(Status);
			}
			else
			{
				return Results.NotFound("Status not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> CreateStatus([FromServices] ApplicationDbContext dbContext, string name)
		{

			if (name == null)
			{
				return Results.BadRequest("Invalid Status");
			}

			var Status = new Status
			{
				Name = name
			};

			await dbContext.Statuses.AddAsync(Status);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Statuses/{Status.Id}", Status);
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> DeleteStatus([FromServices] ApplicationDbContext dbContext, int id)
		{
			var Status = await dbContext.Statuses.FindAsync(id);
			if (Status != null)
			{
				dbContext.Statuses.Remove(Status);
				await dbContext.SaveChangesAsync();
				return Results.Ok("Status deleted");
			}
			else
			{
				return Results.NotFound("Status not found");
			}
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> UpdateStatus([FromServices] ApplicationDbContext dbContext, int id, string name)
		{
			var StatusToUpdate = await dbContext.Statuses.FindAsync(id);

			if (name == null)
			{
				return Results.BadRequest("Invalid Status");
			}

			if (StatusToUpdate != null)
			{
				StatusToUpdate.Name = name;

				await dbContext.SaveChangesAsync();
				return Results.Ok("Status updated");
			}
			else
			{
				return Results.NotFound("Status not found");
			}
		}

	}
}
