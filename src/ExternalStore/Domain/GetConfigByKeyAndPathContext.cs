using System.Text.Json;

namespace ExternalStore.Domain
{
    public record GetConfigByKeyAndPathContext : ContextBase<IDictionary<string, string>>
    {
        public string? ConfigKey { get; init; }
        public IReadOnlyCollection<string>? Paths { get; init; }
    }
}
