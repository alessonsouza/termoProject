using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Dapper;

namespace termoRefeicoes.Services.Security
{
    public class UserGeralService : IUserGeral
    {
        public readonly IConnectionFactory _connection;

        public UserGeralService(IConnectionFactory conn)
        {
            _connection = conn;
        }

        public async Task<IEnumerable<Users>> GetUsers(string username)
        {
            string sql = @"SELECT ";
            var param = new DynamicParameters();
            param.Add(":username", username);


            using (var conn = _connection.Connection())
            {
                try
                {
                    return await conn.QueryAsync<Users>(sql, param);
                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }
        }

        public async Task<IEnumerable<Users>> GetUsersByNumCAd(int matricula)
        {
            string sql = @"select ";
            var param = new DynamicParameters();
            param.Add(":matricula", matricula);


            using (var conn = _connection.Connection())
            {
                try
                {
                    return await conn.QueryAsync<Users>(sql, param);
                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }
        }
    }
}