using Microsoft.Extensions.DependencyInjection;

namespace Flippit.Common.Installers
{
    public interface IInstaller
    {
        void Install(IServiceCollection serviceCollection);
    }
}
