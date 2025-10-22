using Flippit.Api.DAL.Common.Installers;
using Flippit.Api.DAL.Common.Repositories;
using Flippit.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Api.DAL.Memory.Installers
{
    public class ApiDALMemoryInstaller : ApiDALInstallerBase
    {
        public override void Install(IServiceCollection serviceCollection)
        {
            base.Install(serviceCollection);

            serviceCollection.AddSingleton<Storage>();

            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<ApiDALMemoryInstaller>()
                        .AddClasses(classes => classes.AssignableTo(typeof(IApiRepository<>)))
                            .AsMatchingInterface()
                            .WithScopedLifetime()); // changed Transient -> Scoped
        }
    }
}
