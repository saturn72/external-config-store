using Dapper;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace ExternalStore.Data.MySql
{
    public class MySqlConfigStore : IConfigStore
    {
        private readonly string _connectionString;

        public MySqlConfigStore(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<JsonElement> GetConfigByKey(string? configKey)
        {
            using var con = new MySqlConnection(_connectionString);
            var json =  await con.QuerySingleOrDefaultAsync<string>(Dapper.Config.GetConfigByKey, new { configKey });
            return JsonDocument.Parse(json).RootElement;
        }

        public Task<IEnumerable<string>> GetConfigKeys()
        {
            using var con = new MySqlConnection(_connectionString);
            return con.QuerySingleOrDefaultAsync<IEnumerable<string>>(Dapper.Config.GetConfigKeys);
        }
    }
}