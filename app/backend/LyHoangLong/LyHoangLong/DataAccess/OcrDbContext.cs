using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OcrSystem.Models;

namespace OcrSystem.DataAccess
{
    public class OcrDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public OcrDbContext(DbContextOptions<OcrDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceImage> InvoiceImages { get; set; }
        public DbSet<OCRResult> OCRResults { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Quan trọng: Gọi base để Identity cấu hình các bảng của nó

            // Đổi tên các bảng của Identity để phù hợp với cấu trúc của bạn
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
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.UserID);

            modelBuilder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceItems)
                .HasForeignKey(ii => ii.InvoiceID);

            modelBuilder.Entity<InvoiceImage>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.InvoiceImages)
                .HasForeignKey(ii => ii.InvoiceID);

            modelBuilder.Entity<OCRResult>()
                .HasOne(or => or.Invoice)
                .WithMany(i => i.OCRResults)
                .HasForeignKey(or => or.InvoiceID);
        }
    }
}