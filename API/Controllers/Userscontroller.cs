using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using API.Extensions;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class Userscontroller : BaseApicontroller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public Userscontroller(IUserRepository userRepository,IMapper mapper,IPhotoService photoService)
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
            this._photoService = photoService;
        }

        [HttpGet]
        // [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            //var user=await _userRepository.GetUserByUserNameAsync(User.GetUserName());
            //userParams.CurrentUserName=user.UserName;
            // if(string.IsNullOrEmpty(userParams.Gender))
            // {
            //     userParams.Gender=user.Gender=="male"?"female":"male";
            // }
            var users = await _userRepository.GetMembersAsyns(userParams);
            Response.AddPaginationHeader(users.PageNumber,users.ItemsPerPage,users.TotalItems,users.TotalPages);
            return Ok(users);
        }


        // [Authorize(Roles ="Member")]
        [HttpGet("{userName}",Name ="GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string userName)
        {
            return await _userRepository.GetMemberByUserNameAsync(userName);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberEditDto memberEditDto)
        {
            var username= User.GetUserName();

            var user= await _userRepository.GetUserByUserNameAsync(username);

            _mapper.Map(memberEditDto,user);

             _userRepository.Update(user);

             if(await _userRepository.SaveAllAsync()) return NoContent();

             return BadRequest("Profile update failed");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> UploadImageAsync(IFormFile file)
        {
            if(file.Length ==0) return BadRequest("No image selected for upload");

            var user = await _userRepository.GetUserByUserNameAsync(User.GetUserName());

            var result = _photoService.UploadImageAsync(file);

            if(result.Result.Error ==null) 
            {

            }
            else
            {
                return BadRequest(result.Result.Error.Message);
            }

            var photo=new Photo()
            {
                IsMain=user.Photos.Count() >0 ?false:true,
                PublicId=result.Result.PublicId,
                Url=result.Result.SecureUrl.AbsoluteUri
            };

            user.Photos.Add(photo);

            if(await _userRepository.SaveAllAsync())
               return CreatedAtRoute("GetUser",new{userName=User.GetUserName()},_mapper.Map<PhotoDto>(photo));

            return BadRequest("Error while uploading image");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
           var user= await _userRepository.GetUserByUserNameAsync(User.GetUserName());
           var photo= user.Photos.FirstOrDefault(x=>x.Id==photoId);
           if(photo ==null) return NotFound("Photo not found for delete");
           if(photo.IsMain) return BadRequest("You can not delete your main photo");
           if(photo.PublicId!=null)
           {
               var deletionResult= await _photoService.DeleteImageAsync(photo.PublicId);
               if(deletionResult.Error!=null) return BadRequest(deletionResult.Error.Message);
           }

           user.Photos.Remove(photo);
           if(await _userRepository.SaveAllAsync()) return Ok();

           return BadRequest("Failed to delete photo");

        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> UpdateMainPhoto(int photoId)
        {
            var user=await _userRepository.GetUserByUserNameAsync(User.GetUserName());
            
            var photo= user.Photos.FirstOrDefault(x=>x.Id==photoId);
            if(photo.IsMain) return BadRequest("Photo already is your main photo");
            
            var currentPhoto=user.Photos.FirstOrDefault(x=>x.IsMain==true);

            currentPhoto.IsMain=false;
            photo.IsMain=true;

            if(await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Error while updating photo as a Main photo");
        }
        
    }
}