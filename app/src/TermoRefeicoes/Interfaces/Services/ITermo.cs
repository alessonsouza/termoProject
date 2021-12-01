using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services
{
    public interface ITermo
    {
        bool Submit(string competencia, int matricula, int hora);
        Task<IEnumerable<TermoVersion>> GetTermo();
    }
}