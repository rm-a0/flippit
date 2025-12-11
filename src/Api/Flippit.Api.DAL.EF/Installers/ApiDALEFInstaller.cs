using Flippit.Api.DAL.Common.Installers;
using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Flippit.Api.DAL.EF;
using Scrutor;

namespace Flippit.Api.DAL.EF.Installers
{
    public class ApiDALEFInstaller : ApiDALInstallerBase
    {
        public void Install(IServiceCollection serviceCollection, string connectionString)
        {
            base.Install(serviceCollection);

            serviceCollection.AddDbContext<FlippitDbContext>(options => options.UseSqlServer(connectionString));

            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<ApiDALEFInstaller>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IApiRepository<>)))
                    .AsMatchingInterface()
                    .WithScopedLifetime());
        }

        public void InstallWithInMemory(IServiceCollection serviceCollection, string databaseName = "FlippitDataDb")
        {
            base.Install(serviceCollection);

            serviceCollection.AddDbContext<FlippitDbContext>(options => 
                options.UseInMemoryDatabase(databaseName));

            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<ApiDALEFInstaller>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IApiRepository<>)))
                    .AsMatchingInterface()
                    .WithScopedLifetime());
        }
    }
}
