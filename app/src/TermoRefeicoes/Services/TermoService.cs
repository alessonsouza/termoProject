using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Helper;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Dapper;
using Microsoft.AspNetCore.Http;

namespace termoRefeicoes.Services
{
    public class TermoService : ITermo
    {
        public readonly IConnectionFactory _connection;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TermoService(IConnectionFactory conn, IHttpContextAccessor httpContextAccessor)
        {
            _connection = conn;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<TermoVersion>> GetTermo()
        {

            string sql = @"SELECT * ";

            try
            {
                using (var conn = _connection.Connection())
                {
                    var resp = await conn.QueryAsync<TermoVersion>(sql);
                    return resp;
                }

            }
            catch (Exception e)
            {

                throw new ApplicationException(e.Message);
            }
            throw new NotImplementedException();
        }

        public bool Submit(string competencia, int matricula, int hora)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;
            string sql = @"UPDATE :matricula";
            int quantidade = 0;
            string dataat = null;
            var obj = new
            {
                dataInicio = CompetenciaHelper.GetStartDate(competencia),
                dataFim = CompetenciaHelper.GetEndDate(competencia),
                username = userName,
                dataAtualizacao = DateTime.Now,
                horaAtualizacao = hora,
                matricula = matricula
            };
            using (var conn = _connection.Connection())
            {
                try
                {
                    quantidade = conn.Execute(sql, obj);
                    return quantidade > 0;
                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }
        }
    }
}