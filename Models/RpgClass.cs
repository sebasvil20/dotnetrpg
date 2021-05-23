
using System.Collections.Generic;

namespace dotnet_rpg.Models
{
    public class RpgClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        List<Skill> Skills {get; set;}
        List<Character> Characters {get; set;}
    }
}