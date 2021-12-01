using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services
{
    public interface IConsumosAceitosReport
    {
        List<Termo> Consultar(ReportFilters filtros);

        List<Termo> DadosExcel(ReportFilters filtros);
    }

}