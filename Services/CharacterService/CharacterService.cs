using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDto>>();
            Character character = _mapper.Map<Character>(newCharacter);
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            ServiceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                Character character = await _context.Characters.FirstAsync(c => c.Id == id);
                _context.Characters.Remove( character);
                await _context.SaveChangesAsync();
                ServiceResponse.Data = _context.Characters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList(); ;
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
            var dbCharacters = await _context.Characters.ToListAsync();
            ServiceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacters = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            ServiceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacters);
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await _context.SaveChangesAsync();

                ServiceResponse.Data = _mapper.Map<GetCharacterDto>(character);
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