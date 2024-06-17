using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Data
{
    public class DbContextFixture : IDisposable
    {
        public AppDbContext Context { get; private set; }

        public DbContextFixture()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new AppDbContext(options);
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
