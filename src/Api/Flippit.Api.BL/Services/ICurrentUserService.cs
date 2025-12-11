using System;

namespace Flippit.Api.BL.Services
{
    public interface ICurrentUserService
    {
        Guid? CurrentUserId { get; }
        bool IsInRole(string role);
    }
}
