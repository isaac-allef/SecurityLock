using Microsoft.AspNetCore.Mvc;
using SecurityLock.KeyPair;
using SecurityLock.KeyPair.AspNet;

namespace SecurityLock.Api.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private KeyPairLockEngine _engine;

        public TestController(KeyPairLockEngine engine)
        {
            _engine = engine;
        }

        [HttpGet]
        [Route("1")]
        public async Task<IActionResult> Test1(
            [FromHeader] string key1, 
            [FromHeader] string key2
        )
        {
            var result = _engine.TryUnlock((key1, key2));

            if (result.IsUnlocked.Equals(false))
            {
                return Unauthorized(result.Message);
            }

            return Ok("Show");
        }

        [HttpGet]
        [Route("2/{key1}")]
        [SecurityLockKeyPairNotification("engine1", "key1", "key3")]
        public async Task<IActionResult> Test2(
            [FromRoute] string key1, 
            [FromQuery] string key2,
            [FromHeader] string key3
        )
        {
            return Ok("Show");
        }
    }
}