namespace Flippit.Api.App.Models;

public class RegisterRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
}
