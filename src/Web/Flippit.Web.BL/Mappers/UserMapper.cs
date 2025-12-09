using System.Collections.Generic;
using Flippit.Common.Models.User;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial UserListModel DetailToListModel(UserDetailModel detail);
        public partial IList<UserListModel> DetailToListModels(IEnumerable<UserDetailModel> details);
    }
}
