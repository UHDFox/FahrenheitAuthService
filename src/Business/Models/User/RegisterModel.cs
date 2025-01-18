namespace Business.Models.User;

public sealed class RegisterModel
{
    public RegisterModel()
    {
    }
    public string Name { get; set; } = "";

    public string Password { get; set; } = "";

    public string Email { get; set; } = "";

    public string PhoneNumber { get; set; } = "";
}