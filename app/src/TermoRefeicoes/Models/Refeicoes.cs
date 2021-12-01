using System;

namespace termoRefeicoes.Models
{
    public class Refeicoes
    {
        public int CodRefeicao { get; set; }
        public int NumCadastro { get; set; }
        public string DescRefeicao { get; set; }
        public DateTime DataRef { get; set; }
        public int HoraCC { get; set; }
        public int QTDAcc { get; set; }
        public double ValorRef { get; set; }
        public string Usu_Datchk { get; set; }
        public string Usu_Horchk { get; set; }

    }
}