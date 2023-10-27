using System.Reflection;
using CwkSocial.Application.UserProfiles.Queries;
using MediatR;

namespace CwkSocial.Api.Registrars;

public class BogardRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(typeof(Program), typeof(GetAllUserProfilesQuery));
        //builder.Services.AddMediatR(typeof(GetAllUserProfilesQuery));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetAllUserProfilesQuery).GetTypeInfo().Assembly));
    }
}