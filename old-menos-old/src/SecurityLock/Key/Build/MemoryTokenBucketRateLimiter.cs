using Microsoft.Extensions.Caching.Memory;

namespace SecurityLock.Key;

public sealed class MemoryTokenBucketRateLimiter : RateLimiter
{
    private IMemoryCache _memory;
    private const string SUFFIX_BUCKET = "bucket";
    private const int CONSUMPTION_RATE = 1;

    public MemoryTokenBucketRateLimiter(string context = "")
    {
        _context = context;
        _memory = new MemoryCache(new MemoryCacheOptions());
    }

    public override int? ConsumeLimit(string key)
    {
        var bucket = _memory.Get<(int limit, DateTime expiration)?>(ParseKeyBucket(key));
        if (bucket is null)
        {
            return null;
        }

        if (bucket.Value.expiration < DateTime.UtcNow)
        {
            return null;
        }

        SetBucketLimit(key, bucket.Value.limit - CONSUMPTION_RATE, bucket.Value.expiration);
        return bucket.Value.limit;
    }

    public override void SetLimit(string key, int limit, TimeSpan expiresIn)
    {
        SetBucketLimit(key, limit, ExpiresInToExpiration(expiresIn));
    }

    private void SetBucketLimit(string key, int limit, DateTime expiration)
    {
        _memory.Set<(int, DateTime)>(ParseKeyBucket(key), (limit, expiration));
    }

    private DateTime ExpiresInToExpiration(TimeSpan expiresIn)
        => DateTime.UtcNow.Add(expiresIn);

    private string ParseKeyBucket(string key)
        => _context+key+SUFFIX_BUCKET;
}
