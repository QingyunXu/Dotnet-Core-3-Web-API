using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Dto.WeaponDto;
using Dotnet_Core_Web_API.Services;
using Dotnet_Core_Web_API.Services.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Core_Web_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("{controller}")]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;
        public WeaponController(IWeaponService weaponService)
        {
            this._weaponService = weaponService;
        }

        [HttpPost]
        public async Task<IActionResult> AddWeapon(AddWeaponDto addWeapon)
        {
            ServiceResponse<GetCharacterDto> response = await this._weaponService.AddWeapon(addWeapon);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}