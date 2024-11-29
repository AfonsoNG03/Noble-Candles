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
	.MapIdentityUserEndpoints()
	.MapAccountEndpoints()
	.MapCandlesEndpoints()
	.MapCategoriesEndpoints()
	.MapColorsEndpoints()
	.MapFavoritesEndpoints()
	.MapFragrancesEndpoints()
	.MapInventoriesEndpoints()
	.MapOrdersEndpoints()
	.MapOrderItemsEndpoints()
	.MapReviewsEndpoints()
	.MapStatusesEndpoints();
	


app.UseHttpsRedirection();

app.Run();