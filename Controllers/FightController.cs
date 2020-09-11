using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.FightDto;
using Dotnet_Core_Web_API.Services;
using Dotnet_Core_Web_API.Services.FightService;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Core_Web_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;
        public FightController(IFightService fightService)
        {
            this._fightService = fightService;
        }

        [HttpPost("WeaponAttack")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            ServiceResponse<AttackResultDto> response = await this._fightService.WeaponAttack(weaponAttack);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}