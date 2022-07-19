using ExternalStore.API.Models;
using ExternalStore.Services.Subscription;
using Microsoft.AspNetCore.Mvc;

namespace ExternalStore.API.Controllers
{
    [ApiController]
    [Route("subscribe")]
    public class SubscribeController : ControllerBase
    {
        private readonly ISubscriptionService _service;

        public SubscribeController(ISubscriptionService service)
        {
            _service = service;
        }
        [HttpPost]
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

            await _service.Subscribe(context);

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