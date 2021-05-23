using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.CharacterSkill;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterSkillService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkilldto newCharacterSkill)
        {
            
            ServiceResponse<GetCharacterDto> ServiceResponse = new ServiceResponse<GetCharacterDto>();
            try{
                Character character = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.RpgClass)
                    .Include(c => c.CharacterSkill).ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId &&
                    c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier)));
                
                if(character == null){
                    ServiceResponse.Sucess = false;
                    ServiceResponse.Message = "Character not found.";
                    return ServiceResponse;
                }

                Skill skill = await _context.Skills
                    .FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId && s.RpgClass == character.RpgClass);
                
                if(skill == null){
                    ServiceResponse.Sucess = false;
                    ServiceResponse.Message = "Skill not found or Class is not allowed to use this skill.";
                    return ServiceResponse;
                }

                CharacterSkill characterSkill = new CharacterSkill{
                    Character = character,
                    Skill = skill
                };
                await _context.CharacterSkills.AddAsync(characterSkill);
                await _context.SaveChangesAsync();

                ServiceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            
            catch(Exception ex){
                ServiceResponse.Sucess = false;
                ServiceResponse.Message = "Your character already has that skill";
            }
            return ServiceResponse;
        }
    }
}