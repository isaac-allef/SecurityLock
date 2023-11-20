namespace SecurityLock.Key;

public sealed class RateLimiterLock : ILock<string>
{
    private RateLimiter _rateLimiter;
    private int _limit;
    private TimeSpan _period;

    public RateLimiterLock(RateLimiter rateLimiter, int limit, TimeSpan period)
    {
        _rateLimiter = rateLimiter;
        _limit = limit;
        _period = period;
    }

    public LockResponse TryUnlock(string key)
    {
        var limit = _rateLimiter.ConsumeLimit(key);

        if (limit is null)
        {
            _rateLimiter.SetLimit(key, _limit, expiresIn: _period);
            return LockResponse.Unlocked();
        }

        else if (limit > 0)
        {
            return LockResponse.Unlocked();
        }

        else
        {
            return LockResponse.Locked($"Rate limit exceeded to {key}");
        }
    }
}
