using CwkSocial.DAL;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.Api.Registrars;

public class DbRegistrar : IWebApplicationBuilderRegistrar
{
    public void RegisterServices(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("CwkSocialConn");
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
    }
}