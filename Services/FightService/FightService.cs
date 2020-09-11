using System;
using System.Threading.Tasks;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.FightDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly AppDbContext _context;
        public FightService(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await this._context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == weaponAttack.AttackerId);
                Character opponent = await this._context.Characters.FirstOrDefaultAsync(c => c.Id == weaponAttack.OpponentId);
                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                damage -= new Random().Next(opponent.Defense);
                if (damage > 0)
                    opponent.HitPoints -= damage;
                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has beed defeated";

                this._context.Characters.Update(opponent);
                await this._context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }
    }
}