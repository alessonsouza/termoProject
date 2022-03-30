using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services
{
    public interface IConfigService
    {
        Task<Config> getConfig();
        Task<int> saveConfig(Config dados);

    }
}