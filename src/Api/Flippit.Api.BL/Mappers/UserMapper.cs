using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flippit.Api.DAL.Common.Entities;
using Flippit.Common.Models.User;
using Riok.Mapperly.Abstractions;

namespace Flippit.Api.BL.Mappers
{
    [Mapper]
    public partial class UserMapper
    {
        public partial UserEntity ModelToEntity(UserDetailModel model);
        public partial UserDetailModel ToDetailModel(UserEntity model);
        public partial IList<UserListModel> ToListModels(IEnumerable<UserEntity> entities);
        public partial UserListModel ToListModel(UserEntity entity);
    }
}
