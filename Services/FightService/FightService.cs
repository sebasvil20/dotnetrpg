using System;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FightService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            
            ServiceResponse<AttackResultDto> ServiceResponse = new ServiceResponse<AttackResultDto>();
            try{
                Character attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                Character opponent = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                if(attacker.RpgClassId == 2) damage += (new Random().Next(attacker.Intelligence));
                damage -= new Random().Next(opponent.Defense);

                if(damage > 0) opponent.HitPoints -= damage;
                if(opponent.HitPoints <= 0) {
                    opponent.HitPoints = 0;
                    ServiceResponse.Message = $"{opponent.Name} has been defeated!";
                }

                _context.Characters.Update(opponent);
                await _context.SaveChangesAsync();

                ServiceResponse.Data = new AttackResultDto {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.HitPoints,
                    Opponent = opponent.Name,
                    OpponentHp = opponent.HitPoints,
                    Damage = damage
                };
            }
            catch(Exception){
                ServiceResponse.Sucess = false;
                ServiceResponse.Message = "";
            }
            return ServiceResponse;
        }
    }
}