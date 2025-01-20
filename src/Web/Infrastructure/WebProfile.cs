using AutoMapper;
using Business.Models.User;
using Domain.Enums;
using Web.Contracts.User;
using LoginRequest = Web.Contracts.User.LoginRequest;
using RegisterRequest = Web.Contracts.User.RegisterRequest;

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
                src.Role));

        CreateMap<CreateUserRequest, UserModel>()
            .ConstructUsing(src => new UserModel(
                Guid.NewGuid(),
                src.Name,
                src.Password,
                src.Email,
                src.PhoneNumber,
                src.Role
            ));


        CreateMap<UpdateUserRequest, UserModel>()
            .ConstructUsing(src =>
                new UserModel(
                    src.Id,
                    src.Name,
                    src.Password,
                    src.Email,
                    src.PhoneNumber,
                    src.Role
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