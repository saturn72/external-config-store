namespace ExternalStore.Services
{
    internal class ClientCaching
    {
        internal const string Prefix = "client:";
        internal static string BuildGetById(string? id) => $"{Prefix}id={id}";
        internal static readonly TimeSpan Expiration = TimeSpan.FromDays(30);
    }
}
