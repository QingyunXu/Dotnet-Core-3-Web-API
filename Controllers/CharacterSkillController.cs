using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Dto.CharacterSkillDto;
using Dotnet_Core_Web_API.Services;
using Dotnet_Core_Web_API.Services.CharacterSkillService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Core_Web_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService _characterSkillService;
        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            this._characterSkillService = characterSkillService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto)
        {
            ServiceResponse<GetCharacterDto> response = await this._characterSkillService.AddCharacterSkill(addCharacterSkillDto);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}