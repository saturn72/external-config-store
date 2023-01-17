using EasyCaching.Core;
using ExternalStore.Data;
using ExternalStore.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace ExternalStore.Services.Config
{
    public class ConfigService : IConfigService
    {
        private readonly IConfigStore _store;
        private readonly IEasyCachingProvider _cache;
        private readonly ILogger<ConfigService> _logger;

        public ConfigService(
            IConfigStore store,
            IEasyCachingProvider cache,
            ILogger<ConfigService> logger)
        {
            _store = store;
            _cache = cache;
            _logger = logger;
        }

        public async Task GetConfigByKey(GetConfigByKeyContext context)
        {
            context.Result = await GetConfigByKeyInternal(context.ConfigKey);
            if (context.Result.Equals(default))
            {
                context.Error = $"Failed to get config for \'key\'= {context.ConfigKey}";
                _logger.LogError(context.Error);
                return;
            }
        }

        public async Task GetConfigByKeyAndPath(GetConfigByKeyAndPathContext context)
        {
            var (validPaths, invalidPaths) = ValidatePaths(context.Paths);
            if (invalidPaths.Any())
            {
                context.SetErrors($"Invalid paths located: {invalidPaths.ToJsonString()}",
                    "Invalid Request");
                _logger.LogError(context.Error);
                return;
            }

            var cfg = await GetConfigByKeyInternal(context.ConfigKey);
            if (cfg.Equals(default))
            {
                context.Error = $"Failed to get config for \'key\'= {context.ConfigKey}";
                _logger.LogError(context.Error);
                return;
            }

            context.Result = new Dictionary<string, string>();

            var pathKeys = validPaths
                .Select(vp => ConfigCaching.BuildGetByKeyAndPath(context.ConfigKey, vp.source))
                .ToArray();

            var entries = await _cache.GetAllAsync<string>(pathKeys);
            var entriesWithValue = entries.Where(e => e.Value.HasValue).ToDictionary(k => k.Key, v => v.Value);

            if (entriesWithValue.Count() != validPaths.Count())
            {
                var missing = validPaths
                    .Where(v => !entriesWithValue.ContainsKey(ConfigCaching.BuildGetByKeyAndPath(context.ConfigKey, v.source)));

                foreach (var (source, pathParts) in missing)
                {
                    var json = ParseJsonPath(cfg, pathParts);

                    if (json != null)
                        context.Result[source] = json;
                }
            }
        }

        private (IEnumerable<(string source, string[] pathParts)>, IEnumerable<string[]>)
            ValidatePaths(IReadOnlyCollection<string> paths)
        {
            var valid = new List<(string, string[])>();
            var invalid = new List<string[]>();

            foreach (var path in paths)
            {
                var jsonPath = path.Split(":");
                if (jsonPath.All(x => x.HasValue()))
                {
                    valid.Add((path, jsonPath));
                }
                else
                {
                    invalid.Add(jsonPath);
                }
            }
            return (valid, invalid);
        }

        private string? ParseJsonPath(JsonElement jsonElement, string[] jsonPath)
        {
            var cur = jsonElement;

            for (var i = 0; i < jsonPath.Length; i++)
            {
                var jp = jsonPath[i];
                if (!cur.TryGetProperty(jp, out cur))
                {
                    _logger.LogError($"Cannot find path \'{jp}\' in config");
                    break;
                }
            }
            return cur.ToString();
        }

        private async Task<JsonElement> GetConfigByKeyInternal(string configKey)
        {
            var ck = ConfigCaching.BuildGetByKey(configKey);
            var res = await _cache.GetAsync<JsonElement>(ck);

            if (res.HasValue && res.Value.ValueKind != JsonValueKind.Undefined)
                return default;

            if (res.HasValue)
                return res.Value;

            var je = await _store.GetConfigByKey(configKey);
            if (je.Equals(default))
                return default;

            await _cache.SetAsync(ck, je, ConfigCaching.Expiration);
            return je;
        }
    }
}
