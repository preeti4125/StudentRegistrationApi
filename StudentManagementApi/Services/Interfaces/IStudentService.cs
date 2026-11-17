using StudentManagementApi.DTOs;

namespace StudentManagementApi.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync(string? search);
        Task<StudentResponseDto?> GetStudentByIdAsync(int id);
        Task<StudentResponseDto> CreateStudentAsync(CreateStudentDto dto);
        Task<ServiceResult> UpdateStudentAsync(int id, UpdateStudentDto dto);
        Task<bool> DeleteStudentAsync(int id);
    }

    public class ServiceResult
    {
        public bool Succeeded { get; set; }
        public string ErrorType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}