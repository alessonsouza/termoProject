using System.Threading.Tasks;
using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services.Security
{
    public interface IAuthentication
    {
        Task<User> Autenthicate(string username, string password);
        bool BelongToGroup(string groupName);

    }
}