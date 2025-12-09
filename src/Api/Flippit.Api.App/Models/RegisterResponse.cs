namespace Flippit.Api.App.Models;

public class RegisterResponse
{
    public required Guid UserId { get; set; }
    public required string UserName { get; set; }
}
