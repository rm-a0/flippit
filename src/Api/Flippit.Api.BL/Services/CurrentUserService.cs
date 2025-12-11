using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Flippit.Api.BL.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid? _cachedUserId;
        private bool _userIdResolved;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid? CurrentUserId
        {
            get
            {
                if (_userIdResolved)
                {
                    return _cachedUserId;
                }

                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _cachedUserId = null;
                }
                else if (Guid.TryParse(userIdClaim, out var userId))
                {
                    _cachedUserId = userId;
                }
                else
                {
                    _cachedUserId = null;
                }

                _userIdResolved = true;
                return _cachedUserId;
            }
        }

        public bool IsInRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return false;
            }

            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}
