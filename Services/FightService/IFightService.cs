using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.FightDto;

namespace Dotnet_Core_Web_API.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
        Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack);
        Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest);
        Task<ServiceResponse<List<HighScoreDto>>> GetHighScore();
    }
}