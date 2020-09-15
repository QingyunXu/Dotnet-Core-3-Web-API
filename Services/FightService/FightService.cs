using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.FightDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public FightService(AppDbContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest)
        {
            ServiceResponse<FightResultDto> response = new ServiceResponse<FightResultDto> { Data = new FightResultDto() };
            try
            {
                List<Character> characters =
                    await this._context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                    .Where(c => fightRequest.CharacterIds.Contains(c.Id)).ToListAsync();
                bool defeated = false;
                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        Character opponent = opponents[new Random().Next(opponents.Count)];
                        int damage = 0;
                        string attackUsed = string.Empty;
                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);
                        }
                        else
                        {
                            int randomSkill = new Random().Next(attacker.CharacterSkills.Count);
                            attackUsed = attacker.CharacterSkills[randomSkill].Skill.Name;
                            damage = DoSkillAttack(attacker, opponent, attacker.CharacterSkills[randomSkill]);
                        }
                        response.Data.Log.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has beed defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;

                });
                this._context.Characters.UpdateRange(characters);
                await this._context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await this._context.Characters
                                        .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                                        .FirstOrDefaultAsync(c => c.Id == skillAttack.AttackerId);
                Character opponent = await this._context.Characters.FirstOrDefaultAsync(c => c.Id == skillAttack.OpponentId);
                CharacterSkill characterSkill = attacker.CharacterSkills.FirstOrDefault(cs => cs.Skill.Id == skillAttack.SkillId);
                if (characterSkill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know this skill.";
                }
                else
                {
                    int damage = DoSkillAttack(attacker, opponent, characterSkill);
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
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, CharacterSkill characterSkill)
        {
            int damage = characterSkill.Skill.Damage + (new Random().Next(attacker.Intelligence));
            int newDamage = damage * (new Random().Next(opponent.Defense)) / 80;
            if (newDamage > 0)
                opponent.HitPoints -= newDamage;
            return newDamage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            ServiceResponse<AttackResultDto> response = new ServiceResponse<AttackResultDto>();
            try
            {
                Character attacker = await this._context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == weaponAttack.AttackerId);
                Character opponent = await this._context.Characters.FirstOrDefaultAsync(c => c.Id == weaponAttack.OpponentId);
                int damage = DoWeaponAttack(attacker, opponent);
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

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            int newDamage = damage * (new Random().Next(opponent.Defense)) / 80;
            if (newDamage > 0)
                opponent.HitPoints -= newDamage;
            return newDamage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            ServiceResponse<List<HighScoreDto>> response = new ServiceResponse<List<HighScoreDto>>();
            try
            {
                List<Character> characters = await this._context.Characters
                                                .Where(c => c.Fights > 0)
                                                .OrderByDescending(c => c.Victories)
                                                .ThenBy(c => c.Defeats)
                                                .ToListAsync();
                response.Data = characters.Select(c => this._mapper.Map<HighScoreDto>(c)).ToList();
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