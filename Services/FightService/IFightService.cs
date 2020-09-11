using System.Threading.Tasks;
using Dotnet_Core_Web_API.Dto.FightDto;

namespace Dotnet_Core_Web_API.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
    }
}