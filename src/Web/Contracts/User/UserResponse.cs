using Domain.Enums;

namespace Web.Contracts.User;

public sealed class UserResponse
{
    public UserResponse(Guid id, string name, string phoneNumber, string email, string password, UserRole role)
    {
        Id = id;
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        Role = role;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Password{ get; set; }

    public string Email { get; set; }

    public UserRole Role { get; set; }
}