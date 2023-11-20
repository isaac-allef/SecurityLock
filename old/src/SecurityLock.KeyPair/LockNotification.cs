namespace SecurityLock.KeyPair;

public sealed class LockNotification
{
    public List<string> Errors { get; private set; }
    public bool IsUnlocked { get => Errors.Count == 0; }

    public LockNotification()
    {
        Errors = new();
    }
    
    public void AddError(string error)
        => Errors.Add(error);
}
