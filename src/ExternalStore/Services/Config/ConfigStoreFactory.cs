using ExternalStore.Data;

namespace ExternalStore.API.Configurars
{
    public class DefaultConfigStoreFactory : IConfigStoreFactory
    {
        private readonly IReadOnlyDictionary<string, IConfigStore>? _configs;
        public DefaultConfigStoreFactory(IEnumerable<IConfigStore>? configs)
        {
            if (configs == null || !configs.Any()) throw new ArgumentException(nameof(configs));

            var temp = new Dictionary<string, IConfigStore>();
            foreach (var c in configs)
            {
                var keys = c.GetConfigKeys().GetAwaiter().GetResult();
                foreach (var k in keys)
                    temp.Add(k, c);
            }
            _configs = temp;
        }

        public virtual async Task<string?> GetConfig(string key)
        {
            if (!_configs.TryGetValue(key, out var store))
                return null;

            return await store.GetConfigByKey(key);
        }
    }
}
