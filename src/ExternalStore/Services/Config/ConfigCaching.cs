namespace ExternalStore.Services.Config
{
    internal sealed class ConfigCaching
    {
        internal const string Prefix = "config::";
        internal static string BuildGetByKey(string? key) => $"{Prefix}key={key}";
        internal static string BuildGetByKeyAndPath(string? key, string? path) => $"{Prefix}key={key}::path={path}";

        internal static readonly TimeSpan Expiration = TimeSpan.FromDays(30);
    }
}
