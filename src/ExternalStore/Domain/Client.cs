namespace ExternalStore.Domain
{
    public record Client
    {
        private IEnumerable<string>? _transportNames;

        public string? Id { get; init; }
        public string? ClientId { get; init; }
        public IEnumerable<string>? ConfigKeys { get; init; }
        public uint Expiration { get; init; } = 60 * 60 * 24; //one day
        public string? Name { get; init; }
        public IDictionary<string, object>? Transports { get; init; }
        public IEnumerable<string>? TransportNames => _transportNames ??= Transports.Keys;
    }
}
