using StudentManagementApi.Models;

namespace StudentManagementApi.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync(string? search);
        Task<Student?> GetByIdAsync(int id);
        Task<Student> AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        Task<bool> EmailExistsAsync(string email, int excludeId = 0);
    }
}