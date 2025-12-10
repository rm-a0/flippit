using Flippit.Common.Enums;
using Flippit.Common.Models.User;

namespace Flippit.Web.App
{
    public class UserState
    {
        public UserDetailModel? User { get; set; }
        public event Action? OnChange;
        public void SetUser(UserDetailModel? user)
        {
            User = user;
            OnChange?.Invoke();
            
        }
    }
}
