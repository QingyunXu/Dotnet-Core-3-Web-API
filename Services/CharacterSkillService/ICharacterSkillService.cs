using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Dto.CharacterSkillDto;

namespace Dotnet_Core_Web_API.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
        Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto);
    }
}