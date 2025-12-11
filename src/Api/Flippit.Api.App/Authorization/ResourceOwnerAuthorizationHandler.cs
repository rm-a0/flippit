using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Flippit.Api.App.Authorization;

public class ResourceOwnerAuthorizationHandler : AuthorizationHandler<ResourceOwnerRequirement, IResourceWithOwner>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnerRequirement requirement,
        IResourceWithOwner resource)
    {
        if (context.User == null)
        {
            return Task.CompletedTask;
        }

        // Admins can access any resource
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Check if user is the owner
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
        {
            if (resource.CreatorId == userId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
