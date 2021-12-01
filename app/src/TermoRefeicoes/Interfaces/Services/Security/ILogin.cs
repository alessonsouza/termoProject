using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services.Security
{
    public interface ILogin
    {
        Task<ResponseLogin> Authenticate(Login login);
    }
}