using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Infrastructure.Data.Configurations
{
    public class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
    {
        public void Configure(EntityTypeBuilder<Pizza> builder)
        {
            builder.ToTable("Pizzas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(p => p.Name).IsUnique();

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            // Use numeric precision appropriate for PostgreSQL (numeric(10,5))
            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(10, 5);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            // Map readonly collection backed by a field
            var navigation = builder.Metadata.FindNavigation(nameof(Pizza.PizzaToppings));
            if (navigation != null)
            {
                navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            }
        }
    }
}
