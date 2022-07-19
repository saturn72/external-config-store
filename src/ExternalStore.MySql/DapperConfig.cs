namespace ExternalStore.Data.MySql
{
    internal sealed partial class Dapper
    {
        internal sealed class Config
        {
            private const string TableName = "configs";

            //private static readonly string SELECT_WHERE = $"SELECT " +
            //    $"{nameof(AccountDomainModel.Id)}, " +
            //    $"{nameof(AccountDomainModel.CreatedByUserId)}, " +
            //    $"{nameof(AccountDomainModel.Name)} " +
            //    $"FROM {TableName} WHERE ";

            public static readonly string GetConfigKeys = $"SELECT ConfigKey FROM {TableName}";
            public static readonly string GetConfigByKey = $"SELECT Value FROM {TableName} WHERE configKey = @ConfigKey";
        }
    }
}
