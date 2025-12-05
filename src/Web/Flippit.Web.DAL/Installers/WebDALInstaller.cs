using Flippit.Common.Installers;
using Scrutor;
using Flippit.Web.DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Web.DAL.Installers
{
    public class WebDALInstaller : IInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<LocalDb>();
            serviceCollection.Scan(scan =>
                scan.FromAssemblyOf<WebDALInstaller>()
                    .AddClasses(classes => classes.AssignableTo(typeof(IWebRepository<>)))
                    .AsSelf()
                    .WithSingletonLifetime());
        }
    }
}
