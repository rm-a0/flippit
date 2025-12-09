using Flippit.IdentityProvider.DAL.Entities;

namespace Flippit.IdentityProvider.DAL.Repositories;

public interface IAppUserRepository
{
    Task<IList<AppUserEntity>> SearchAsync(string searchString);
}
