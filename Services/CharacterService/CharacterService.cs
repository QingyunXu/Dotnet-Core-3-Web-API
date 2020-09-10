using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public CharacterService(AppDbContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto addCharacter)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            this._context.Characters.Add(this._mapper.Map<Character>(addCharacter));
            this._context.SaveChanges();
            List<Character> characters = await this._context.Characters.ToListAsync();
            response.Data = characters.Select(c => this._mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = await this._context.Characters.FirstAsync(c => c.Id == id);
                this._context.Characters.Remove(character);
                this._context.SaveChanges();
                List<Character> characters = await this._context.Characters.ToListAsync();
                response.Data = characters.Select(c => this._mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters(int userId)
        {
            ServiceResponse<List<GetCharacterDto>> response = new ServiceResponse<List<GetCharacterDto>>();
            List<Character> characters = await this._context.Characters.Where(c => c.User.Id == userId).ToListAsync();
            response.Data = characters.Select(c => this._mapper.Map<GetCharacterDto>(c)).ToList();
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await this._context.Characters.FirstAsync(c => c.Id == id);
                response.Data = this._mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.Message = e.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updateCharacter)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await this._context.Characters.FirstAsync(c => c.Id == updateCharacter.Id);
                character.Name = updateCharacter.Name;
                character.HitPoints = updateCharacter.HitPoints;
                character.Defense = updateCharacter.Defense;
                character.Strength = updateCharacter.Strength;
                character.Intelligence = updateCharacter.Intelligence;
                character.Class = updateCharacter.Class;
                this._context.Characters.Update(character);
                this._context.SaveChanges();
                response.Data = this._mapper.Map<GetCharacterDto>(character);
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