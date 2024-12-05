using DeliveryAPI.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace DeliveryAPI.Data
{
    public class DeliveryDbContext: DbContext
    {
        public DeliveryDbContext(DbContextOptions dbContextOptions): base(dbContextOptions)
        {
            
        }

        public DbSet<User> Users{ get; set; }

        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Rating> Rating { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=UNIVERSE77\\MSSQLSERVER01;Database=Dish;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
