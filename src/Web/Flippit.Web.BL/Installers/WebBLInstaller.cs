using Flippit.Common.BL.Facades;
using Flippit.Common.Installers;
using Flippit.Web.BL.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Web.BL.Installers
{
    public class WebBLInstaller : IInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(selector =>
                selector.FromAssemblyOf<WebBLInstaller>()
                    .AddClasses(classes => classes.AssignableTo<IAppFacade>())
                    .AsSelfWithInterfaces()
                    .WithScopedLifetime());

            serviceCollection.AddSingleton<UserMapper>();
            serviceCollection.AddSingleton<CardMapper>();
            serviceCollection.AddSingleton<CollectionMapper>();
            serviceCollection.AddSingleton<CompletedLessonMapper>();
            serviceCollection.AddSingleton<ApiModelMapper>();
        }
    }
}
