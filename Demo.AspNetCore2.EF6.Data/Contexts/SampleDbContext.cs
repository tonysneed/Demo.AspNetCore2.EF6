using System.Data.Entity;
using Demo.AspNetCore2.EF6.Data.Models;

namespace Demo.AspNetCore2.EF6.Data.Contexts
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext(string connectionName) : base(connectionName)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}