using Microsoft.AspNetCore.Authorization;

namespace Noble_Candles.Controllers
{
	public static class AuthorizationDemoEndpoints
	{

		public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapGet("/AdminOnly", AdminOnly);

			/*
			 app.MapGet("/Phonenumber", [Authorize(Policy = "HasPhoneNumber")] () =>
			{return "Phone number yes!!"; });
			 */

			return app;
		}


		[Authorize(Roles = "Admin")]
		private static string AdminOnly()
		{
			return "This is an admin only endpoint";
		}
	}
}
