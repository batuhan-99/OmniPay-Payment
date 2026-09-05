using Microsoft.EntityFrameworkCore;
using OmniPay.Domain.Entities;
using OmniPay.Domain.Enums;

namespace OmniPay.Infrastructure.Persistence;

public sealed class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasKey(payment => payment.Id);

            entity.Property(payment => payment.Id)
                .HasColumnName("id");

            entity.Property(payment => payment.MerchantId)
                .HasColumnName("merchant_id")
                .IsRequired();

            entity.Property(payment => payment.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(payment => payment.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(payment => payment.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(payment => payment.Description)
                .HasColumnName("description")
                .HasMaxLength(500);

            entity.Property(payment => payment.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Ignore(payment => payment.Merchant);
        });
    }
}
