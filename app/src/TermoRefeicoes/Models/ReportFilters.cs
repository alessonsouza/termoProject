using System;

namespace termoRefeicoes.Models
{
    public class ReportFilters
    {
        public int id { get; set; }
        public int NUMCAD { get; set; }
        public string DATA_ACEITE { get; set; }
        public int HORA_ACEITE { get; set; }
        public string TERMO_DESCRICAO { get; set; }
        public string NOMFUN { get; set; }
        public string CODCCU { get; set; }
        public string NOMCCU { get; set; }
        public string DATA_INICIO { get; set; }
        public string DATA_FIM { get; set; }
        public string JAHACEITOU { get; set; }
    }
}