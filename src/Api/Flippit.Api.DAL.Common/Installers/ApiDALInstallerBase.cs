using Flippit.Api.DAL.Common.Mappers;
using Flippit.Common.Installers;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Api.DAL.Common.Installers
{
    public abstract class ApiDALInstallerBase : IInstaller
    {
        public virtual void Install(IServiceCollection serviceCollection)
        {
            // AddSingleton -> AddScoped (supports potential future DB integration, safe if mappers hold state)
            serviceCollection.AddScoped<CardMapper>();
            serviceCollection.AddScoped<CollectionMapper>();
            serviceCollection.AddScoped<CompletedLessonMapper>();
            serviceCollection.AddSingleton<UserMapper>();
        }
    }
}
