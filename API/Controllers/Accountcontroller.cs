using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Data;
using API.Entities;
using API.DTOs;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
namespace API.Controllers
{    
    public class Accountcontroller : BaseApicontroller
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public Accountcontroller(DataContext context,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await IsUserExists(registerDto.Username)) return BadRequest("User already taken");

            using var hmac=new HMACSHA512();

            var user=new AppUser()
            {
                UserName=registerDto.Username.ToLower(),
                PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

           await _context.Users.AddAsync(user);
           _context.SaveChanges();

           return new UserDto(){
               Username=user.UserName,
               Token=_tokenService.CreateToken(user)
           };
        }  

        private async Task<bool> IsUserExists(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }  

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var User= await _context.Users.SingleOrDefaultAsync(x=>x.UserName==loginDto.Username.ToLower());
            if(User==null)
            return Unauthorized("User not found");

            var key = User.PasswordSalt;
            var hmac=new HMACSHA512(key);
            var computedPassword=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i=0;i<computedPassword.Length;i++)
            {
                if(computedPassword[i]!=User.PasswordHash[i])
                return Unauthorized("Invalid password");
            }

            return new UserDto(){
                Username=User.UserName,
                Token=_tokenService.CreateToken(User)
            };
        }   
    }
}