using Flippit.Api.DAL.Common.Mappers;
using Flippit.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Api.DAL.Common.Installers
{
    public abstract class ApiDALInstallerBase : IInstaller
    {
        public virtual void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<CardMapper>();
            serviceCollection.AddSingleton<CollectionMapper>();
            serviceCollection.AddSingleton<CompletedLessonMapper>();
            serviceCollection.AddSingleton<UserMapper>();
        }
    }
}
