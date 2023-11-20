namespace SecurityLock.KeyPair;

public sealed class CombinationLimiterLock
{
    private KeyValueAccess<ISet<(string, DateTime)>?> _keyValueAccess;
    private int _limit;
    private TimeSpan _expiresIn;

    public CombinationLimiterLock(KeyValueAccess<ISet<(string, DateTime)>?> keyValueAccess, int limit, TimeSpan expiresIn)
    {
        _keyValueAccess = keyValueAccess;
        _limit = limit;
        _expiresIn = expiresIn;
    }

    public async Task<LockResponse> TryUnlock((string partA, string partB) key)
    {
        var (partA, partB) = key;
        var linkedKyes = await GetLinkedKeys(partA);

        if (linkedKyes.Contains(partB))
        {
            return LockResponse.Unlocked();
        }
        
        else if (linkedKyes.Count < _limit)
        {
            await LinkKeys(partA, partB, _expiresIn);
            return LockResponse.Unlocked();
        }

        else
        {
            return LockResponse.Locked($"{partA} has already reached his combination limit");
        }
    }

    public async Task<ISet<string>> GetLinkedKeys(string key)
    {
        var linkedKeys = await GetNoExpiredLinkedKeys(key);
        var result = linkedKeys.Select(lk => lk.Item1).ToHashSet();
        return result;
    }

    private async Task<ISet<(string, DateTime)>> GetNoExpiredLinkedKeys(string key)
    {
        var linkedKeys = await _keyValueAccess.Get(key);
        if (linkedKeys is null)
        {
            return new HashSet<(string, DateTime)>();
        }
        return linkedKeys.Where(lk => lk.Item2 > DateTime.UtcNow).ToHashSet();
    }

    public async Task LinkKeys(string key, string linkedKey, TimeSpan expiresIn)
    {
        var expiration = ExpiresInToExpiration(expiresIn);
        var linkedKeys = await GetNoExpiredLinkedKeys(key);

        linkedKeys.Add((linkedKey, expiration));
        await _keyValueAccess.Set(key, linkedKeys, TimeSpan.FromHours(3));
    }

    private DateTime ExpiresInToExpiration(TimeSpan expiresIn)
        => DateTime.UtcNow.Add(expiresIn);
}
