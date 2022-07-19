using ExternalStore.Domain;

namespace ExternalStore.Data.InMemory
{
    public sealed class InMemoryClientStore : IClientStore
    {
        private readonly ICollection<Client> _clients;
        public InMemoryClientStore(IEnumerable<Client> clients)
        {
            _clients = clients.ToList();
        }
        public Task<Client?> GetClientById(string? clientId)
        {
            var c = _clients.FirstOrDefault(c => c.ClientId == clientId);
            return Task.FromResult(c);
        }
    }
}
