namespace SecurityLock.KeyPair;

public interface IKeysBucketLimitRepository
{
    /// <summary>
    /// Returns bucket limit and decrements limit
    /// </summary>
    public int? ConsumeBucketLimit(string key);
    public void SetBucketLimit(string key, int limit, TimeSpan expiresIn);
}
