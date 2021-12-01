using System;
using System.Collections.Generic;
using termoRefeicoes.Helper;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Dapper;
using Microsoft.AspNetCore.Http;


namespace termoRefeicoes.Services.Report
{
    public class ConsumosAceitosService : IConsumosAceitosReport
    {
        private readonly IConnectionFactory _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private List<Termo> termos;
        private int i = 1;

        public ConsumosAceitosService(IConnectionFactory conn, IHttpContextAccessor httpContextAccessor)
        {
            _connection = conn;
            _httpContextAccessor = httpContextAccessor;
        }


        public Datas GetDate(int mesParam, int anoParam)
        {
            var dateMes = mesParam;
            var dateAno = anoParam;

            dateMes = mesParam - 1;
            if (dateMes < 1)
            {
                dateMes = 12;
                dateAno -= 1;
            }

            if (dateMes > 12)
            {
                dateMes = 1;
                dateAno += 1;
            }

            var datas = new Datas
            {
                ano = dateAno,
                mes = dateMes
            };
            // Refeicoes(dateAno + dateMes.toString().padStart(2, '0'));         


            // var res = new {
            //     dataC = dateMes.ToString().PadLeft(2, '0') + "/" + dateAno.ToString().Remove(0, 2),
            //     dataE =  dateMes.ToString().PadLeft(2, '0') + dateAno.ToString()
            // };

            return datas;
        }
        public string ReturnDataComBarra(int mes, int ano)
        {

            return mes.ToString().PadLeft(2, '0') + "/" + ano.ToString().Remove(0, 2);
        }

        public string ReturnDataSemBarra(int mes, int ano)
        {

            return ano.ToString() + mes.ToString().PadLeft(2, '0');
        }
        public List<Termo> Consultar(ReportFilters filtros)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;

            string filterCLause = "";
            string whereClause = "";
            bool abreParenteses = false;
            string dataC = "";
            var dataFim = "";
            string AndORCLause = " AND (";
            string sql = "";
            string caseWhenClause = "";
            var dataInicio = filtros.DATA_INICIO;
            bool jAHACEITOU = false;

            var ano = "";
            var mes = "";
            var mesInt = 0;
            var anoInt = 0;
            var firstMonth = "";

            // dataC = mes + "/" + ano.Remove(0, 2);

            whereClause = " where ";

            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "aceitos")
            {
                jAHACEITOU = true;
                filterCLause += whereClause;
                filterCLause += "acc.usu_datchk IS NOT NULL and  fun.SITAFA <> 7";
                dataC = filtros.DATA_INICIO;
            }
            else
            {
                ano = filtros.DATA_INICIO.Remove(4, 2).Replace("'", "");
                mes = filtros.DATA_INICIO.Remove(0, 4).Replace("'", "");
                mesInt = Int32.Parse(mes);
                anoInt = Int32.Parse(ano);
                firstMonth = GetFirstMonth();

                dataC = mes + "/" + ano.Remove(0, 2);
                filterCLause += whereClause;
                filterCLause += "acc.usu_datchk IS NULL and  fun.SITAFA <> 7 and acc.codref > 0 and acc.codrlg  = 20 ";
            }

            caseWhenClause += "case";
            while (firstMonth != dataC)
            {

                if (String.IsNullOrEmpty(firstMonth))
                {
                    firstMonth = dataC;
                }

                if (!String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()))
                {
                    if (String.IsNullOrEmpty(AndORCLause))
                    {
                        AndORCLause = " OR ";
                    }

                    filterCLause += AndORCLause;
                    filterCLause += "(";
                    abreParenteses = true;
                    if (jAHACEITOU)
                    {
                        filterCLause += " to_char(acc.usu_datchk,'YYYYMMDD') >= '" + filtros.DATA_INICIO + "'";
                    }
                    else
                    {
                        dataFim = CompetenciaHelper.GetEndDate(dataInicio);
                        dataInicio = CompetenciaHelper.GetStartDate(dataInicio);
                        filterCLause += " to_char(acc.datapu,'YYYYMMDD') >= '" + dataInicio + "'";
                        caseWhenClause += " when (to_char(acc.datapu,'YYYYMMDD') >= '" + dataInicio + "'";

                    }
                }
                AndORCLause = "";
                if (!String.IsNullOrEmpty(filtros.DATA_FIM.Trim()))
                {

                    if (String.IsNullOrEmpty(AndORCLause))
                    {
                        AndORCLause += " AND ";
                    }
                    filterCLause += AndORCLause;
                    caseWhenClause += AndORCLause;
                    if (jAHACEITOU)
                    {
                        // if (!abreParenteses)
                        // {
                        //     filterCLause += "(";
                        // }
                        filterCLause += " to_char(acc.usu_datchk,'YYYYMMDD')  <= '" + filtros.DATA_FIM + "'";
                        // if (abreParenteses)
                        // {
                        //     filterCLause += ")";
                        // }
                    }
                    else
                    {

                        filterCLause += "  to_char(acc.datapu,'YYYYMMDD') < '" + dataFim + "'";
                        caseWhenClause += "  to_char(acc.datapu,'YYYYMMDD') < '" + dataFim + "')";

                    }
                }

