using System;

namespace termoRefeicoes.Helper
{
    public static class CompetenciaHelper
    {

        private static readonly string startDay = "26";
        private static readonly string endDay = "26";

        public static string GetEndDate(string competencia)
        {
            return competencia + endDay;
        }

        public static string GetStartDate(string competencia)
        {
            var ano = Int32.Parse(competencia.Substring(0, 4));
            var mes = Int32.Parse(competencia.Substring(4, 2));

            mes -= 1;
            if (mes < 1)
            {
                mes = 12;
                ano -= 1;
            }

            string dataInicial = ano + mes.ToString().PadLeft(2, '0') + startDay;
            return dataInicial;
        }
    }
}