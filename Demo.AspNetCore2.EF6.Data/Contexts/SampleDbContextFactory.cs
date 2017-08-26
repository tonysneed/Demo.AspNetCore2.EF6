using System.Data.Entity.Infrastructure;

namespace Demo.AspNetCore2.EF6.Data.Contexts
{
    class SampleDbContextFactory : IDbContextFactory<SampleDbContext>
    {
        public SampleDbContext Create()
        {
            return new SampleDbContext(@"Server=(localdb)\mssqllocaldb;Database=SampleDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
