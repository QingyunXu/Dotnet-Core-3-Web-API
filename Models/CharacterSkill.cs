namespace Dotnet_Core_Web_API.Models
{
    public class CharacterSkill
    {
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}