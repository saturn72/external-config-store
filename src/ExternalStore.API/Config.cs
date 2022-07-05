using ExternalStore.Domain;

namespace ExternalStore.API
{
    public sealed class Config
    {
        internal static readonly IEnumerable<Client> Clients = new[]
        {
            new Client{
                ClientId = "c-id-1",
                ConfigKeys = new []{ "cfg-1", "cfg-2", "cfg-3", },
                Transports = new Dictionary<string, object>
                {
                    {"signalr", null },
                    {"rabbitmq", null }
                },
            }
        };
    }
}
