namespace Flippit.Common.Models.AuthModels;

public class RegisterResponse
{
    public required Guid UserId { get; set; }
    public required string UserName { get; set; }
}
