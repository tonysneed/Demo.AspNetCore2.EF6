using Demo.AspNetCore2.EF6.Data.Models;

namespace Demo.AspNetCore2.EF6.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Demo.AspNetCore2.EF6.Data.Contexts.SampleDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Demo.AspNetCore2.EF6.Data.Contexts.SampleDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            context.Products.AddOrUpdate(
                new Product { Id = 1, ProductName = "Chai", UnitPrice = 10 },
                new Product { Id = 2, ProductName = "Chang", UnitPrice = 11 },
                new Product { Id = 3, ProductName = "Aniseed Syrup", UnitPrice = 12 }
            );
        }
    }
}
