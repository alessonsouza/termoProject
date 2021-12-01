using System;
using System.Collections.Generic;

namespace termoRefeicoes.Models
{
    public class Termo
    {
        public int ID { get; set; }
        public int NUMCAD { get; set; }
        public DateTime DATA_ACEITE { get; set; }
        public int HORA_ACEITE { get; set; }
        public string TERMO_DESCRICAO { get; set; }
        public string NOMFUN { get; set; }
        public string CODCCU { get; set; }
        public string NOMCCU { get; set; }
        public List<Termo> TermoList { get; set; }
        public int FK_TERMO { get; set; }

    }
}