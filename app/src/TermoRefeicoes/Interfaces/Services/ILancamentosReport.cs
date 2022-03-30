using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services
{
    public interface ILancamentosReport
    {
        Task<IEnumerable<Lancamentos>> Consultar(ReportFilters filtros);

        List<Lancamentos> DadosExcel(ReportFilters filtros);
    }

}