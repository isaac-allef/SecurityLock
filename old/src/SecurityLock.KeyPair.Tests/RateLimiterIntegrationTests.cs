using SecurityLock.KeyPair;

namespace SecurityLock.KeyPair.Tests;

public class RateLimiterIntegrationTests
{
    private Mock<IKeysBucketLimitRepository> _repo;
    private RateLimiter _sut;

    public RateLimiterIntegrationTests()
    {
        _repo = new Mock<IKeysBucketLimitRepository>();

        _sut = new RateLimiter(_repo.Object, limit: 1, period: TimeSpan.FromDays(1));
    }

    [Theory(DisplayName = "Should consume bucket limit correctly")]
    [InlineData("keyA", "_")]
    public void Test1(string keyA, string keyB)
    {
        var result = _sut.TryUnlock((keyA, keyB));

        _repo.Verify(r => r.ConsumeBucketLimit(keyA), Times.Once);
    }

    [Theory(DisplayName = "Should set bucket limit correctly if limit was null")]
    [InlineData("keyA", "_")]
    public void Test2(string keyA, string keyB)
    {
        _repo.Setup(r => r.ConsumeBucketLimit(It.IsAny<string>())).Returns(() => null);
        
        var result = _sut.TryUnlock((keyA, keyB));

        _repo.Verify(r => r.SetBucketLimit(keyA, 1, TimeSpan.FromDays(1)), Times.Once);
    }
}
