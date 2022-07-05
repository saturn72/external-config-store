using ExternalStore.API.Models;
using ExternalStore.Domain;
using ExternalStore.Services.Config;
using ExternalStore.Services.Subscription;
using Microsoft.AspNetCore.Mvc;

namespace ExternalStore.API.Controllers
{
    [ApiController]
    [Route("config")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfigService _configService;
        private readonly ISubscriptionService _subscriptionService;

        public ConfigController(
            IConfigService configService,
            ISubscriptionService subscriptionService)
        {
            _configService = configService;
            _subscriptionService = subscriptionService;
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
            await _configService.GetConfigByKey(context);

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

            await _configService.GetConfigByKeyAndPath(context);

            if (context.IsError)
                return BadRequest(context.UserError);

            return new JsonResult(context.Result);
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] IEnumerable<SubscriptionRequestModel> model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var clientId = User.FindFirst("client-id");
            if (clientId == null)
                return BadRequest();

            var requests = model.Select(s => new SubscriptionToPathRequest
            {
                ConfigKey = s.ConfigKey,
                Path = s.Path,
                Priority = s.Priority,
                Transport = s.Transport.ToLower(),
            }).ToList();

            var context = new SubscriptionRequestContext
            {
                Requests = requests,
                ClientId = clientId.Value,
            };

            await _subscriptionService.Subscribe(context);

            if (context.IsError)
                return BadRequest(context.UserError);
            throw new NotImplementedException("return all records");

            //var sr = new
            //{
            //    subscribedAt = context.SubscribedAt.ToUnixTimeSeconds(),
            //    expiration = context.Expiration,
            //};
            //return Ok(sr);
        }
    }
}