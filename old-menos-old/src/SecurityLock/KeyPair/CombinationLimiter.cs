namespace SecurityLock.KeyPair;

public abstract class CombinationLimiter
{
    protected string _context = string.Empty;

    public CombinationLimiter(string context = "")
    {
        _context = context;
    }

    /// <summary>
    /// Returns keys linked to a given key
    /// </summary>
    public abstract ISet<string> GetLinkedKeys(string key);

    /// <summary>
    /// Link a key to another
    /// </summary>
    public abstract void LinkKeys(string key, string linkedKey, TimeSpan expiresIn);
}
