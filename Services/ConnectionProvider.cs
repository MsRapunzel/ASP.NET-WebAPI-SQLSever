using System.Data;
using System.Data.SqlClient;

namespace testSQLServer.Services;



public class DapperDbContext : IDapperDbContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connection;

    public DapperDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connection = configuration.GetConnectionString("DefaultConnection")!;
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connection);
}




public interface IDapperDbContext
{
    IDbConnection CreateConnection();
}