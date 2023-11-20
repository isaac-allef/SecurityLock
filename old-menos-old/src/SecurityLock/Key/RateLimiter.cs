namespace SecurityLock.Key;

public abstract class RateLimiter
{
    protected string _context = string.Empty;

    public RateLimiter(string context = "")
    {
        _context = context;
    }

    /// <summary>
    /// Returns bucket limit and decrements limit
    /// </summary>
    public abstract int? ConsumeLimit(string key);

    public abstract void SetLimit(string key, int limit, TimeSpan expiresIn);
}
