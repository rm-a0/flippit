namespace Flippit.Api.App.Authorization;

public interface IResourceWithOwner
{
    Guid CreatorId { get; }
}
