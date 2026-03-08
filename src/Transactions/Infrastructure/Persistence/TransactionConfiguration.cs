using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Transactions.Domain.Entities;

namespace Transactions.Infrastructure.Persistence
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions");

            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .HasColumnName("id");

            builder.HasIndex(t => t.IdempotencyKey).IsUnique();
            builder.Property(t => t.IdempotencyKey)
                .HasColumnName("idempotency_key")
                .IsRequired();

            builder.Property(t => t.FromUser)
                .HasColumnName("from_user")
                .IsRequired();

            builder.Property(t => t.FromAccount)
                .HasColumnName("from_account")
                .IsRequired();

            builder.OwnsOne(t => t.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("currency")
                    .HasConversion<string>()
                    .IsRequired();
            });

            builder.Property(t => t.DestinationType)
                .HasColumnName("destination_type")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.Destination)
                .HasColumnName("destination_value")
                .IsRequired();

            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(t => t.CompletedAt)
                .HasColumnName("completed_at");

            builder.Property(t => t.FailureReason)
                .HasColumnName("failure_reason");
        }
    }
}
