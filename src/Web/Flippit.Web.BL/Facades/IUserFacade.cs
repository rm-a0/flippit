using Flippit.Common.BL.Facades;
using Flippit.Common.Models.User;

namespace Flippit.Web.BL.Facades
{
    public interface IUserFacade : IAppFacade
    {
        Task<IList<UserListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<UserDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(UserDetailModel userModel);
        Task DeleteAsync(Guid id);
    }
}
