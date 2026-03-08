using Accounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Accounts.Infrastructure.Persistence
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .HasColumnName("id");

            builder.Property(a => a.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(a => a.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.NickName)
                .HasColumnName("nickname")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.OpenedAt)
                .HasColumnName("opened_at")
                .IsRequired();

            builder.Property(a => a.ClosedAt)
                .HasColumnName("closed_at")
                .IsRequired(false);

            builder.OwnsOne(a => a.Balance, mb =>
            {
                mb.Property(m => m.Amount)
                    .HasColumnName("balance_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();

                mb.Property(m => m.Currency)
                    .HasColumnName("balance_currency")
                    .HasConversion<string>()
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Ignore(u => u.DomainEvents);
        }
    }
}
