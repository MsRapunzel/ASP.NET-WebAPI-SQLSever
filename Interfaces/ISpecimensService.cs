using testSQLServer.Models;

namespace testSQLServer.Interfaces
{
    public interface ISpecimensService
    {
        Task<IEnumerable<Species>> GetAllSpecimensData();
    }
}

