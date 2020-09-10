using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.UserDto;

namespace Dotnet_Core_Web_API.Services.Auth
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(RegisterDto registerUser);
        Task<ServiceResponse<string>> Login(LoginDto loginUser);
        Task<bool> UserExists(string username);
    }
}