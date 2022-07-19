namespace ExternalStore.Data.Files
{
    public class FileConfigStore : IConfigStore, IDisposable
    {
        private readonly FileConfigStoreOptions _options;
        private readonly FileSystemWatcher? _watcher;
        public FileConfigStore(FileConfigStoreOptions options)
        {
            _options = options;
            if (_options.EnableNotifications)
                _watcher = StartDirectoryWatcher(options.RootDirectory);
        }
        public Task<string?> GetConfigByKey(string? key)
        {
            throw new NotImplementedException();
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
