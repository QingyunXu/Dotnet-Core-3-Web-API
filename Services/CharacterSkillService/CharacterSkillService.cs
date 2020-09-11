using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Dto.CharacterSkillDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        public CharacterSkillService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
            this._context = context;
        }

        private int GetUserId() => int.Parse(this._httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto addCharacterSkillDto)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await this._context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkills)
                    .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == addCharacterSkillDto.CharacterId && c.User.Id == this.GetUserId());
                Skill skill = await this._context.Skills.FirstOrDefaultAsync(s => s.Id == addCharacterSkillDto.SkillId);
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                }
                else if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found";
                }
                else
                {
                    CharacterSkill characterSkill = new CharacterSkill
                    {
                        Character = character,
                        Skill = skill
                    };
                    await this._context.CharacterSkills.AddAsync(characterSkill);
                    await this._context.SaveChangesAsync();
                    response.Data = this._mapper.Map<GetCharacterDto>(character);
                }
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