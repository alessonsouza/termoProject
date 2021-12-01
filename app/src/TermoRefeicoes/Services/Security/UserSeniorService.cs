using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Dapper;

namespace termoRefeicoes.Services.Security
{
    public class UserSeniorService : IUserSenior
    {
        public readonly IConnectionFactory _connection;

        public UserSeniorService(IConnectionFactory conn)
        {
            _connection = conn;
        }

        public async Task<IEnumerable<Users>> GetUsers(string username)
        {
            string sql = @"SELECT r034usu.numcad as numCadastro
                             FROM r999usu
                             JOIN r034usu
                                ON ( r034usu.codusu = r999usu.codusu )
                            WHERE lower(r999usu.nomusu) = lower(:username)";
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
            string sql = @"select fun.NOMFUN as Name from r034fun fun
                            where fun.numcad = :matricula";
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