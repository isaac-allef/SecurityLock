using SecurityLock.KeyPair;

namespace SecurityLock.KeyPair.Tests;

public class CombinationLimiterIntegrationTests
{
    private Mock<ILinkedKeysRepository> _repo;
    private CombinationLimiter _sut;

    public CombinationLimiterIntegrationTests()
    {
        _repo = new Mock<ILinkedKeysRepository>();

        _sut = new CombinationLimiter(_repo.Object, limit: 1, expiresIn: TimeSpan.FromDays(1));
    }

    [Theory(DisplayName = "Should get linked keys correctly")]
    [InlineData("keyA", "_")]
    public void Test1(string keyA, string keyB)
    {
        _repo.Setup(r => r.GetLinkedKeys(It.IsAny<string>())).Returns(new HashSet<string>());
        
        var result = _sut.TryUnlock((keyA, keyB));

        _repo.Verify(r => r.GetLinkedKeys(keyA), Times.Once);
    }

    [Theory(DisplayName = "Should set linked key correctly if was exceeded")]
    [InlineData("keyA", "_")]
    public void Test2(string keyA, string keyB)
    {
        _repo.Setup(r => r.GetLinkedKeys(It.IsAny<string>())).Returns(new HashSet<string>());
        
        var result = _sut.TryUnlock((keyA, keyB));

        _repo.Verify(r => r.LinkKeys(keyA, keyB, TimeSpan.FromDays(1)), Times.Once);
    }
}
