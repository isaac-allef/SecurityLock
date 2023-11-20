using Microsoft.AspNetCore.Mvc;
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
            var result = await _engine.TryUnlockAndNotify((key1, key2));

            if (result.IsUnlocked.Equals(false))
            {
                return Unauthorized(result.Errors);
            }

            return Ok("Show");
        }

        [HttpGet]
        [Route("2")]
        [SecurityLockKeyPairAttribute("engine1", "key1", "key2")]
        public async Task<IActionResult> Test2(
            [FromHeader] string key1, 
            [FromHeader] string key2
        )
        {
            return Ok("Show");
        }
    }
}