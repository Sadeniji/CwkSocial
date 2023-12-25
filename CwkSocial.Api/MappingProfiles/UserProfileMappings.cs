using AutoMapper;
using CwkSocial.Api.Contracts.Posts.Responses;
using CwkSocial.Api.Contracts.UserProfiles.Requests;
using CwkSocial.Api.Contracts.UserProfiles.Responses;
using CwkSocial.Application.UserProfiles.Commands;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;

namespace CwkSocial.Api.MappingProfiles;

public class UserProfileMappings : Profile
{
    public UserProfileMappings()
    {
        CreateMap<UserProfileCreateUpdate, CreateUserCommand>();
        CreateMap<UserProfileCreateUpdate, UpdateUserProfileCommand>();
        CreateMap<UserProfile, UserProfileResponse>();
        CreateMap<BasicInfo, BasicInformation>();
        CreateMap<UserProfile, InteractionUser>()
            .ForMember(dest => dest.FullName, opt
            => opt.MapFrom(src 
            => src.BasicInfo.FirstName + " " + src.BasicInfo.LastName))
            .ForMember(dest => dest.City, opt
            => opt.MapFrom(src => src.BasicInfo.CurrentCity));
    }
}