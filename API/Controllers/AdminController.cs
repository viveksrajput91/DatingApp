using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApicontroller
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("users-with-roles")]
        [Authorize(Policy="UserWithAdminRolePolicy")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var result= await _userManager.Users.Include(u=>u.UserRole).ThenInclude(r=>r.Role).OrderBy(u=>u.UserName).Select(u=>new{
                Id=u.Id,
                Username=u.UserName,
                Roles=u.UserRole.Select(r=>r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(result);
        }

        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditUserRoles(string username,[FromQuery] string roles)
        {
            var selectedRoles= roles.Split(",").ToArray();
            var user= await _userManager.FindByNameAsync(username);
            if(user==null) return NotFound("User not found");
            var userRoles=await _userManager.GetRolesAsync(user);
            var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles));
            if(result.Succeeded==false) return BadRequest("Failed to add to role");
            result=await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles));
            if(result.Succeeded==false) return BadRequest("Failed to remove from role");
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpGet("photos-to-moderate")]
        [Authorize(Policy="UserWithAdminModeratorRolePolicy")]
        public ActionResult GetPhotosToModerate()
        {
            return Ok("Only user with Admin or Moderator role can access this method");
        }
    }
}