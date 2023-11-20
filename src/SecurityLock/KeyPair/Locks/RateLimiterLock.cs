namespace SecurityLock.KeyPair;

public sealed class RateLimiterLock
{
    private KeyValueAccess<(int limit, DateTime expiration)?> _keyValueAccess;
    private int _limit;
    private TimeSpan _period;
    private const int CONSUMPTION_RATE = 1;


    public RateLimiterLock(KeyValueAccess<(int, DateTime)?> keyValueAccess, int limit, TimeSpan period)
    {
        _keyValueAccess = keyValueAccess;
        _limit = limit;
        _period = period;
    }

    public async Task<LockResponse> TryUnlock((string partA, string partB) key)
    {
        var (firstPart, _) = key;
        var limit = await ConsumeLimit(firstPart);

        if (limit is null)
        {
            await SetLimit(firstPart, _limit, expiresIn: _period);
            return LockResponse.Unlocked();
        }

        else if (limit > 0)
        {
            return LockResponse.Unlocked();
        }

        else
        {
            return LockResponse.Locked($"Rate limit exceeded to {firstPart}");
        }
    }

    private async Task<int?> ConsumeLimit(string key)
    {
        var bucket = await _keyValueAccess.Get(key);
        if (bucket is null)
        {
            return null;
        }

        if (bucket.Value.expiration < DateTime.UtcNow)
        {
            return null;
        }

        if (bucket.Value.limit <= 0)
        {
            await Task.CompletedTask;
            return bucket.Value.limit;
        }

        SetBucketLimit(key, bucket.Value.limit - CONSUMPTION_RATE, bucket.Value.expiration);
        await Task.CompletedTask;
        return bucket.Value.limit - CONSUMPTION_RATE;
    }

    private async Task SetLimit(string key, int limit, TimeSpan expiresIn)
    {
        SetBucketLimit(key, limit, ExpiresInToExpiration(expiresIn));
        await Task.CompletedTask;
    }

    private async void SetBucketLimit(string key, int limit, DateTime expiration)
    {
        await _keyValueAccess.Set(key, (limit, expiration), TimeSpan.FromHours(3));
    }

    private DateTime ExpiresInToExpiration(TimeSpan expiresIn)
        => DateTime.UtcNow.Add(expiresIn);
}
