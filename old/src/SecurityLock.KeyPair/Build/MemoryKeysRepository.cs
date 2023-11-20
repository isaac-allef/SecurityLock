using Microsoft.Extensions.Caching.Memory;

namespace SecurityLock.KeyPair;

public sealed class MemoryKeysRepository : KeysRepository
{
    private IMemoryCache _memory;
    private const string SUFFIX_LINK = "link";
    private const string SUFFIX_BUCKET = "bucket";
    private const int CONSUMPTION_RATE = 1;

    public MemoryKeysRepository()
    {
        _memory = new MemoryCache(new MemoryCacheOptions());
    }

    public MemoryKeysRepository(string context)
    {
        _context = context;
        _memory = new MemoryCache(new MemoryCacheOptions());
    }

    public override ISet<string> GetLinkedKeys(string key)
    {
        var linkedKeys = GetNoExpiredLinkedKeys(key);
        return linkedKeys.Select(lk => lk.Item1).ToHashSet();
    }

    private ISet<(string, DateTime)> GetNoExpiredLinkedKeys(string key)
    {
        var linkedKeys = _memory.Get<ISet<(string, DateTime)>?>(ParseKeyLink(key));
        if (linkedKeys is null)
        {
            return new HashSet<(string, DateTime)>();
        }
        return linkedKeys.Where(lk => lk.Item2 > DateTime.UtcNow).ToHashSet();
    }

    public override void LinkKeys(string key, string linkedKey, TimeSpan expiresIn)
    {
        var expiration = ExpiresInToExpiration(expiresIn);
        var linkedKeys = GetNoExpiredLinkedKeys(key);

        linkedKeys.Add((linkedKey, expiration));
        _memory.Set<ISet<(string, DateTime)>>(ParseKeyLink(key), linkedKeys);
    }

    public override int? ConsumeBucketLimit(string key)
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

    public override void SetBucketLimit(string key, int limit, TimeSpan expiresIn)
    {
        SetBucketLimit(key, limit, ExpiresInToExpiration(expiresIn));
    }

    private void SetBucketLimit(string key, int limit, DateTime expiration)
    {
        _memory.Set<(int, DateTime)>(ParseKeyBucket(key), (limit, expiration));
    }

    private DateTime ExpiresInToExpiration(TimeSpan expiresIn)
        => DateTime.UtcNow.Add(expiresIn);

    private string ParseKeyLink(string key)
        => _context+key+SUFFIX_LINK;

    private string ParseKeyBucket(string key)
        => _context+key+SUFFIX_BUCKET;
}
