using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Noble_Candles.Models;
using NuGet.Configuration;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Noble_Candles.Controllers
{

	public class UserRegister
	{
		[Required]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public required string Email { get; set; }
		[Required]
		public required string Password { get; set; }
		[Required]
		public required string Morada { get; set; }

		[Required]
		[Phone(ErrorMessage = "Invalid phone number.")]
		public required string PhoneNumber { get; set; }
		[Required]
		public required string UserName { get; set; }

	}

	public class UserLogin
	{
		[Required]
		public required string Email { get; set; }
		[Required]
		public required string Password { get; set; }
	}

	public static class IdentityUserEndpoints
	{
		public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
		{
			app.MapPost("/signup", CreateUser);

			app.MapPost("/signin", SignIn);

			return app;
		}

		[AllowAnonymous]
		private static async Task<IResult> CreateUser(UserManager<User> userManager, [FromBody] UserRegister userRegisterModel)
		{
			// Validate model
			var validationContext = new ValidationContext(userRegisterModel);
			var validationResults = new List<ValidationResult>();

			if (!Validator.TryValidateObject(userRegisterModel, validationContext, validationResults, true))
			{
				return Results.BadRequest(new { Errors = validationResults.Select(v => v.ErrorMessage) });
			}

			// Create user
			User user = new()
			{
				Email = userRegisterModel.Email,
				UserName = userRegisterModel.UserName,
				Morada = userRegisterModel.Morada,
				PhoneNumber = userRegisterModel.PhoneNumber
			};
			var result = await userManager.CreateAsync(user, userRegisterModel.Password);
			await userManager.AddToRoleAsync(user, "User");

			if (result.Succeeded)
				return Results.Ok(result);
			else
				return Results.BadRequest(result);
		}

		[AllowAnonymous]
		private static async Task<IResult> SignIn(UserManager<User> userManager, [FromBody] UserLogin loginModel, IOptions<AppSettings> appSettings)
		{
			// Validate model
			var validationContext = new ValidationContext(loginModel);
			var validationResults = new List<ValidationResult>();

			if (!Validator.TryValidateObject(loginModel, validationContext, validationResults, true))
			{
				return Results.BadRequest(new { Errors = validationResults.Select(v => v.ErrorMessage) });
			}

			var user = await userManager.FindByEmailAsync(loginModel.Email);

			if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
			{
				var roles = await userManager.GetRolesAsync(user);
				var SignInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));

				ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
				{
					new Claim("UserID", user.Id.ToString()),
					new Claim("UserName", user.UserName!.ToString()),
					new Claim(ClaimTypes.Role, roles.First())
				});

				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = claims,
					Expires = DateTime.UtcNow.AddMinutes(60),
					SigningCredentials = new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256Signature)
				};

				var tokenHandler = new JwtSecurityTokenHandler();
				var securityToken = tokenHandler.CreateToken(tokenDescriptor);
				var token = tokenHandler.WriteToken(securityToken);
				return Results.Ok(new { token });
			}
			else
				return Results.BadRequest(new { message = "Username or password are incorrect" });
		}
	}
}
