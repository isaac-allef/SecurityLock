namespace SecurityLock.KeyPair;

public sealed class RateLimiter : ILock
{
    private IKeysBucketLimitRepository _repo;
    private int _limit;
    private TimeSpan _period;

    public RateLimiter(IKeysBucketLimitRepository repo, int limit, TimeSpan period)
    {
        _repo = repo;
        _limit = limit;
        _period = period;
    }

    public LockResponse TryUnlock((string keyA, string keyB) keys)
    {
        var (keyA, _) = keys;

        var limit = _repo.ConsumeBucketLimit(keyA);

        if (limit is null)
        {
            _repo.SetBucketLimit(keyA, _limit, expiresIn: _period);
            return LockResponse.Unlocked();
        }

        else if (limit > 0)
        {
            return LockResponse.Unlocked();
        }

        else
        {
            return LockResponse.Locked($"Rate limit exceeded to {keyA}");
        }
    }
}
