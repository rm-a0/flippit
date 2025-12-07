using Flippit.Common.Models.User;
using Flippit.IdentityProvider.BL.Models;
using Flippit.IdentityProvider.DAL.Entities;
using Riok.Mapperly.Abstractions;

namespace CookBook.IdentityProvider.BL.Mappers;

[Mapper]
public partial class AppUserMapper
{
    public partial AppUserEntity ToEntity(AppUserCreateModel appUserCreateModel);

    public partial UserListModel ToListModel(AppUserEntity appUserEntity);
    public partial List<UserListModel> ToListModels(IEnumerable<AppUserEntity> appUserEntities);

    public partial AppUserDetailModel ToDetailModel(AppUserEntity appUserEntity);
}
