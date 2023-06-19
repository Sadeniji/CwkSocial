using CwkSocial.DAL.Configurations;
using CwkSocial.Domain.Aggregates.PostAggregate;
using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CwkSocial.DAL;

public class DataContext : IdentityDbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
        
    }
    // DbSet for an aggregates. Don't create DbSet for non aggregates, maybe you should remodel your domain model
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new PostCommentConfig());
        builder.ApplyConfiguration(new PostInteractionConfig());
        builder.ApplyConfiguration(new UserProfileConfig());
        builder.ApplyConfiguration(new IdentityUserLoginConfig());
        builder.ApplyConfiguration(new IdentityUserRoleConfig());
        builder.ApplyConfiguration(new IdentityUserTokenConfig());
    }
}