using termoRefeicoes.Models;

namespace termoRefeicoes.Interfaces.Services.Security
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}