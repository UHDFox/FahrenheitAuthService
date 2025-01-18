using Domain.Enums;

namespace Web.Contracts.User;

public sealed class UpdateUserRequest
{
    public UpdateUserRequest(string name, string phoneNumber, string password, string email, UserRole role)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        Password = password;
        Email = email;
        Role = role;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public UserRole Role { get; set; }
}