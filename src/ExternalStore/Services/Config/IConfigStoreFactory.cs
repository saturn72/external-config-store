namespace ExternalStore.API.Configurars
{
    public interface IConfigStoreFactory
    {
        Task<string?> GetConfig(string key);
    }
}