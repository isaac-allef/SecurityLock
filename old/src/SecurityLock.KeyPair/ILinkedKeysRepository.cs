namespace SecurityLock.KeyPair;

public interface ILinkedKeysRepository
{
    /// <summary>
    /// Returns keys linked to a given key
    /// </summary>
    public ISet<string> GetLinkedKeys(string key);

    /// <summary>
    /// Link a key to another
    /// </summary>
    public void LinkKeys(string key, string linkedKey, TimeSpan expiresIn);
}
