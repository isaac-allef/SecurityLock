namespace SecurityLock;

public struct LockResponse
{
    public bool IsUnlocked { get; private set; }
    public string Message { get; private set; }

    private LockResponse(bool isUnlocked, string message)
    {
        IsUnlocked = isUnlocked;
        Message = message;
    }

    public static LockResponse Unlocked()
        => new LockResponse(true, string.Empty);
    
    public static LockResponse Locked(string message)
        => new LockResponse(false, message);
}
