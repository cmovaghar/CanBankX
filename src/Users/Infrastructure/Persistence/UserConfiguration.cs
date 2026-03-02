using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;

namespace Users.Infrastructure.Persistence
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.OwnsOne(u => u.Email, nav =>
            {
                nav.Property(e => e.Value)
                   .HasColumnName("Email")
                   .HasMaxLength(254)
                   .IsRequired();
                nav.HasIndex(e => e.Value).IsUnique();
            });

            builder.OwnsOne(u => u.Password, nav =>
            {
                nav.Property(p => p.Hash)
                   .HasColumnName("PasswordHash")
                   .HasMaxLength(512)
                   .IsRequired();
            });

            builder.OwnsOne(u => u.FullName, nav =>
            {
                nav.Property(n => n.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
                nav.Property(n => n.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
            });

            builder.OwnsOne(u => u.PhoneNumber, nav =>
            {
                nav.Property(p => p.Value).HasColumnName("PhoneNumber").HasMaxLength(20);
            });

            builder.OwnsOne(u => u.Address, nav =>
            {
                nav.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200).IsRequired();
                nav.Property(a => a.City).HasColumnName("City").HasMaxLength(100).IsRequired();
                nav.Property(a => a.Province).HasColumnName("Province").HasMaxLength(2).IsRequired();
                nav.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(6).IsRequired();
                nav.Property(a => a.Country).HasColumnName("Country").HasMaxLength(2).IsRequired();
            });

            builder.Property(u => u.Status)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(u => u.KycStatus)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(u => u.TwoFactorMethod)
                   .HasConversion<string>()
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(u => u.DateOfBirth).IsRequired();
            builder.Property(u => u.IsTwoFactorEnabled).IsRequired();
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt);

            builder.Ignore(u => u.DomainEvents);
        }
    }

}
