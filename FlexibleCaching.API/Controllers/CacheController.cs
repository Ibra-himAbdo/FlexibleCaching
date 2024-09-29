using Microsoft.AspNetCore.Mvc;

namespace FlexibleCaching.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IFlexibleCacheService<string> _cacheService;

        public CacheController(IFlexibleCacheService<string> cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetCache([FromQuery] string key, [FromBody] string value)
        {
            await _cacheService.SetAsync(key, value, TimeSpan.FromMinutes(30));
            return Ok("Cached successfully");
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetCache([FromQuery] string key)
        {
            var cachedValue = await _cacheService.GetAsync(key);
            if (cachedValue == null)
                return NotFound("Cache not found.");
            return Ok(cachedValue);
        }

        [HttpDelete("clear/{key}")]
        public async Task<IActionResult> ClearCache(string key)
        {
            await _cacheService.RemoveAsync(key);
            return Ok("Cache cleared.");
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCache()
        {
            await _cacheService.ClearAsync();
            return Ok("Cache cleared.");
        }
    }
}
