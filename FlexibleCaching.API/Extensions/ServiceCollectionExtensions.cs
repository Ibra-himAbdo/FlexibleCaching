namespace FlexibleCaching.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFlexibleCaching(
        this IServiceCollection services, string? redisConnectionString)
    {
        services.AddSingleton(typeof(IFlexibleCacheService<>), typeof(FlexibleCacheService<>));

        if (string.IsNullOrEmpty(redisConnectionString))
        {
            Console.WriteLine("Redis connection string is null or empty. Falling back to in-memory caching.");
            services.AddDistributedMemoryCache();
            return services;
        }

        try
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });

            Log.Logger.Information("Successfully connected to Redis.");
        }
        catch (Exception)
        {
            Log.Logger.Warning("Failed to connect to Redis. Falling back to in-memory cache.");
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}
