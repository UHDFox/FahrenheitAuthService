using AutoMapper;
using Business.Models.User;
using Contracts.Contracts.User;
using Contracts.Enums;

namespace Web.Infrastructure;

public class WebProfile : Profile
{
    public WebProfile()
    {
        CreateMap<UserModel, UserResponse>()
            .ConstructUsing(src => new UserResponse(
                src.Id,
                src.Name,
                src.PhoneNumber,
                src.Email,
                src.Password,
                (Contracts.Enums.UserRole)src.Role));

        CreateMap<CreateUserRequest, UserModel>()
            .ConstructUsing(src => new UserModel(
                Guid.NewGuid(),
                src.Name,
                src.Password,
                src.Email,
                src.PhoneNumber,
                (UserRole)src.Role
            ));


        CreateMap<UpdateUserRequest, UserModel>()
            .ConstructUsing(src =>
                new UserModel(
                    src.Id,
                    src.Name,
                    src.Password,
                    src.Email,
                    src.PhoneNumber,
                    (UserRole)src.Role
                )).ReverseMap();
        CreateMap<LoginRequest, LoginModel>();

        CreateMap<RegisterRequest, RegisterModel>();

        CreateMap<RegisterModel, UserModel>()
            .ConstructUsing(src =>
                new UserModel(
                    Guid.Empty,
                    src.Name,
                    src.Password,
                    src.Email,
                    src.PhoneNumber,
                    UserRole.User
                ));
    }
}