using CwkSocial.Api.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace CwkSocial.Api.Registrars;

public class MvcRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(config =>
        {
            config.Filters.Add(typeof(CwkSocialExceptionHandler));
        });
        
        builder.Services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
            config.ApiVersionReader = new UrlSegmentApiVersionReader();
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddVersionedApiExplorer(config =>
        {
            config.GroupNameFormat = "'v'VVV";
            config.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddEndpointsApiExplorer();
    }
}