namespace SecurityLock.KeyPair.AspNet;

[AttributeUsage(AttributeTargets.Method)]
public class SecurityLockKeyPairAttribute : Attribute
{
    public string Context { get; } = string.Empty;
    public string KeyAFieldName { get; private set; }
    public string KeyBFieldName { get; private set; }

    public SecurityLockKeyPairAttribute(string keyAFieldName, string keyBFieldName)
        => (KeyAFieldName, KeyBFieldName) = (keyAFieldName, keyBFieldName);

    public SecurityLockKeyPairAttribute(string context, string keyAFieldName, string keyBFieldName)
        => (Context, KeyAFieldName, KeyBFieldName) = (context, keyAFieldName, keyBFieldName);
}
