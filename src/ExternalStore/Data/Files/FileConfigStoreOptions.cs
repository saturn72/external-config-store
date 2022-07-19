namespace ExternalStore.Data.Files
{
    public sealed class FileConfigStoreOptions
    {
        public string RootDirectory { get; set; } = "./config-files";
        public bool EnableNotifications { get; set; } = true;
    }
}