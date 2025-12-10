namespace Flippit.Common.Models.AuthModels;

public class LoginResponse
{
    public required string Token { get; set; }
    public required Guid UserId { get; set; }
    public required string UserName { get; set; }
    public required IList<string> Roles { get; set; }
}
