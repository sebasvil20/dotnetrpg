using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            ServiceResponse<GetCharacterDto> ServiceResponse = new ServiceResponse<GetCharacterDto>();
            try{
                Character character = await _context.Characters
                    .Include(c => c.RpgClass)
                    .Include(c => c.CharacterSkill).ThenInclude(c => c.Skill)
                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId &&
                    c.User.Id == int.Parse(_httpContextAccessor.HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier)));
                
                if(character == null){
                    ServiceResponse.Sucess = false;
                    ServiceResponse.Message = "Character not found.";
                    return ServiceResponse;
                }

                Weapon weapon = new Weapon{
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };
                await _context.Weapons.AddAsync(weapon);
                await _context.SaveChangesAsync();

                ServiceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch(Exception ex){
                ServiceResponse.Sucess = false;
                ServiceResponse.Message = ex.Message;
            }
            return ServiceResponse;
        }
    }
}