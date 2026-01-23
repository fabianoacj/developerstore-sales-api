using DeveloperStore.Domain.Repositories;
using DeveloperStore.Domain.Services;
using DeveloperStore.ORM.Caching;
using DeveloperStore.ORM.MongoDB.Configuration;
using DeveloperStore.ORM.MongoDB.Context;
using DeveloperStore.ORM.MongoDB.Repositories;
using DeveloperStore.ORM.PostgreSQL.Context;
using DeveloperStore.ORM.PostgreSQL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DeveloperStore.IoC.ModuleInitializers;

/// <summary>
/// Initializes infrastructure layer dependencies including database context and repositories.
/// </summary>
public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        // Register DbContext with PostgreSQL
        builder.Services.AddDbContext<DefaultContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("DeveloperStore.ORM")));

        // Register generic DbContext for flexibility
        builder.Services.AddScoped<DbContext>(provider => 
            provider.GetRequiredService<DefaultContext>());

        // Register PostgreSQL repositories
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();

        // Register MongoDB configuration and context
        var mongoSettings = builder.Configuration.GetSection("MongoDB").Get<MongoDbSettings>() 
            ?? new MongoDbSettings();
        builder.Services.AddSingleton(mongoSettings);
        builder.Services.AddSingleton<MongoDbContext>();

        // Register MongoDB event store
        builder.Services.AddScoped<IEventStore, MongoEventStore>();

        // Register Redis caching
        ConfigureRedis(builder);
    }

    /// <summary>
    /// Configures Redis caching services.
    /// </summary>
    private void ConfigureRedis(WebApplicationBuilder builder)
    {
        // Bind Redis configuration
        var redisConfig = builder.Configuration.GetSection(RedisConfiguration.SectionName).Get<RedisConfiguration>()
            ?? new RedisConfiguration();
        builder.Services.Configure<RedisConfiguration>(
            builder.Configuration.GetSection(RedisConfiguration.SectionName));

        // Only configure Redis if enabled
        if (redisConfig.Enabled && !string.IsNullOrEmpty(redisConfig.ConnectionString))
        {
            // Register Redis connection multiplexer as singleton
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(redisConfig.ConnectionString);
                configuration.AbortOnConnectFail = false; // Graceful degradation if Redis is down
                return ConnectionMultiplexer.Connect(configuration);
            });

            // Register cache service
            builder.Services.AddScoped<ICacheService, RedisCacheService>();
        }
        else
        {
            // Register a no-op cache service if Redis is disabled
            builder.Services.AddScoped<ICacheService, NullCacheService>();
        }
    }
}
