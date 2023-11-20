namespace SecurityLock.KeyPair;

public abstract class KeysRepository : ILinkedKeysRepository, IKeysBucketLimitRepository
{
    protected string _context = string.Empty;

    public KeysRepository()
    {
    }

    public KeysRepository(string context)
    {
        _context = context;
    }

    public abstract int? ConsumeBucketLimit(string key);

    public abstract ISet<string> GetLinkedKeys(string key);

    public abstract void LinkKeys(string key, string linkedKey, TimeSpan expiresIn);

    public abstract void SetBucketLimit(string key, int limit, TimeSpan expiresIn);
}
