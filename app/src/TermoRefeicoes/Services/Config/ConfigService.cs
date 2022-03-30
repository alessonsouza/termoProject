using System.Threading.Tasks;
using Dapper;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;

namespace TermoRefeicoes.Services.ConfigService
{
    public class ConfigService : IConfigService
    {
        public readonly IConnectionFactory _connection;

        public ConfigService(IConnectionFactory connection)
        {
            _connection = connection;
        }

        public async Task<Config> getConfig()
        {
            string sql = @"select USU_DiaIni as diaInicio, USU_DiaFim as diaFim
                            from R068DRF";

            using (var conn = _connection.Connection())
            {
                return await conn.QueryFirstOrDefaultAsync<Config>(sql);
            }
        }

        public async Task<int> saveConfig(Config dados)
        {
            string sql = @"UPDATE R068DRF
                             SET USU_DiaIni = :DATAINI, USU_DiaFim = :DATAFIM";

            object obj = new
            {
                DATAINI = dados.diaInicio,
                DATAFIM = dados.diaFim
            };

            using (var conn = _connection.Connection())
            {
                return await conn.ExecuteScalarAsync<int>(sql, obj);
            }
        }
    }
}