using System.Linq;
using AutoMapper;
using Dotnet_Core_Web_API.Dto.CharacterDto;
using Dotnet_Core_Web_API.Dto.SkillDto;
using Dotnet_Core_Web_API.Dto.WeaponDto;
using Dotnet_Core_Web_API.Models;

namespace Dotnet_Core_Web_API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>().ForMember(dto => dto.Skills, c => c.MapFrom(c => c.CharacterSkills.Select(cs => cs.Skill)));
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
        }
    }
}