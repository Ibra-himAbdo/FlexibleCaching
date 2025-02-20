namespace FlexibleCaching.API.Services;

public class FlexibleCacheService<T> : IFlexibleCacheService<T>
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer? _redisConnection;
    private readonly HashSet<string> _memoryCacheKeys = [];
    private const string KeyPrefix = "AppCache:";

    public FlexibleCacheService(IDistributedCache cache, IConnectionMultiplexer? redisConnection = null)
    {
        _cache = cache;
        Cache = _cache;
        _redisConnection = redisConnection;
    }

    public IDistributedCache Cache { get; }

    public async Task SetAsync(string key, T value, TimeSpan? expiration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        };

        var serializedValue = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(GetKeyPrefix(key), serializedValue, options);

        if (_cache is MemoryDistributedCache)
        {
            _memoryCacheKeys.Add(GetKeyPrefix(key));
        }
    }

    public async Task<T?> GetAsync(string key)
    {
        var value = await _cache.GetStringAsync(GetKeyPrefix(key));
        return string.IsNullOrEmpty(value) ? default : JsonSerializer.Deserialize<T>(value);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(GetKeyPrefix(key));

        if (_cache is MemoryDistributedCache)
            _memoryCacheKeys.Remove(GetKeyPrefix(key));
    }

    public async Task ClearAsync()
    {
        switch (_cache)
        {
            case RedisCache when _redisConnection != null:
            {
                var redisDatabase = _redisConnection.GetDatabase();
                var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{KeyPrefix}*").ToArray();
                await redisDatabase.KeyDeleteAsync(keys);
                break;
            }
            case MemoryDistributedCache:
            {
                foreach (var key in _memoryCacheKeys)
                    await _cache.RemoveAsync(key);

                _memoryCacheKeys.Clear();
                break;
            }
        }
    }

    private static string GetKeyPrefix(string key) => $"{KeyPrefix}{key}";
}