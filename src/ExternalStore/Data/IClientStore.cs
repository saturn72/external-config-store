using ExternalStore.Domain;

namespace ExternalStore.Data
{
    public interface IClientStore
    {
        Task<Client?> GetClientById(string? clientId);
    }
}
