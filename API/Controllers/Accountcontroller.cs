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
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{    
    public class Accountcontroller : BaseApicontroller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public Accountcontroller(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            this._mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await IsUserExists(registerDto.Username)) return BadRequest("User already taken");

            var user=_mapper.Map<AppUser>(registerDto);

            user.UserName=registerDto.Username.ToLower();

           var result= await _userManager.CreateAsync(user,"Pa$$w0rd");

           if(result.Succeeded==false) return BadRequest(result.Errors);

           var roleResult= await _userManager.AddToRoleAsync(user,"Member");
           
           if(roleResult.Succeeded==false) return BadRequest(roleResult.Errors);

           return new UserDto(){
               Username=user.UserName,
               Token= await _tokenService.CreateToken(user),
               KnownAs=user.KnownAs,
               Gender=user.Gender
           };
        }  

        private async Task<bool> IsUserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }  

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto) 
        {
            var User= await _userManager.Users.Include(x=>x.Photos).SingleOrDefaultAsync(x=>x.UserName==loginDto.Username.ToLower());

            if(User==null) return Unauthorized("User not found");

            var result= await _signInManager.CheckPasswordSignInAsync(User,loginDto.Password,false);
            if(result.Succeeded == false) return Unauthorized();
           
            return new UserDto(){
                Username=User.UserName,
                Token= await _tokenService.CreateToken(User),
                PhotoUrl=User.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                KnownAs=User.KnownAs,
                Gender=User.Gender
            };
        }   
    }
}