namespace ExternalStore.Data
{
    public interface IConfigStore
    {
        Task<string?> GetConfigByKey(string? key);
    }
}
