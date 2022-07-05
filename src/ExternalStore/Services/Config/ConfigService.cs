using EasyCaching.Core;
using ExternalStore.Data;
using ExternalStore.Domain;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExternalStore.Services.Config
{
    public class ConfigService : IConfigService
    {
        private static readonly JsonElement DefaultJsonElement = new();
        private readonly IEasyCachingProvider _cache;
        private readonly IConfigStore _store;
        private readonly ILogger<ConfigService> _logger;

        public ConfigService(
            IEasyCachingProvider cache,
            IConfigStore store,
            ILogger<ConfigService> logger)
        {
            _cache = cache;
            _store = store;
            _logger = logger;
        }

        public async Task GetConfigByKey(GetConfigByKeyContext context)
        {
            await GetConfigByKeyInternal(context.ConfigKey, context);
        }

        public async Task GetConfigByKeyAndPath(GetConfigByKeyAndPathContext context)
        {
            var ck = ConfigCaching.BuildGetByKey(context.ConfigKey);
            var cv = await _cache.GetAsync<JsonElement>(ck);

            if (cv.HasValue && cv.Value.ValueKind != JsonValueKind.Undefined)
            {
                context.SetErrors($"Failed to get config for \'key\'= {context.ConfigKey}",
                    "Key not exists");
                _logger.LogError(context.Error);
                return;
            }
            else
            {
                var json = await _store.GetConfigByKey(context.ConfigKey);
                if (json == default)
                {
                    context.SetErrors($"Failed to get config for \'key\'= {context.ConfigKey}",
                    "Key not exists");
                    _logger.LogError(context.Error);
                    return;
                }
                var je = JsonDocument.Parse(json).RootElement;
                await _cache.SetAsync(ck, cv, ConfigCaching.Expiration);
            }

            context.Result = new Dictionary<string, string>();
            foreach (var path in context.Paths)
            {
                if (context.Result.ContainsKey(path))
                    continue;

                var json = ParseJsonPath(cv.Value, path);
                if (json != null)
                    context.Result[path] = json;
            }
        }

        private string? ParseJsonPath(JsonElement jsonElement, string path)
        {
            var cur = jsonElement;
            var jsonPath = path.Split(":", StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < jsonPath.Length; i++)
            {
                if (!cur.TryGetProperty(jsonPath[i], out cur))
                {
                    _logger.LogError($"Cannot find path \'{jsonPath[i]}\' in config");
                    break;
                }
                return cur.ToString();
            }
            return default;
        }

        private async Task GetConfigByKeyInternal(string configKey, ContextBase<JsonElement> context)
        {
            var ck = ConfigCaching.BuildGetByKey(configKey);
            var res = await _cache.GetAsync<JsonElement>(ck);

            if (res.HasValue && res.Value.ValueKind != JsonValueKind.Undefined)
            {
                context.Result = res.Value;
                return;
            }

            var json = await _store.GetConfigByKey(configKey);
            if (json == default)
            {
                context.Error = $"Failed to get config for \'key\'= {configKey}";
                _logger.LogError(context.Error);
                return;
            }
            context.Result = JsonDocument.Parse(json).RootElement;
            await _cache.SetAsync(ck, context.Result, ConfigCaching.Expiration);
        }
    }
}
