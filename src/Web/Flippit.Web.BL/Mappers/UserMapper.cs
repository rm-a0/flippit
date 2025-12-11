using System.Collections.Generic;
using Flippit.Common.Models.User;
using Riok.Mapperly.Abstractions;

namespace Flippit.Web.BL.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial Flippit.Common.Models.User.UserListModel DetailToListModel(Flippit.Common.Models.User.UserDetailModel detail);
        public partial IList<Flippit.Common.Models.User.UserListModel> DetailToListModels(IEnumerable<Flippit.Common.Models.User.UserDetailModel> details);
    }
}
