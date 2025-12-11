using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Flippit.Api.DAL.EF.Factories
{
    public class FlippitDbContextFactory : IDesignTimeDbContextFactory<FlippitDbContext>
    {
        public FlippitDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<FlippitDbContextFactory>(optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<FlippitDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            return new FlippitDbContext(optionsBuilder.Options);
        }
    }
}
