using Flippit.Common.BL.Facades;
using Flippit.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Api.BL.Installers
{
    public class ApiBLInstaller : IInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<ApiBLInstaller>()
                        .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                        .AsSelfWithInterfaces()
                        .WithScopedLifetime());
        }
    }
}
