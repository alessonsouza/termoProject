using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces
{
    public interface ISetor
    {
        Task<IEnumerable<Setor>> GetAll();
    }
}