using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Services;
using Dotnet_Core_Web_API.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet_Core_Web_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            this._characterService = characterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> response = await this._characterService.GetAllCharacters();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> response = await this._characterService.GetCharacterById(id);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);

        }

        [HttpPost]
        public async Task<IActionResult> AddCharacter(AddCharacterDto addCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> response = await this._characterService.AddCharacter(addCharacter);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> response = await this._characterService.UpdateCharacter(updateCharacter);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = await this._characterService.DeleteCharacter(id);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}