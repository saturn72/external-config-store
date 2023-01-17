using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExternalStore.Data.Files
{
    public class FileConfigStore : IConfigStore, IDisposable
    {
        private readonly FileConfigStoreOptions _options;
        private readonly FileSystemWatcher? _watcher;
        private readonly ILogger<FileConfigStore> _logger;

        public FileConfigStore(
            FileConfigStoreOptions options,
            ILogger<FileConfigStore> logger)
        {
            _options = options;
            if (_options.EnableNotifications)
                _watcher = StartDirectoryWatcher(options.RootDirectory);

            _logger = logger;
        }

        public async Task<JsonElement> GetConfigByKey(string? key)
        {
            _logger.LogDebug($"Getting config key: {key}");
            var fullPath = Path.Combine(_options.RootDirectory, key + ".json");

            using var stream = File.OpenRead(fullPath);

            try
            {
                var jd = await JsonDocument.ParseAsync(stream);
                return jd.RootElement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return default;
        }

        public Task<IEnumerable<string>> GetConfigKeys()
        {
            throw new NotImplementedException();
        }

        private FileSystemWatcher StartDirectoryWatcher(string rootDirectory)
        {
            var watcher = new FileSystemWatcher
            {
                EnableRaisingEvents = true,
                Filter = "*.*",
                NotifyFilter = NotifyFilters.LastWrite,
                Path = rootDirectory,
            };
            watcher.Changed += (s, e) => OnChanged(e);
            watcher.Created += (s, e) => OnCreated(e);
            watcher.Deleted += (s, e) => OnDeleted(e);
            watcher.Renamed += (s, e) => OnRenamed(e);

            return watcher;
        }

        private void OnRenamed(RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnDeleted(FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnCreated(FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnChanged(FileSystemEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _watcher?.Dispose();
        }

    }
}
