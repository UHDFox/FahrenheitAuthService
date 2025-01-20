using Domain.Enums;

namespace Web.Contracts.User;

public sealed class CreateUserRequest
{
    public CreateUserRequest(string name, string password, string email, string phoneNumber, UserRole role)
    {
        Name = name;
        Password = password;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
    }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public UserRole Role { get; set; }
}