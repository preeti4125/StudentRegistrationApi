using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Models;
using StudentManagementApi.Repositories.Interfaces;

namespace StudentManagementApi.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Student>> GetAllAsync(string? search)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Name.Contains(search) || s.Department.Contains(search));
            }
            return await query.ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id) => await _context.Students.FindAsync(id);

        public async Task<Student> AddAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int excludeId = 0) =>
            await _context.Students.AnyAsync(s => s.Email == email && s.Id != excludeId);
    }
}