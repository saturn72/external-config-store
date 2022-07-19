namespace ExternalStore.Data
{
    public interface IConfigStore
    {
        Task<string?> GetConfigByKey(string? key);
        Task<IEnumerable<string>> GetConfigKeys();
    }
}
