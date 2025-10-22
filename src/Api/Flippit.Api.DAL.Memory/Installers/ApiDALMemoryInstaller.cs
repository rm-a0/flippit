using Flippit.Api.DAL.Common.Mappers;
using Flippit.Api.DAL.Memory.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Api.DAL.Memory.Installers
{
    public class ApiDALMemoryInstaller
    {
        public void Install(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<Storage>();
            serviceCollection.AddTransient<CardRepository>();
            serviceCollection.AddTransient<UserRepository>();
            serviceCollection.AddTransient<CollectionRepository>();
            serviceCollection.AddTransient<CompletedLessonRepository>();
            serviceCollection.AddTransient<CardMapper>();
            serviceCollection.AddTransient<UserMapper>();
            serviceCollection.AddTransient<CollectionMapper>();
            serviceCollection.AddTransient<CompletedLessonMapper>();
        }
    }
}
