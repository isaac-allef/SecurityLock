using Microsoft.Extensions.Caching.Memory;

namespace SecurityLock.KeyPair;

public sealed class MemoryCombinationLimiter : CombinationLimiter
{
    private IMemoryCache _memory;
    private const string SUFFIX_LINK = "link";

    public MemoryCombinationLimiter(string context = "")
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

    private DateTime ExpiresInToExpiration(TimeSpan expiresIn)
        => DateTime.UtcNow.Add(expiresIn);

    private string ParseKeyLink(string key)
        => _context+key+SUFFIX_LINK;
}
