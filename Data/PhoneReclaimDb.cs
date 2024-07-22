using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhoneReclaim.Data.Models;
using PhoneReclaim.Models;

public class PhoneReclaimDb : IdentityDbContext<AppUser>
{
    public PhoneReclaimDb(DbContextOptions<PhoneReclaimDb> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; } 
     protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.AppUser)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.AppUserId)
                .IsRequired();
                


        modelBuilder.Entity<WishlistItem>()
            .HasKey(wi => new { wi.AppUserId, wi.ProductId });

        modelBuilder.Entity<WishlistItem>()
            .HasOne(wi => wi.AppUser)
            .WithMany(u => u.WishlistItems)
            .HasForeignKey(wi => wi.AppUserId);

        modelBuilder.Entity<WishlistItem>()
            .HasOne(wi => wi.Product)
            .WithMany(p => p.WishlistItems)
            .HasForeignKey(wi => wi.ProductId);
        }


}
       


// public class PhoneReclaimDb : DbContext

// {
//     public DbSet<Product> Products { get; set; }
//     public DbSet<AppUser> AppUsers { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         optionsBuilder.UseSqlite("Data Source=PhoneReclaim.db");
//     }
// }
