using Contracts.Enums;

namespace Business.Models.User;

public sealed class UserModel
{
    public UserModel(Guid id, string name, string password, string email, string phoneNumber, UserRole role)
    {
        Id = id;
        Name = name;
        Password = password;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public UserRole Role { get; set; }
}