namespace SecurityLock.KeyPair;

public abstract class KeyValueAccess<T>
{
    protected string _context = string.Empty;

    public KeyValueAccess(string context)
    {
        _context = context;
    }

    protected string AddContext(string key)
        => _context+"-"+key;

    public abstract Task<T?> Get(string key);

    public abstract Task Set(string key, T value, TimeSpan expiresIn);
}
