# Flexible Caching

This project provides an implementation of a flexible caching service using either Redis or in-memory cache based on the availability of a Redis connection string. It uses the `StackExchange.Redis` and `Microsoft.Extensions.Caching` libraries to offer a unified caching solution.

## Features

- **Redis Caching**: If a Redis connection is available, the service will use Redis for distributed caching.
- **In-Memory Caching**: If Redis is unavailable, it will fall back to in-memory caching, suitable for local environments or small-scale applications.
- **Generic Caching Service**: The caching service is implemented as a generic interface `IFlexibleCacheService<T>` allowing for any type of objects to be cached.
- **Auto Key Management**: Automatically adds a prefix to cache keys to avoid key collisions.
- **Key Expiration**: Allows setting expiration times for cache entries.

---

## Installation

### Prerequisites
- [.NET 8](https://dotnet.microsoft.com/download)
- Redis server (if you want to use Redis caching)

### Steps

1. **Clone the Repository**

    ```bash
    git clone https://github.com/yourusername/FlexibleCaching.git
    cd FlexibleCaching
    ```

2. **Restore NuGet Packages**

    Navigate to the project directory and restore the required packages:

    ```bash
    dotnet restore
    ```

3. **Configure Redis (Optional)**

    If you plan to use Redis for caching, make sure you have Redis running locally or hosted elsewhere. Update your `appsettings.json` with the connection string to your Redis server:

    ```json
    {
      "ConnectionStrings": {
        "Redis": "localhost" // or any other Redis host address
      }
    }
    ```

---

## How to Use

### 1. Adding the Flexible Caching Service

In your **Program.cs**, register the Flexible Caching service based on whether Redis is available:

```csharp
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddFlexibleCaching(redisConnectionString);
```

This code ensures that the app uses Redis caching if the connection string is provided, otherwise it falls back to in-memory caching.

### 2. Using the Flexible Cache Service in Your Application

Inject the `IFlexibleCacheService<T>` into any service or controller where you need caching functionality:

```csharp
public class MyService
{
    private readonly IFlexibleCacheService<MyDataType> _cacheService;

    public MyService(IFlexibleCacheService<MyDataType> cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task CacheDataAsync(MyDataType data)
    {
        string cacheKey = "MyCacheKey";
        await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromMinutes(30));
    }

    public async Task<MyDataType?> GetCachedDataAsync(string cacheKey)
    {
        return await _cacheService.GetAsync(cacheKey);
    }
}
```

### 3. Clearing Cache

You can also clear all cached data or remove specific cache entries:

```csharp
public async Task ClearCacheAsync()
{
    await _cacheService.ClearAsync();
}

public async Task RemoveSpecificCacheEntry(string key)
{
    await _cacheService.RemoveAsync(key);
}
```

---

## Service API Details

### Caching Methods

- `Task SetAsync(string key, T value, TimeSpan? expiration)`: Set a value in the cache with an optional expiration.
- `Task<T?> GetAsync(string key)`: Get a value from the cache.
- `Task RemoveAsync(string key)`: Remove a specific key from the cache.
- `Task ClearAsync()`: Clear all cache entries (Redis or In-memory).

### Service Extension

Use the `AddFlexibleCaching` extension method to add the caching service to the `IServiceCollection`.

```csharp
public static IServiceCollection AddFlexibleCaching(
    this IServiceCollection services, string? redisConnectionString)
```

- **Parameter**: 
  - `redisConnectionString`: Connection string for Redis. If this is null or empty, in-memory caching is used.

---

## Example API Usage

### Adding Caching to a Controller

In your controller, you can use the caching service to cache responses as in **CacheController**


---

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
