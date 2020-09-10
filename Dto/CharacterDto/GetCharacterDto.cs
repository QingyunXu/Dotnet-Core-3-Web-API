using Dotnet_Core_Web_API.Models;

namespace Dotnet_Core_Web_API.Dto.CharacterDto
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HitPoints { get; set; }
        public int Strength { get; set; }
        public int Defense { get; set; }
        public int Intelligence { get; set; }
        public RpgClass Class { get; set; }
    }
}