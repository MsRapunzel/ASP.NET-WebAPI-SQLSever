using testSQLServer.Models;
using testSQLServer.Services;

namespace testSQLServer.Interfaces
{
    public interface IStaffService
    {
        Task<Staff> CreateStaffAsync(Staff staff);
        Task<IEnumerable<Staff>> GetStaffAsync();
        Task<Staff> GetStaffByIdAsync(int staffId);
        Task UpdateStaffAsync(int id, Staff staff);
        Task<bool> DeleteStaffAsync(int staffId);

        Task<IEnumerable<Staff>> GetFilterStaffAsync(string? name,
                                               string? position,
                                               string? contactEmail,
                                               string? contactNumber,
                                               int? departmentId,
                                               StaffService.OrderBy? orderBy,
                                               bool descending = false);
    }
}

