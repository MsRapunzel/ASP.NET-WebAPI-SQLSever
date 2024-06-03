using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Dapper;
using testSQLServer.Models;
using testSQLServer.Interfaces;

namespace testSQLServer.Services;


public class StaffService : IStaffService
{
    private readonly IDapperDbContext _connection;

    public enum OrderBy
    {
        name,
        position,
        contact_email,
        contact_number,
    }

    public StaffService(IDapperDbContext connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Staff>> GetStaffAsync()
    {
        using var connection = _connection.CreateConnection();
        var sql = $@"SELECT 
		                     [staff_id]
		                    ,[department_id]
		                    ,[name]
		                    ,[position]
		                    ,[contact_email]
		                    ,[contact_number]
                        FROM [dbo].[Staff] ";
        return await connection.QueryAsync<Staff>(sql);
    }

    public async Task<Staff> GetStaffByIdAsync(int staffId)
    {
        using var connection = _connection.CreateConnection();

        var isRecord = await IsRecordAsync(staffId);
        if (!isRecord)
        {
            throw new ArgumentException($"There is no Staff with id {staffId}");
        }

        var sql = $@"SELECT 
		                     [staff_id]
		                    ,[department_id]
		                    ,[name]
		                    ,[position]
		                    ,[contact_email]
		                    ,[contact_number]
                        FROM [dbo].[Staff]
                        WHERE [staff_id] = @Staff_id";

        var id = new { Staff_id = staffId };
        return await connection.QueryFirstAsync<Staff>(sql, id);
    }

    public async Task<bool> DeleteStaffAsync(int staffId)
    {
        using var connection = _connection.CreateConnection();
        {
            connection.Open();

            using var tran = connection.BeginTransaction();
            {

                var sql = "DELETE FROM Staff WHERE staff_id = @Staff_Id";
                var id = new { Staff_id = staffId };
                var rowsAffected = await connection.ExecuteAsync(sql: sql, param: id, transaction: tran);

                if (rowsAffected == 0)
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
        }
    }

    public async Task UpdateStaffAsync(int id, Staff staff)
    {
        using var connection = _connection.CreateConnection();
        connection.Open();

        using var tran = connection.BeginTransaction();
        var isRecord = await IsRecordAsync(id);
        if (!isRecord)
        {
            throw new ArgumentException($"There is no Staff with id {id}");
        }

        var sql = new StringBuilder("UPDATE [dbo].[Staff] SET ");
        var parameters = new DynamicParameters();

        var properties = typeof(Staff).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            var value = prop.GetValue(staff);
            if (!value.Equals("") && !value.Equals(0))
            {
                sql.Append($"[{prop.Name.ToLower()}] = @{prop.Name}, ");
                parameters.Add($"@{prop.Name}", value);
            }
        }

        sql.Length -= 2;

        sql.Append($" WHERE [staff_Id] = {id}");

        try
        {
            await connection.ExecuteAsync(sql.ToString(), parameters, tran);
            tran.Commit();
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            tran.Rollback();
            throw new ArgumentException($"Error occured: the email or number already exists.");
        }
    }

    public async Task<Staff> CreateStaffAsync(Staff staff)
    {
        using var connection = _connection.CreateConnection();
        var p = new DynamicParameters();
        p.Add("@department_id", staff.Department_id);
        p.Add("@name", staff.Name, DbType.String, size: 255);
        p.Add("@position", staff.Position, DbType.String, size: 255);
        p.Add("@contact_email", staff.Contact_email, DbType.String, size: 255);
        p.Add("@contact_number", staff.Contact_number, DbType.String, size: 15);
        p.Add("@new_staff_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@result_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@error_message", dbType: DbType.String, direction: ParameterDirection.Output, size: 255);

        connection.Execute("ProcAddNewStaff", p, commandType: CommandType.StoredProcedure);

        int result_code = p.Get<int>("@result_code");

        if (result_code == 0)
        {
            int new_staff_id = p.Get<int>("@new_staff_id");
            return await GetStaffByIdAsync(new_staff_id);
        }
        else
        {
            string error_message = p.Get<string>("@error_message");
            throw new ArgumentException($"Error occured {error_message} Exit code: {result_code}");
        }
    }

    public async Task<IEnumerable<Staff>> GetFilterStaffAsync(string? name,
                                                     string? position,
                                                     string? contactEmail,
                                                     string? contactNumber,
                                                     int? departmentId,
                                                     OrderBy? orderBy,
                                                     bool descending = false)
    {
        using var connection = _connection.CreateConnection();
        var query = new StringBuilder("SELECT * FROM Staff WHERE 1=1");

        if (!string.IsNullOrEmpty(name))
        {
            query.Append(" AND Name LIKE @Name");
        }

        if (!string.IsNullOrEmpty(position))
        {
            query.Append(" AND Position LIKE @Position");
        }

        if (!string.IsNullOrEmpty(contactEmail))
        {
            query.Append(" AND Contact_email LIKE @ContactEmail");
        }

        if (!string.IsNullOrEmpty(contactNumber))
        {
            query.Append(" AND Contact_number LIKE @ContactNumber");
        }

        if (departmentId.HasValue)
        {
            query.Append(" AND Department_id = @Department_id");
        }

        if (!string.IsNullOrEmpty(orderBy.ToString()))
        {
            query.Append(" ORDER BY " + orderBy);

            if (descending)
            {
                query.Append(" DESC");
            }
            else
            {
                query.Append(" ASC");
            }
        }

        return await connection.QueryAsync<Staff>(query.ToString(),
                                                  new
                                                  {
                                                      Name = $"%{name}%",
                                                      Position = $"%{position}%",
                                                      ContactEmail = $"%{contactEmail}%",
                                                      ContactNumber = $"%{contactNumber}%",
                                                      Department_id = departmentId
                                                  });
    }

    private async Task<bool> IsRecordAsync(int staffId)
    {
        using var connection = _connection.CreateConnection();
        var sql = "SELECT COUNT(1) FROM Staff WHERE staff_id = @Staff_Id";
        var id = new { Staff_id = staffId };
        var result = await connection.ExecuteScalarAsync<bool>(sql, id);
        return result;
    }
    /* public async Task<Staff> CreateStaffAsync(Staff staff) // Deprecated
    {
        using var connection = _connection.CreateConnection();
        var sql = $@"INSERT INTO [dbo].[Staff]
                            (
	                             [department_id]
	                            ,[name]
	                            ,[position]
	                            ,[contact_email]
	                            ,[contact_number]
                            )
                    VALUES
                            (
	                             @Department_id
                                ,@Name
                                ,@Position
                                ,@Contact_email
                                ,@Contact_number
                            )";
        await connection.ExecuteAsync(sql, staff);
        return staff;
    }
    */
}
