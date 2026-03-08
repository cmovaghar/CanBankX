using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("id");

            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("email")
                    .HasMaxLength(256)
                    .IsRequired();
                email.HasIndex(e => e.Value).IsUnique();
            });

            builder.OwnsOne(u => u.Password, password =>
            {
                password.Property(p => p.Hash)
                    .HasColumnName("password_hash")
                    .HasMaxLength(512)
                    .IsRequired();
            });

            builder.OwnsOne(u => u.FullName, name =>
            {
                name.Property(n => n.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
                name.Property(n => n.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
            });

            builder.OwnsOne(u => u.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value).HasColumnName("phone_number").HasMaxLength(20);
            });

            builder.OwnsOne(u => u.Address, address =>
            {
                address.Property(a => a.Street).HasColumnName("address_street").HasMaxLength(200).IsRequired();
                address.Property(a => a.City).HasColumnName("address_city").HasMaxLength(100).IsRequired();
                address.Property(a => a.Province).HasColumnName("address_province").HasMaxLength(2).IsRequired();
                address.Property(a => a.PostalCode).HasColumnName("address_postal_code").HasMaxLength(6).IsRequired();
                address.Property(a => a.Country).HasColumnName("address_country").HasMaxLength(2).IsRequired();
            });

            builder.OwnsOne(u => u.Sin, sin =>
            {
                sin.Property(p => p.Encrypted)
                    .HasColumnName("encrypted_sin")
                    .HasMaxLength(512)
                    .IsRequired();
            });

            builder.Property(u => u.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(u => u.TwoFactorMethod)
                .HasColumnName("two_factor_method")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(u => u.IsTwoFactorEnabled)
                .HasColumnName("is_two_factor_enabled")
                .IsRequired();

            builder.Property(u => u.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
            builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");

            builder.Property(u => u.InteracDepositAccount).HasColumnName("interact_deposit_account");

            builder.Ignore(u => u.DomainEvents);
        }
    }

}
