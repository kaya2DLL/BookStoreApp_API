using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.CategoryId);//PrimaryKey
            builder.Property(x => x.CategoryName).IsRequired();

            builder.HasData(
                new Category()
                { CategoryName = "Computer Science", CategoryId = 2 },
                new Category()
                { CategoryName = "Netwoek", CategoryId = 3 },
                new Category()
                { CategoryName = "Tale", CategoryId = 4 }
            );
        }
    }
}
