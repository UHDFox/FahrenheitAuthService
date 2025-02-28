namespace FahrenheitAuthService.Contracts.Contracts.User;

public sealed class RegisterRequest
{
    public RegisterRequest(string name, string password, string email, string phoneNumber)
    {
        Name = name;
        Password = password;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }
}