using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OcrSystemApi.Models;

namespace OcrSystemApi.DataAccess
{
    public class OcrDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public OcrDbContext(DbContextOptions<OcrDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<InvoiceImage> InvoiceImages { get; set; }
        public DbSet<OCRResult> OCRResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Quan trọng: Gọi base để Identity cấu hình các bảng của nó

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

            // Cấu hình ánh xạ cho bảng Users
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .HasColumnName("UserID");

            modelBuilder.Entity<User>()
                .Property(u => u.UserName)
                .HasColumnName("Username");

            // Đảm bảo Email và Username là duy nhất
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();

            // Cấu hình các mối quan hệ

            modelBuilder.Entity<InvoiceImage>()
                .HasOne(ii => ii.Users)
                .WithMany(i => i.InvoiceImages)
                .HasForeignKey(ii => ii.UserID);

            modelBuilder.Entity<OCRResult>()
                .HasOne(or => or.InvoiceImages)
                .WithMany(i => i.OCRResults)
                .HasForeignKey(or => or.ImageID);
        }
    }
}