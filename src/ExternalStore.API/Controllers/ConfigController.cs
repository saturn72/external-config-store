using ExternalStore.Domain;
using ExternalStore.Services.Config;
using Microsoft.AspNetCore.Mvc;

namespace ExternalStore.API.Controllers
{
    [ApiController]
    [Route("config")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _service;

        public ConfigController(IConfigService service)
        {
            _service = service;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetConfigByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return BadRequest();

            var context = new GetConfigByKeyContext
            {
                ConfigKey = key,
            };
            await _service.GetConfigByKey(context);

            if (context.IsError)
                return BadRequest(context.UserError);

            return new JsonResult(context.Result);
        }

        [HttpGet("{key}/{allPaths}")]
        public async Task<IActionResult> GetConfigValueByPath(string key, string allPaths)
        {
            var paths = allPaths?
                .Split(",", StringSplitOptions.RemoveEmptyEntries)?
                .Select(s => s.Trim())
                .Where(str => str.HasValue())
                .ToArray();

            if (!key.HasValue() || paths.IsNullOrEmpty())
                return BadRequest();

            var context = new GetConfigByKeyAndPathContext
            {
                ConfigKey = key,
                Paths = paths,
            };

            await _service.GetConfigByKeyAndPath(context);

            if (context.IsError)
                return BadRequest(context.UserError);

            return new JsonResult(context.Result);
        }
    }
}