using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PizzaStore.Domain.Entities;

namespace PizzaStore.Infrastructure.Data.Configurations
{
    public class PizzaToppingConfiguration : IEntityTypeConfiguration<PizzaTopping>
    {
        public void Configure(EntityTypeBuilder<PizzaTopping> builder)
        {
            builder.ToTable("PizzaToppings");

            builder.HasKey(pt => new { pt.PizzaId, pt.ToppingId });

            builder.HasOne(pt => pt.Pizza)
                .WithMany(p => p.PizzaToppings)
                .HasForeignKey(pt => pt.PizzaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pt => pt.Topping)
                .WithMany(t => t.PizzaToppings)
                .HasForeignKey(pt => pt.ToppingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
