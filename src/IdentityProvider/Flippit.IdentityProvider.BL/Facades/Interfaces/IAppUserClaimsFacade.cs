using Flippit.Common.BL.Facades;
using Flippit.IdentityProvider.BL.Models;

namespace Flippit.IdentityProvider.BL.Facades;

public interface IAppUserClaimsFacade : IAppFacade
{
    Task<IEnumerable<AppUserClaimListModel>> GetAppUserClaimsByUserIdAsync(Guid userId);
}
