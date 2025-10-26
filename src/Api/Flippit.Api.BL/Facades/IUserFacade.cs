using System;
using System.Collections.Generic;
using Flippit.Common.BL.Facades;
using Flippit.Common.Models;
using Flippit.Common.Models.Card;
using Flippit.Common.Models.User;

namespace Flippit.Api.BL.Facades
{
    public interface IUserFacade : IAppFacade
    {
        IList<UserListModel> GetAll(string? filter = null, string? sortBy = null, int page = 1, int pageSize = 10);
        UserDetailModel? GetById(Guid id);
        IList<UserListModel> Search(string searchText);
        Guid CreateOrUpdate(UserDetailModel userModel);
        Guid Create(UserDetailModel userModel);
        Guid? Update(UserDetailModel userModel);
        void Delete(Guid id);
    }
}
