using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services
{
    public interface IRefeicoes
    {
        Task<IEnumerable<Refeicoes>> Consultar(string competencia);
        int GetCountAccept(string competencia);
        int GetTermoAceite(int matricula);
        string GetFirstMonth(int matricula);
        object SaveTerm(Termo obj);
    }
}