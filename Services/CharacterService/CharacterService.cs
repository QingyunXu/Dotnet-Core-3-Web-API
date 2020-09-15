using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CharacterService(AppDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._mapper = mapper;
            this._context = context;
        }

        private int GetUserId() => int.Parse(this._httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        private string GetUserRole() => this._httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto addCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            Character character = this._mapper.Map<Character>(addCharacter);
            character.User = await this._context.Users.FirstOrDefaultAsync(u => u.Id == this.GetUserId());
            this._context.Characters.Add(character);
            this._context.SaveChanges();
            List<Character> characters = await this._context.Characters.Where(c => c.User.Id == this.GetUserId()).ToListAsync();
            response.Data = characters.Select(c => this._mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = await this._context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == this.GetUserId());
                if (character != null)
                {
                    this._context.Characters.Remove(character);
                    this._context.SaveChanges();

                    List<Character> characters = await this._context.Characters.Where(c => c.User.Id == this.GetUserId()).ToListAsync();
                    response.Data = characters.Select(c => this._mapper.Map<GetCharacterDto>(c)).ToList();
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> characters = this.GetUserRole().Equals("Admin") ?
                await this._context.Characters.ToListAsync() :
                await this._context.Characters.Where(c => c.User.Id == this.GetUserId()).ToListAsync();
            response.Data = characters.Select(c => this._mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            Character character = await this._context.Characters
                                    .Include(c => c.Weapon)
                                    .Include(c => c.CharacterSkills)
                                    .ThenInclude(cs => cs.Skill)
                                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == this.GetUserId());
            response.Data = this._mapper.Map<GetCharacterDto>(character);
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await this._context.Characters.Include(c => c.User).FirstAsync(c => c.Id == updateCharacter.Id);
                if (character.User.Id == this.GetUserId())
                {
                    character.Name = updateCharacter.Name;
                    character.HitPoints = updateCharacter.HitPoints;
                    character.Defense = updateCharacter.Defense;
                    character.Strength = updateCharacter.Strength;
                    character.Intelligence = updateCharacter.Intelligence;
                    character.Class = updateCharacter.Class;
                    this._context.Characters.Update(character);
                    await this._context.SaveChangesAsync();
                    response.Data = this._mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    response.Success = false;
                    response.Message = "Character not found";
                }
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
            throw new System.NotImplementedException();
        }
    }
}