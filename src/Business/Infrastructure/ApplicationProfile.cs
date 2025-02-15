using AutoMapper;
using Business.Models.User;
using Contracts.Enums;
using Domain.Entities.Users;

namespace Business.Infrastructure;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        // User mappings
        CreateMap<UserModel, UserRecord>()
            .ConstructUsing(src =>
                new UserRecord(src.Name, src.Password, src.Email, src.PhoneNumber, UserRole.User));

        CreateMap<UserRecord, UserModel>()
            .ConstructUsing(src =>
                new UserModel(
                    src.Id,
                    src.Name,
                    src.PasswordHash,
                    src.Email,
                    src.PhoneNumber,
                    src.Role
                ));

        CreateMap<RegisterModel, UserModel>()
            .ConstructUsing(src =>
                new UserModel(
                    Guid.NewGuid(),
                    src.Name,
                    src.Password,
                    src.Email,
                    src.PhoneNumber,
                    UserRole.User
                ))
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}