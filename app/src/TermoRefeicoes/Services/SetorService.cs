using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Models;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace termoRefeicoes.Services
{
    public class SetorService : ISetor
    {
        public readonly IConnectionFactory _connection;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public SetorService(IConnectionFactory conn, IHttpContextAccessor httpContextAccessor)
        {
            _connection = conn;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Setor>> GetAll()
        {

            string sql = @"select CODCCU as id, NOMCCU as descricao
                                from r018ccu
                                where exists (select 1
                                from r034fun
                                where r034fun.codccu = r018ccu.codccu
                                and r034fun.sitafa <> 7)";

            using (var conn = _connection.Connection())
            {
                try
                {
                    return await conn.QueryAsync<Setor>(sql);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
        }
    }
}