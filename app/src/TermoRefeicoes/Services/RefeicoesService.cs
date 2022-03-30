using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Helper;
using termoRefeicoes.Interfaces;
using termoRefeicoes.Interfaces.Services;
using termoRefeicoes.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Dynamic;
using Oracle.ManagedDataAccess.Types;
using System.Text;
using System.Security.Cryptography;

namespace termoRefeicoes.Services
{
    public class RefeicoesService : IRefeicoes
    {
        public readonly IConnectionFactory _connection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RefeicoesService(IConnectionFactory conn, IHttpContextAccessor httpContextAccessor)
        {
            _connection = conn;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Refeicoes>> Consultar(string competencia)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;

            var dataFim = CompetenciaHelper.GetEndDate(competencia);
            var dataInicio = CompetenciaHelper.GetStartDate(competencia);


            string sql = @"SELECT trf.codref              AS CodRefeicao,
                                    fun.numcad              AS NumCadastro,
                                    upper(trf.desref)       AS DescRefeicao,
                                    acc.datacc             DataRef,
                                    acc.horacc,
                                    acc.qtdacc,
                                    acc.valref              ValorRef,
                                    acc.usu_datchk,
                                    acc.usu_horchk
                                FROM r070acc acc
                                JOIN r034fun  fun
                                    ON ( fun.numemp = acc.numemp
                                AND fun.tipcol = acc.tipcol
                                AND fun.numcad = acc.numcad )
                                JOIN r068trf  trf
                                    ON trf.codref = acc.codref 
                                AND trf.numemp = acc.numemp
                              WHERE to_char(acc.datapu,'YYYYMMDD HH:mm') >= :dataInicio
                                AND TO_CHAR(acc.datapu, 'YYYYMMDD HH:mm') < :dataFim                              

                                AND acc.codrlg    = 20
                                AND fun.numcad    = ( SELECT r034usu.numcad
                                                        FROM r999usu
                                                        JOIN r034usu
                                                            ON ( r034usu.codusu = r999usu.codusu )
                                                        WHERE lower(r999usu.nomusu) = lower(:username)
                                                    )";

            var param = new DynamicParameters();
            param.Add(":dataInicio", dataInicio);
            param.Add(":dataFim", dataFim);
            param.Add(":username", userName);

            using (var conn = _connection.Connection())
            {
                try
                {
                    return await conn.QueryAsync<Refeicoes>(sql, param);
                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }
        }

        public int GetCountAccept(string competencia)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            string userName = user.Identity.Name;
            var dataFim = CompetenciaHelper.GetEndDate(competencia); //"25/04/2021";
            var dataInicio = CompetenciaHelper.GetStartDate(competencia);
            string sql = @"SELECT count(acc.codref)
                                FROM r070acc acc
                                JOIN r034fun  fun
                                    ON ( fun.numemp = acc.numemp
                                AND fun.tipcol = acc.tipcol
                                AND fun.numcad = acc.numcad )
                                JOIN r068trf  trf
                                    ON trf.codref = acc.codref 
                                AND trf.numemp = acc.numemp
                              WHERE to_char(acc.datapu,'YYYYMMDD HH:mm') >= :dataInicio
                                AND TO_CHAR(acc.datapu, 'YYYYMMDD HH:mm') < :dataFim                              
                                AND acc.usu_datchk is null
                                AND acc.codrlg    = 20
                                AND fun.numcad    = ( SELECT r034usu.numcad
                                                        FROM r999usu
                                                        JOIN r034usu
                                                            ON ( r034usu.codusu = r999usu.codusu )
                                                        WHERE lower(r999usu.nomusu) = lower(:username)
                                                    )";

            var param = new DynamicParameters();
            param.Add(":dataInicio", dataInicio);
            param.Add(":dataFim", dataFim);
            param.Add(":username", userName);
            Console.WriteLine(sql);
            using (var conn = _connection.Connection())
            {
                try
                {
                    return conn.ExecuteScalar<int>(sql, param);
                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }
        }

        public int GetTermoAceite(int matricula)
        {

            var res = 0;
            string sql = @"SELECT  count(*) as id
                                FROM USU_TTERMAC                               
                              WHERE usu_numcad = :matricula";

            var param = new DynamicParameters();
            param.Add(":matricula", matricula);

            using (var conn = _connection.Connection())
            {
                try
                {

                    res = conn.ExecuteScalar<int>(sql, param);
                    return res;

                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }
        }

        public object SaveTerm(Termo obj)
        {

            int res = 0;
            string sql = @" INSERT INTO USU_TTERMAC(USU_CODTER, USU_CODVER, USU_NUMCAD, USU_DTACEI, USU_HRACEI, USU_HASHID)            
                            VALUES(SEQ_USU_TTERMAC.nextval, :FK_TERMO, :NUMCAD, :DATA_ACEITE, :HORA_ACEITE, :TERMO_DESCRICAO)";
            // string sql = @"INSERT INTO USU_TVERACE (USU_CODVER, USU_VERSAO, USU_DESTER, USU_DATCRI)
            //                 VALUES(1, :NUMCAD, :TERMO_DESCRICAO,  :DATA_ACEITE)";
            using (var conn = _connection.Connection())
            {
                try
                {
                    var post = new
                    {
                        NUMCAD = obj.NUMCAD,
                        HORA_ACEITE = obj.HORA_ACEITE,
                        TERMO_DESCRICAO = obj.TERMO_DESCRICAO,
                        FK_TERMO = obj.FK_TERMO,
                    };

                    // string hashCode = String.Format("{0:X}", sourceString.GetHashCode());

                    byte[] hashValue;

                    string messageString = post.NUMCAD + obj.DATA_ACEITE.ToString() + post.HORA_ACEITE + obj.TERMO_DESCRICAO;

                    //Create a new instance of the UnicodeEncoding class to
                    //convert the string into an array of Unicode bytes.
                    UnicodeEncoding ue = new UnicodeEncoding();

                    //Convert the string into an array of bytes.
                    byte[] messageBytes = ue.GetBytes(messageString);

                    //Create a new instance of the SHA256 class to create
                    //the hash value.
                    SHA256 shHash = SHA256.Create();

                    //Create the hash value from the array of bytes.
                    //Criptografia de indexa a assinatura do  colaborador
                    hashValue = shHash.ComputeHash(messageBytes);

                    //Display the hash value to the console.
                    // foreach (byte b in hashValue)
                    // {
                    //     Console.Write("{0} ", b);
                    // }
                    var param = new DynamicParameters();
                    param.Add(":FK_TERMO", post.FK_TERMO);
                    param.Add(":NUMCAD", post.NUMCAD);
                    param.Add(":DATA_ACEITE", obj.DATA_ACEITE);
                    param.Add(":HORA_ACEITE", post.HORA_ACEITE);
                    param.Add(":TERMO_DESCRICAO", hashValue);

                    Console.WriteLine(post);
                    res = conn.Execute(sql, param);
                    return res > 0;

                }
                catch (Exception e)
                {

                    throw new ApplicationException(e.Message);
                }
            }

        }


        public string GetFirstMonth(int matricula)
        {


            string sql = @"SELECT to_char(acc.datacc,'YYYY-MM-DD') DataRef                               
                                FROM r070acc acc
                                JOIN r034fun  fun
                                    ON ( fun.numemp = acc.numemp
                                AND fun.tipcol = acc.tipcol
                                AND fun.numcad = acc.numcad )                               
                              WHERE acc.codrlg    = 20
                                AND fun.numcad    = :matricula
                                order by acc.datacc  asc
                                fetch first row only";

            var param = new DynamicParameters();
            param.Add(":matricula", matricula);

            using (var conn = _connection.Connection())
            {
                try
                {
                    var res = conn.ExecuteScalar<string>(sql, param);
                    return res;

                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message);
                }
            }
        }
    }
}