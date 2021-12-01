using System.Data;

namespace termoRefeicoes.Interfaces
{
    public interface IConnectionFactory
    {
        IDbConnection Connection();
    }
}