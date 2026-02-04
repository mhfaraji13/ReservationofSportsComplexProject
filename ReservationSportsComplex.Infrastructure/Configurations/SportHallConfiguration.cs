using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationSportsComplex.Domain.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSportsComplex.Infrastructure.Configurations
{
    public class SportHallConfiguration : IEntityTypeConfiguration<SportHall>
    {
        public void Configure(EntityTypeBuilder<SportHall> builder)
        {
            builder.HasKey(x=>x.Id);

            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
