using CookBook.IdentityProvider.BL.Mappers;
using Flippit.Common.BL.Facades;
using Flippit.Common.Installers;
using Flippit.IdentityProvider.BL.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.IdentityProvider.BL.Installers;

public class IdentityProviderBLInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<AppUserClaimMapper>();
        serviceCollection.AddSingleton<AppUserMapper>();

        serviceCollection.Scan(selector =>
            selector.FromAssemblyOf<IdentityProviderBLInstaller>()
                .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
    }
}
