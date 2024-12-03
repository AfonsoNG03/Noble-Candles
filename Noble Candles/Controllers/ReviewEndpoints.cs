using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Noble_Candles.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Noble_Candles.Controllers
{

	/*
		id	INT (PK, AI)	Identificador único da avaliação.
		user_id	string (FK)	Referência ao utilizador que avaliou.
		candle_id	INT (FK)	Referência à vela avaliada.
		rating	TINYINT	Nota de avaliação (1 a 5).
		comment	TEXT	Comentário da avaliação.
		created_at	DATETIME	Data da avaliação.
	    updated_at	DATETIME	Data da última atualização da avaliação.
	*/

	public class ReviewCreateModel
	{

		[Required(ErrorMessage = "CandleId is required.")]
		public required int CandleId { get; set; }

		[Required(ErrorMessage = "Rating is required.")]
		public required byte Rating { get; set; }

		public string? Comment { get; set; }
	}

	public static class ReviewEndpoints
	{

		public static IEndpointRouteBuilder MapReviewsEndpoints(this IEndpointRouteBuilder app)
		{
			// Get all Reviews
			app.MapGet("/Reviews", GetReviews);

			// Get OrderItems by CandleId
			app.MapGet("/Reviews/Candle/{candleId}", GetReviewsByCandle);

			// Get Review by id
			app.MapGet("/Reviews/{id}", GetReviewById);

			// Get Reviews by user
			app.MapGet("/Reviews/User", GetReviewsByUser);

			// Create Review
			app.MapPost("/Reviews/Create", CreateReview);

			// Update Review
			app.MapPut("/Reviews/Update/{id}", UpdateReview);

			// Delete Review
			app.MapDelete("/Reviews/Delete/{id}", DeleteReview);

			return app;
		}

		[Authorize(Roles = "Admin")]
		private static async Task<IResult> GetReviews([FromServices] ApplicationDbContext dbContext)
		{
			var reviews = await dbContext.Reviews.ToListAsync();
			return reviews.Count > 0
				? Results.Ok(new { message = "Reviews found", data = reviews })
				: Results.NotFound("No reviews found.");
		}

		[AllowAnonymous]
		private static async Task<IResult> GetReviewsByCandle([FromServices] ApplicationDbContext dbContext, int candleId)
		{
			var reviews = await dbContext.Reviews
				.Where(r => r.CandleId == candleId)
				.ToListAsync();

			return reviews.Count > 0
				? Results.Ok(new { message = "Reviews found", data = reviews })
				: Results.NotFound("No reviews found for this candle.");
		}

		[AllowAnonymous]
		private static async Task<IResult> GetReviewById([FromServices] ApplicationDbContext dbContext, int id)
		{
			var review = await dbContext.Reviews.FindAsync(id);
			return review != null
				? Results.Ok(review)
				: Results.NotFound("Review not found.");
		}

		private static async Task<IResult> GetReviewsByUser([FromServices] ApplicationDbContext dbContext, ClaimsPrincipal user)
		{

			// Extract the currently logged-in user's ID
			string? userID = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userID == null)
			{
				return Results.Unauthorized();
			}

			var reviews = await dbContext.Reviews
				.Where(r => r.UserId == userID)
				.ToListAsync();

			return reviews.Count > 0
				? Results.Ok(new { message = "Reviews found", data = reviews })
				: Results.NotFound("No reviews found for this user.");
		}

		private static async Task<IResult> CreateReview([FromServices] ApplicationDbContext dbContext, ReviewCreateModel reviewCreateModel, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			string? userID = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userID == null)
			{
				return Results.Unauthorized();
			}

			// Check if the user has completed an order containing the specified candle
			var hasPurchasedCandle = await dbContext.OrderItems
				.AnyAsync(oi => oi.Order.UserId == userID && oi.CandleId == reviewCreateModel.CandleId && oi.Order.StatusId != 1);


			if (!hasPurchasedCandle)
			{
				return Results.BadRequest("You must have purchased this candle to leave a review.");
			}

			// Create the review
			var review = new Review
			{
				UserId = userID,
				CandleId = reviewCreateModel.CandleId,
				Rating = reviewCreateModel.Rating,
				Comment = reviewCreateModel.Comment,
			};

			// Add the review to the database
			await dbContext.Reviews.AddAsync(review);
			await dbContext.SaveChangesAsync();

			return Results.Created($"/Reviews/{review.Id}", review);
		}


		private static async Task<IResult> UpdateReview([FromServices] ApplicationDbContext dbContext, int id, ReviewCreateModel reviewCreateModel, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			string? userID = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userID == null)
			{
				return Results.Unauthorized();
			}

			var review = await dbContext.Reviews.FindAsync(id);
			if (review == null)
			{
				return Results.NotFound("Review not found.");
			}

			if (review.UserId != userID)
			{
				return Results.Unauthorized();
			}

			review.CandleId = reviewCreateModel.CandleId;
			review.Rating = reviewCreateModel.Rating;
			review.Comment = reviewCreateModel.Comment;

			await dbContext.SaveChangesAsync();

			return Results.Ok(review);
		}

		private static async Task<IResult> DeleteReview([FromServices] ApplicationDbContext dbContext, int id, ClaimsPrincipal user)
		{
			// Extract the currently logged-in user's ID
			string? userID = user.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
			if (userID == null)
			{
				return Results.Unauthorized();
			}

			var review = await dbContext.Reviews.FindAsync(id);
			if (review == null)
			{
				return Results.NotFound("Review not found.");
			}

			if (review.UserId != userID)
			{
				return Results.Unauthorized();
			}

			dbContext.Reviews.Remove(review);
			await dbContext.SaveChangesAsync();

			return Results.Ok("Review deleted.");
		}

	}
}
