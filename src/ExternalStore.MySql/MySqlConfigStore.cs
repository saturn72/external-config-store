using Dapper;
using MySql.Data.MySqlClient;

namespace ExternalStore.Data.MySql
{
    public class MySqlConfigStore : IConfigStore
    {
        private readonly string _connectionString;

        public MySqlConfigStore(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<string?> GetConfigByKey(string? configKey)
        {
            using var con = new MySqlConnection(_connectionString);
            return await con.QuerySingleOrDefaultAsync<string>(Dapper.Config.GetConfigByKey, new { configKey });
        }

        public Task<IEnumerable<string>> GetConfigKeys()
        {
            using var con = new MySqlConnection(_connectionString);
            return con.QuerySingleOrDefaultAsync<IEnumerable<string>>(Dapper.Config.GetConfigKeys);
        }
    }
}