using System.Collections.Generic;
using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services
{
    public interface IUserSenior
    {
        Task<IEnumerable<Users>> GetUsers(string username);
        Task<IEnumerable<Users>> GetUsersByNumCAd(int matricula);
    }
}