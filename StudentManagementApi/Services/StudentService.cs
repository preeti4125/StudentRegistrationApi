using StudentManagementApi.DTOs;
using StudentManagementApi.Models;
using StudentManagementApi.Repositories.Interfaces;
using StudentManagementApi.Services.Interfaces;

namespace StudentManagementApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository) => _repository = repository;

        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(string? search)
        {
            var students = await _repository.GetAllAsync(search);
            return students.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                Department = s.Department,
                Age = s.Age
            });
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
        {
            var s = await _repository.GetByIdAsync(id);
            if (s == null) return null;

            return new StudentResponseDto { Id = s.Id, Name = s.Name, Email = s.Email, Department = s.Department, Age = s.Age };
        }

        public async Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto dto)
        {
            var student = new Student { Name = dto.Name, Email = dto.Email, Department = dto.Department, Age = dto.Age };
            var created = await _repository.AddAsync(student);

            return new StudentResponseDto { Id = created.Id, Name = created.Name, Email = created.Email, Department = created.Department, Age = created.Age };
        }

        public async Task<ServiceResult> UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null)
                return new ServiceResult { Succeeded = false, ErrorType = "NotFound", Message = "Student not found." };

            if (await _repository.EmailExistsAsync(dto.Email, id))
                return new ServiceResult { Succeeded = false, ErrorType = "Conflict", Message = "Email already in use." };

            student.Name = dto.Name;
            student.Email = dto.Email;
            student.Department = dto.Department;
            student.Age = dto.Age;

            await _repository.UpdateAsync(student);
            return new ServiceResult { Succeeded = true };
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return false;

            await _repository.DeleteAsync(student);
            return true;
        }
    }
}