using Flippit.Common.Models.User;

namespace Flippit.Web.App
{
    public class UserState
    {
        public UserDetailModel? User { get; set; }

        public void SetUser(UserDetailModel? user)
        {
            this.User = user;
        }
    }
}
