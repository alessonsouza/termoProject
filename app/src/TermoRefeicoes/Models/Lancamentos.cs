using System;

namespace termoRefeicoes.Models
{
    public class Lancamentos
    {
        public int NUMCAD { get; set; }
        public DateTime USU_DATCON { get; set; }
        public int USU_HORCON { get; set; }
        public int USU_CODREF { get; set; }
        public int USU_QTDREF { get; set; }
        public string USU_TPCAPT { get; set; }
        public string DESREF { get; set; }
        public string NOMFUN { get; set; }
        public string NOMCCU { get; set; }
    }
}