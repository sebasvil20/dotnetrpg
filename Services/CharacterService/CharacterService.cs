using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);
            character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();
            ServiceResponse.Data = await _context.Characters
                .Include(c => c.RpgClass)
                .Include(c => c.Weapon)
                .Include(c => c.CharacterSkill).ThenInclude(c => c.Skill)
                .Where(c => c.User.Id == GetUserId())
                .Select(c => _mapper.Map<GetCharacterDto>(c))
                .ToListAsync();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                if(character != null){
                    _context.Characters.Remove(character);
                    await _context.SaveChangesAsync();
                    ServiceResponse.Data = _context.Characters
                        .Where(c => c.User.Id == GetUserId())
                        .Select(c => _mapper.Map<GetCharacterDto>(c))
                        .ToList();
                }
                else{
                    ServiceResponse.Sucess = false;
                    ServiceResponse.Message = "Character not found.";
                }
            }
            catch (Exception ex)
            {
                ServiceResponse.Sucess = false;
                ServiceResponse.Message = ex.Message;
            }
            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters
                    .Include(c => c.RpgClass)
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkill)
                    .ThenInclude(cs => cs.Skill)
                    .Where(c => c.User.Id == GetUserId())
                    .ToListAsync();
            ServiceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters
                    .Include(c => c.RpgClass)
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkill)
                    .ThenInclude(cs => cs.Skill)
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                    
            if(dbCharacter != null) ServiceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            else{
                ServiceResponse.Sucess = false;
                ServiceResponse.Message = "Character not found.";
            }
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .Include(c => c.RpgClass)
                    .Include(c => c.User)
                    .Include(c => c.Weapon)
                    .Include(c => c.CharacterSkill)
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if(character.User.Id == GetUserId()){

                    character.Name = updatedCharacter.Name;
                    character.HitPoints = updatedCharacter.HitPoints;
                    character.Strength = updatedCharacter.Strength;
                    character.Defense = updatedCharacter.Defense;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.RpgClassId = updatedCharacter.RpgClassId;

                    _context.Characters.Update(character);
                    await _context.SaveChangesAsync();

                    ServiceResponse.Data = _mapper.Map<GetCharacterDto>(character);

                }
                else{
                    ServiceResponse.Sucess = false;
                    ServiceResponse.Message = "Character not found.";
                }

            }
            catch (Exception ex)
            {
                ServiceResponse.Sucess = false;
                ServiceResponse.Message = ex.Message;
            }
            return ServiceResponse;

        }
    }
}