using System.Text.Json;

namespace ExternalStore.Data
{
    public interface IConfigStore
    {
        Task<JsonElement> GetConfigByKey(string? key);
        Task<IEnumerable<string>> GetConfigKeys();
    }
}
