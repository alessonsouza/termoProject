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
    public class LancamentosService : ILancamentosReport
    {
        private readonly IConnectionFactory _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LancamentosService(IConnectionFactory conn, IHttpContextAccessor httpContextAccessor)
        {
            _connection = conn;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IEnumerable<Lancamentos>> Consultar(ReportFilters filtros)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;
            string whereClause = "";
            string filterCLause = "";
            string AndORCLause = "";
            string sql = "";


            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "0")
            {
                sql = @"SELECT  ref.usu_numcad as numcad,
                    fun.nomfun,                  
                    (CASE 
                    WHEN REF.USU_HORCON >= 360 AND REF.USU_HORCON <= 660 THEN 'LANCHE MANHÃ'                     
                    WHEN REF.USU_HORCON >= 661 AND REF.USU_HORCON <= 900 THEN 'ALMOÇO' 
                    WHEN REF.USU_HORCON >= 901 AND REF.USU_HORCON <= 1169 THEN 'LANCHE TARDE' 
                    WHEN REF.USU_HORCON >= 1170 AND REF.USU_HORCON <= 1349 THEN 'LANCHE NOITE' 
                    WHEN (REF.USU_HORCON < 360) OR (REF.USU_HORCON >= 1350) THEN 'JANTA' 
                    END) DESREF,                    
                    (case
                    when ref.usu_tpcapt = 'D' then 'Digitado'
                    when ref.usu_tpcapt = 'C' then 'Capturado'
                    when ref.usu_tpcapt = 'S' then 'Senior'
                    end) as usu_tpcapt ,
                    ref.usu_datcon,
                    ref.usu_horcon,
                    cc.nomccu FROM usu_tconref ref ";
            }
            else
            {
                sql = @"SELECT  ref.usu_numcad as numcad,
                    fun.nomfun,        
                    ref.usu_codref,
                    trf.desref, 
                    ref.usu_qtdref,
                    (case 
                    when ref.usu_tpcapt = 'D' then 'Digitado'
                    when ref.usu_tpcapt = 'C' then 'Capturado'
                    end) as usu_tpcapt ,
                    ref.usu_datcon,
                    ref.usu_horcon,
                    cc.nomccu
                    from usu_tconfri ref
                     JOIN r068trf  trf  ON (trf.codref = ref.usu_codref
                                AND trf.numemp = ref.usu_numemp)";
            }
            sql += @" join r034fun  fun  on ( fun.numcad = ref.usu_numcad and
                        fun.tipcol = ref.usu_tipcol and 
                        fun.numemp = ref.usu_numemp)
                    JOIN r018CCU  cc   ON ( cc.codccu = fun.codccu and cc.numemp = fun.numemp)
                   ";

            if (!String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }

                filterCLause += " to_char(ref.usu_datcon,'YYYYMMDD') >= '" + filtros.DATA_INICIO + "'";
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
                filterCLause += " to_char(ref.usu_datcon,'YYYYMMDD') <= '" + filtros.DATA_FIM + "'";
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
                filterCLause += " ref.usu_numcad = " + filtros.NUMCAD;
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
                filterCLause += " cc.codccu in (" + filtros.CODCCU.Replace("'", "") + ")";
            }
            // if (filtros.JAHACEITOU == "1")
            // {
            //     //     if (String.IsNullOrEmpty(AndORCLause) && !String.IsNullOrEmpty(whereClause))
            //     //     {
            //     //         AndORCLause += " AND ";
            //     //     }
            //     if (String.IsNullOrEmpty(whereClause))
            //     {
            //         whereClause = " where ";
            //         sql += whereClause;
            //     }
            //     else if (String.IsNullOrEmpty(AndORCLause))
            //     {
            //         AndORCLause += " AND ";
            //     }

            //     filterCLause += AndORCLause;
            //     filterCLause += @"fun.USU_CODTER IS NULL 
            //                      and  fun.SITAFA <> 7 and tipcol = 1 
            //                      and ((numcad >= 1 and numcad <= 9999) or (numcad >= 800000 and numcad <= 999999))";
            // }
            sql += filterCLause;
            sql += "   order by usu_datcon desc, usu_horcon desc";
            Console.WriteLine(sql);

            using (var conn = _connection.Connection())
            {
                try
                {
                    return await conn.QueryAsync<Lancamentos>(sql);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
        }



        public List<Lancamentos> DadosExcel(ReportFilters filtros)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;
            string whereClause = "";
            string filterCLause = "";
            string AndORCLause = "";
            string sql = "";
            List<Lancamentos> dados = new List<Lancamentos>();
            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "0")
            {
                sql = @"SELECT  ref.usu_numcad as numcad,
                    fun.nomfun,                  
                    (CASE 
                    WHEN REF.USU_HORCON >= 360 AND REF.USU_HORCON <= 660 THEN 'LANCHE MANHÃ'                     
                    WHEN REF.USU_HORCON >= 661 AND REF.USU_HORCON <= 900 THEN 'ALMOÇO' 
                    WHEN REF.USU_HORCON >= 901 AND REF.USU_HORCON <= 1169 THEN 'LANCHE TARDE' 
                    WHEN REF.USU_HORCON >= 1170 AND REF.USU_HORCON <= 1349 THEN 'LANCHE NOITE' 
                    WHEN (REF.USU_HORCON < 360) OR (REF.USU_HORCON >= 1350) THEN 'JANTA' 
                    END) DESREF,                    
                    (case
                    when ref.usu_tpcapt = 'D' then 'Digitado'
                    when ref.usu_tpcapt = 'C' then 'Capturado'
                    end) as usu_tpcapt ,
                    ref.usu_datcon,
                    ref.usu_horcon,
                    cc.nomccu FROM usu_tconref ref ";
            }
            else
            {
                sql = @"SELECT  ref.usu_numcad as numcad,
                    fun.nomfun,        
                    ref.usu_codref,
                    trf.desref, 
                    ref.usu_qtdref,
                    (case 
                    when ref.usu_tpcapt = 'D' then 'Digitado'
                    when ref.usu_tpcapt = 'C' then 'Capturado'
                    end) as usu_tpcapt ,
                    ref.usu_datcon,
                    ref.usu_horcon,
                    cc.nomccu
                    from usu_tconfri ref
                     JOIN r068trf  trf  ON (trf.codref = ref.usu_codref
                                AND trf.numemp = ref.usu_numemp)";
            }
            sql += @" join r034fun  fun  on ( fun.numcad = ref.usu_numcad and
                        fun.tipcol = ref.usu_tipcol and 
                        fun.numemp = ref.usu_numemp)
                    JOIN r018CCU  cc   ON ( cc.codccu = fun.codccu and cc.numemp = fun.numemp)
                   ";

            if (!String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()))
            {
                if (String.IsNullOrEmpty(whereClause))
                {
                    whereClause = " where ";
                    sql += whereClause;
                }

                filterCLause += " to_char(ref.usu_datcon,'YYYYMMDD') >= '" + filtros.DATA_INICIO + "'";
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
                filterCLause += " to_char(ref.usu_datcon,'YYYYMMDD') <= '" + filtros.DATA_FIM + "'";
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
                filterCLause += " ref.usu_numcad = " + filtros.NUMCAD;
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
                filterCLause += " cc.codccu in (" + filtros.CODCCU.Replace("'", "") + ")";
            }
            // if (filtros.JAHACEITOU == "1")
            // {
            //     //     if (String.IsNullOrEmpty(AndORCLause) && !String.IsNullOrEmpty(whereClause))
            //     //     {
            //     //         AndORCLause += " AND ";
            //     //     }
            //     if (String.IsNullOrEmpty(whereClause))
            //     {
            //         whereClause = " where ";
            //         sql += whereClause;
            //     }
            //     else if (String.IsNullOrEmpty(AndORCLause))
            //     {
            //         AndORCLause += " AND ";
            //     }

            //     filterCLause += AndORCLause;
            //     filterCLause += @"fun.USU_CODTER IS NULL 
            //                      and  fun.SITAFA <> 7 and tipcol = 1 
            //                      and ((numcad >= 1 and numcad <= 9999) or (numcad >= 800000 and numcad <= 999999))";
            // }
            sql += filterCLause;
            sql += "   order by usu_datcon desc, usu_horcon desc";
            Console.WriteLine(sql);

            using (var conn = _connection.Connection())
            {
                try
                {

                    var reso = conn.Query<Lancamentos>(sql);
                    return (List<Lancamentos>)reso;
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
        }





    }
}