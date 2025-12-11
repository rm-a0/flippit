using Flippit.Common.Models;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models.User;

namespace Flippit.Web.BL.Facades
{
    public interface IUserFacade : IAppFacade
    {
        Task<IList<Flippit.Common.Models.User.UserListModel>> GetAllAsync(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        Task<Flippit.Common.Models.User.UserDetailModel?> GetByIdAsync(Guid id);
        Task<Guid> CreateOrUpdateAsync(Flippit.Common.Models.User.UserDetailModel userModel);
        Task DeleteAsync(Guid id);
        Task<(Flippit.Common.Models.AuthModels.LoginResponse?, string?)> RegisterLoginAsync(Flippit.Common.Models.AuthModels.RegisterRequest registerRequest);
        Task<(Flippit.Common.Models.AuthModels.LoginResponse?, string?)> LoginAsync(Flippit.Common.Models.AuthModels.LoginRequest loginRequest);
    }
}
