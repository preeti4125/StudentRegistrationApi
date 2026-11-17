using StudentManagementApi.Models;

namespace StudentManagementApi.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}