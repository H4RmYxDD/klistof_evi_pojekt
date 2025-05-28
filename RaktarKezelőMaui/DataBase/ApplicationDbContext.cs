using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<PurchaseProduct> PurchaseProduct { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
                 => options.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=raktarkezeloMaui;Integrated Security=true");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithMany(p => p.Categories);
        }

    }

}
