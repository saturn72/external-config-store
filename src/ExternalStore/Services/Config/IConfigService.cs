using ExternalStore.Domain;

namespace ExternalStore.Services.Config
{
    public interface IConfigService
    {
        Task GetConfigByKey(GetConfigByKeyContext context);
        Task GetConfigByKeyAndPath(GetConfigByKeyAndPathContext context);
    }
}
