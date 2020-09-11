using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dotnet_Core_Web_API.Data;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Dto.WeaponDto;
using Dotnet_Core_Web_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet_Core_Web_API.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WeaponService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(this._httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto addWeapon)
        {
            ServiceResponse<GetCharacterDto> response = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await this._context.Characters.FirstOrDefaultAsync(c => c.Id == addWeapon.CharacterId && c.User.Id == this.GetUserId());
                if (character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found";
                }
                else
                {
                    Weapon weapon = new Weapon { Name = addWeapon.Name, Damage = addWeapon.Damage, Character = character };
                    await this._context.Weapons.AddAsync(weapon);
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