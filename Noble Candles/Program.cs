using Noble_Candles.Controllers;
using Noble_Candles.Extensions;
using Noble_Candles.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerExplorer()
				.InjectDbContext(builder.Configuration)
				.AddAppConfig(builder.Configuration)
				.AddIdentityHandlersAndStores()
				.ConfigureIdentityOptions()	
				.AddIdentityAuth(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

app.ConfigureSwaggerExplorer()
	.ConfigureCORS(builder.Configuration)
	.AddIdentityAuthMiddlewares();

app.MapControllers();
app.MapGroup("/api")
	.MapIdentityApi<User>();
app.MapGroup("/api")
	.MapIdentityUserEndpoints()
	.MapAccountEndpoints();


app.UseHttpsRedirection();

app.Run();