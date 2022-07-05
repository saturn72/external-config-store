using System.Text.Json;

namespace ExternalStore.Domain
{
    public record GetConfigByKeyContext : ContextBase<JsonElement>
    {
        public string? ConfigKey { get; init; }
    }
}
