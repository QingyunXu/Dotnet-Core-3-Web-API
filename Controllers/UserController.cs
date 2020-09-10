using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.UserDto;
using Dotnet_Core_Web_API.Services;
using Dotnet_Core_Web_API.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Core_Web_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public UserController(IAuthRepository authRepository)
        {
            this._authRepository = authRepository;
        }

        [HttpPost("Regisger")]
        public async Task<IActionResult> Register(RegisterDto registerUser)
        {
            ServiceResponse<int> response = await this._authRepository.Register(registerUser);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginUser)
        {
            ServiceResponse<string> response = await this._authRepository.Login(loginUser);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

    }
}