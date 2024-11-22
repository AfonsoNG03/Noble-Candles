using Microsoft.EntityFrameworkCore;

namespace Noble_Candles.Models
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
		public DbSet<Candle> Candles { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Color> Colors { get; set; }
		public DbSet<Favorite> Favorites { get; set; }
		public DbSet<Fragrance> Fragrances { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<Status> Statuses { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }

	}
}
