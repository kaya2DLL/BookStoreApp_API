using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Config
{

    public class bookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasData(
                new Book { Id = 1, Title = "Karagöz Hacivat", Price = 73,CategoryId=3 },
                new Book { Id = 2, Title = "Mesnevi", Price = 75, CategoryId = 2 },
                new Book { Id = 3, Title = "Devlet", Price = 71 , CategoryId = 3}
                );
        }
    }
}