                if (!jAHACEITOU)
                {
                    AndORCLause = "";
                    dataC = ReturnDataComBarra(mesInt, anoInt);

                    var datas = GetDate(mesInt, anoInt);
                    mesInt = datas.mes;
                    anoInt = datas.ano;
                    dataInicio = ReturnDataSemBarra(mesInt, anoInt);
                    caseWhenClause += " then '" + dataC + "'";
                }
                if (abreParenteses)
                {
                    filterCLause += ")";
                }

            }

            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "aceitos")
            {
                jAHACEITOU = true;
                sql = @"SELECT  distinct (acc.usu_datchk) as data_aceite,
                                acc.usu_horchk as hora_aceite,
                                fun.numcad,
                                fun.nomfun,                        
                                cc.nomccu
                        FROM r034fun  fun  
                        JOIN r018CCU  cc
                        ON ( cc.codccu = fun.codccu and cc.numemp = fun.numemp)
                        join r070acc acc on (fun.numemp = acc.numemp
                                        AND fun.tipcol = acc.tipcol
                                        AND fun.numcad = acc.numcad )
                        ";
            }
            else
            {
                caseWhenClause += " END  as termo_descricao ";
                sql = @"SELECT  distinct fun.numcad,
                                acc.usu_datchk as data_aceite,
                                acc.usu_horchk as hora_aceite,                                
                                fun.nomfun,                             
                                cc.nomccu,";
                sql += caseWhenClause;
                sql += @" 
                        FROM r034fun  fun
                        JOIN r018CCU  cc
                        ON ( cc.codccu = fun.codccu and cc.numemp = fun.numemp)
                        join r070acc acc on (fun.numemp = acc.numemp
                                        AND fun.tipcol = acc.tipcol
                                        AND fun.numcad = acc.numcad )
                        ";

            }

            filterCLause += !String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()) ? ")" : "";
            sql += filterCLause;

            filterCLause = "";
            if (filtros.NUMCAD != 0)
            {

                if (String.IsNullOrEmpty(AndORCLause))
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

            sql += filterCLause;
            sql += " ORDER BY cc.nomccu asc";
            Console.WriteLine(sql);
            using (var conn = _connection.Connection())
            {
                try
                {
                    termos = new List<Termo>();
                    var resp = conn.Query<Termo>(sql);


                    termos.Add(new Termo { TermoList = (List<Termo>)resp });
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }


            }



            return termos[0].TermoList;
        }

        public string GetFirstMonth()
        {


            // string sql = @"SELECT to_char(acc.datacc,'YYYY-MM-DD') DataRef                               
            //                     FROM r070acc acc
            //                     JOIN r034fun  fun
            //                         ON ( fun.numemp = acc.numemp
            //                     AND fun.tipcol = acc.tipcol
            //                     AND fun.numcad = acc.numcad )                               
            //                   WHERE acc.codrlg    = 20
            //                     AND fun.numcad    = :matricula
            //                     order by acc.datacc  asc
            //                     fetch first row only";


            string sql = @" SELECT 
                            MIN (TO_CHAR(acc.datapu, 'MM/YY')) as datatT                                                    
                            FROM r034fun  fun
                            JOIN r018CCU  cc
                            ON ( cc.codccu = fun.codccu )
                            join r070acc acc on (fun.numemp = acc.numemp
                                            AND fun.tipcol = acc.tipcol
                                            AND fun.numcad = acc.numcad )
                            where acc.usu_datchk IS NULL and  fun.SITAFA <> 7 ORDER BY FUN.NUMCAD";
            // var param = new DynamicParameters();
            // param.Add(":matricula", matricula);

            using (var conn = _connection.Connection())
            {
                try
                {
                    var res = conn.ExecuteScalar<string>(sql);
                    return res;
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

            string filterCLause = "";
            string whereClause = "";
            bool abreParenteses = false;
            string dataC = "";
            var dataFim = "";
            string AndORCLause = " AND (";
            string sql = "";
            string caseWhenClause = "";
            var dataInicio = filtros.DATA_INICIO;
            bool jAHACEITOU = false;

            var ano = "";
            var mes = "";
            var mesInt = 0;
            var anoInt = 0;
            var firstMonth = "";

            // dataC = mes + "/" + ano.Remove(0, 2);

            whereClause = " where ";

            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "aceitos")
            {
                jAHACEITOU = true;
                filterCLause += whereClause;
                filterCLause += "acc.usu_datchk IS NOT NULL and  fun.SITAFA <> 7";
                dataC = filtros.DATA_INICIO;
            }
            else
            {
                ano = filtros.DATA_INICIO.Remove(4, 2).Replace("'", "");
                mes = filtros.DATA_INICIO.Remove(0, 4).Replace("'", "");
                mesInt = Int32.Parse(mes);
                anoInt = Int32.Parse(ano);
                firstMonth = GetFirstMonth();

                dataC = mes + "/" + ano.Remove(0, 2);
                filterCLause += whereClause;
                filterCLause += "acc.usu_datchk IS NULL and  fun.SITAFA <> 7 and acc.codref > 0 and acc.codrlg  = 20 ";
            }

            caseWhenClause += "case";
            while (firstMonth != dataC)
            {

                if (String.IsNullOrEmpty(firstMonth))
                {
                    firstMonth = dataC;
                }

                if (!String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()))
                {
                    if (String.IsNullOrEmpty(AndORCLause))
                    {
                        AndORCLause = " OR ";
                    }

                    filterCLause += AndORCLause;
                    filterCLause += "(";
                    abreParenteses = true;
                    if (jAHACEITOU)
                    {
                        filterCLause += " to_char(acc.usu_datchk,'YYYYMMDD') >= '" + filtros.DATA_INICIO + "'";
                    }
                    else
                    {
                        dataFim = CompetenciaHelper.GetEndDate(dataInicio);
                        dataInicio = CompetenciaHelper.GetStartDate(dataInicio);
                        filterCLause += " to_char(acc.datapu,'YYYYMMDD') >= '" + dataInicio + "'";
                        caseWhenClause += " when (to_char(acc.datapu,'YYYYMMDD') >= '" + dataInicio + "'";

                    }
                }
                AndORCLause = "";
                if (!String.IsNullOrEmpty(filtros.DATA_FIM.Trim()))
                {

                    if (String.IsNullOrEmpty(AndORCLause))
                    {
                        AndORCLause += " AND ";
                    }
                    filterCLause += AndORCLause;
                    caseWhenClause += AndORCLause;
                    if (jAHACEITOU)
                    {
                        // if (!abreParenteses)
                        // {
                        //     filterCLause += "(";
                        // }
                        filterCLause += " to_char(acc.usu_datchk,'YYYYMMDD')  <= '" + filtros.DATA_FIM + "'";
                        // if (abreParenteses)
                        // {
                        //     filterCLause += ")";
                        // }
                    }
                    else
                    {

                        filterCLause += "  to_char(acc.datapu,'YYYYMMDD') < '" + dataFim + "'";
                        caseWhenClause += "  to_char(acc.datapu,'YYYYMMDD') < '" + dataFim + "')";

                    }
                }

                if (!jAHACEITOU)
                {
                    AndORCLause = "";
                    dataC = ReturnDataComBarra(mesInt, anoInt);

                    var datas = GetDate(mesInt, anoInt);
                    mesInt = datas.mes;
                    anoInt = datas.ano;
                    dataInicio = ReturnDataSemBarra(mesInt, anoInt);
                    caseWhenClause += " then '" + dataC + "'";
                }
                if (abreParenteses)
                {
                    filterCLause += ")";
                }

            }

            if (String.IsNullOrEmpty(filtros.JAHACEITOU.Trim()) || filtros.JAHACEITOU == "aceitos")
            {
                jAHACEITOU = true;
                sql = @"SELECT  distinct (acc.usu_datchk) as data_aceite,
                                acc.usu_horchk as hora_aceite,
                                fun.numcad,
                                fun.nomfun,                        
                                cc.nomccu
                        FROM r034fun  fun  
                        JOIN r018CCU  cc
                        ON ( cc.codccu = fun.codccu and cc.numemp = fun.numemp)
                        join r070acc acc on (fun.numemp = acc.numemp
                                        AND fun.tipcol = acc.tipcol
                                        AND fun.numcad = acc.numcad )
                        ";
            }
            else
            {
                caseWhenClause += " END  as termo_descricao ";
                sql = @"SELECT  distinct fun.numcad,
                                acc.usu_datchk as data_aceite,
                                acc.usu_horchk as hora_aceite,                                
                                fun.nomfun,                             
                                cc.nomccu,";
                sql += caseWhenClause;
                sql += @" 
                        FROM r034fun  fun
                        JOIN r018CCU  cc
                        ON ( cc.codccu = fun.codccu and cc.numemp = fun.numemp)
                        join r070acc acc on (fun.numemp = acc.numemp
                                        AND fun.tipcol = acc.tipcol
                                        AND fun.numcad = acc.numcad )
                        ";

            }

            filterCLause += !String.IsNullOrEmpty(filtros.DATA_INICIO.Trim()) ? ")" : "";
            sql += filterCLause;

            filterCLause = "";
            if (filtros.NUMCAD != 0)
            {

                if (String.IsNullOrEmpty(AndORCLause))
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

            sql += filterCLause;
            sql += " ORDER BY cc.nomccu asc";

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