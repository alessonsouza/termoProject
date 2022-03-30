using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Helper;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Dapper;
using Microsoft.AspNetCore.Http;


namespace termoRefeicoes.Services.Report
{
    public class TermosAceitosService : ITermosAceitosReport
    {
        private readonly IConnectionFactory _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TermosAceitosService(IConnectionFactory conn, IHttpContextAccessor httpContextAccessor)
        {
            _connection = conn;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<Termo>> Consultar(ReportFilters filtros)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;
            string whereClause = "";
            string filterCLause = "";
            string AndORCLause = "";
            string sql = "";

            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "aceitos")
            {
                sql = @"SELECT ter.usu_numcad as numcad, 
                                ter.usu_dtacei as data_aceite, 
                                ter.usu_hracei as hora_aceite, 
                                tv.usu_dester as termo_descricao,
                                fun.nomfun,
                                fun.codccu,
                                cc.nomccu                     
                        FROM USU_TTERMAC ter
                        JOIN r034fun  fun
                            ON ( fun.numcad = ter.usu_numcad )
                        JOIN r018CCU  cc
                            ON ( cc.codccu = fun.codccu )
                        JOIN usu_tverace tv  on (ter.usu_codver = tv.usu_codver)";
            }
            else
            {
                sql = @"SELECT  fun.numcad, 
                                fun.nomfun,
                                cc.nomccu
                          FROM r034fun  fun  
                          JOIN r018CCU  cc
                               ON ( cc.codccu = fun.codccu ) ";
            }

            if (!String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }

                filterCLause += " to_char(ter.usu_dtacei,'YYYYMMDD') >= '" + filtros.DATA_INICIO + "'";
            }
            if (!String.IsNullOrEmpty(filtros.DATA_FIM.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }
                filterCLause += AndORCLause;
                filterCLause += " to_char(ter.usu_dtacei,'YYYYMMDD') <= '" + filtros.DATA_FIM + "'";
            }
            if (filtros.NUMCAD != 0)
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }
                filterCLause += AndORCLause;
                filterCLause += " fun.numcad = " + filtros.NUMCAD;
            }
            if (!String.IsNullOrEmpty(filtros.CODCCU.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }
                filterCLause += AndORCLause;
                filterCLause += " fun.codccu in (" + filtros.CODCCU.Replace("'", "") + ")";
            }
            if (filtros.JAHACEITOU == "naoaceitos")
            {
                //     if (String.IsNullOrEmpty(AndORCLause) && !String.IsNullOrEmpty(whereClause))
                //     {
                //         AndORCLause += " AND ";
                //     }
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }

                filterCLause += AndORCLause;
                filterCLause += @"fun.USU_CODTER IS NULL 
                                 and  fun.SITAFA <> 7 and tipcol = 1 
                                 and ((numcad >= 1 and numcad <= 9999) or (numcad >= 800000 and numcad <= 999999))";
            }
            sql += filterCLause;
            sql += " ORDER BY cc.nomccu";
            Console.WriteLine(sql);

            using (var conn = _connection.Connection())
            {
                try
                {
                    return await conn.QueryAsync<Termo>(sql);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
        }



        public List<Termo> DadosExcel(ReportFilters filtros)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;
            string whereClause = "";
            string filterCLause = "";
            string AndORCLause = "";
            string sql = "";
            List<Termo> dados = new List<Termo>();
            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "aceitos")
            {
                sql = @"SELECT ter.usu_numcad as numcad, 
                               ter.usu_dtacei as data_aceite, 
                               to_char(ter.usu_dtacei,'DD/MM/YYYY') as data_aceite_char,
                                ter.usu_hracei as hora_aceite, 
                                tv.usu_dester as termo_descricao,
                                fun.nomfun,
                                fun.codccu,
                                cc.nomccu                     
                        FROM USU_TTERMAC ter
                        JOIN r034fun  fun
                            ON ( fun.numcad = ter.usu_numcad )
                        JOIN r018CCU  cc
                            ON ( cc.codccu = fun.codccu )
                        JOIN usu_tverace tv  on (ter.usu_codver = tv.usu_codver)";
            }
            else
            {
                sql = @"SELECT  fun.numcad, 
                                fun.nomfun,
                                cc.nomccu
                          FROM r034fun  fun  
                          JOIN r018CCU  cc
                               ON ( cc.codccu = fun.codccu ) ";
            }

            if (!String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }

                filterCLause += " to_char(ter.usu_dtacei,'YYYYMMDD') >= '" + filtros.DATA_INICIO + "'";
            }
            if (!String.IsNullOrEmpty(filtros.DATA_FIM.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }
                filterCLause += AndORCLause;
                filterCLause += " to_char(ter.usu_dtacei,'YYYYMMDD') <= '" + filtros.DATA_FIM + "'";
            }
            if (filtros.NUMCAD != 0)
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }
                filterCLause += AndORCLause;
                filterCLause += " fun.numcad = " + filtros.NUMCAD;
            }
            if (!String.IsNullOrEmpty(filtros.CODCCU.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }
                filterCLause += AndORCLause;
                filterCLause += " fun.codccu in (" + filtros.CODCCU.Replace("'", "") + ")";
            }
            if (filtros.JAHACEITOU == "naoaceitos")
            {
                //     if (String.IsNullOrEmpty(AndORCLause) && !String.IsNullOrEmpty(whereClause))
                //     {
                //         AndORCLause += " AND ";
                //     }
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }
                else if (String.IsNullOrEmpty(AndORCLause))
                {
                    AndORCLause += " AND ";
                }

                filterCLause += AndORCLause;
                filterCLause += @"fun.USU_CODTER IS NULL 
                                  and  fun.SITAFA <> 7 and tipcol = 1 
                                  and ((numcad >= 1 and numcad <= 9999) or (numcad >= 800000 and numcad <= 999999))";
            }
            sql += filterCLause;
            sql += " ORDER BY cc.nomccu";
            Console.WriteLine(sql);

            using (var conn = _connection.Connection())
            {
                try
                {

                    var reso = conn.Query<Termo>(sql);
                    return (List<Termo>)reso;
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
        }





    }
}