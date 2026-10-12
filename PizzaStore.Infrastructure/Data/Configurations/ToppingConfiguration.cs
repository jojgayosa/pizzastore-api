using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Infrastructure.Data.Configurations
{
    public class ToppingConfiguration : IEntityTypeConfiguration<Topping>
    {
        public void Configure(EntityTypeBuilder<Topping> builder)
        {
            builder.ToTable("Toppings");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(t => t.Name).IsUnique();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt)
                .IsRequired();

            var navigation = builder.Metadata.FindNavigation(nameof(Topping.PizzaToppings));
            if (navigation != null)
            {
                navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            }
        }
    }
}
