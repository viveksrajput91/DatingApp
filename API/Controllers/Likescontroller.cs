using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Extensions;
using System.Security.Claims;
using API.Entities;
using API.DTOs;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class Likescontroller : BaseApicontroller
    {
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;

        public Likescontroller(ILikesRepository likesRepository,IUserRepository userRepository)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
        
        }

        [HttpPost("{userName}")]
        public async Task<ActionResult> AddLikes(string userName)
        {
            var sourceUserId= User.GetUserId();
            var sourceUser= await _likesRepository.GetUserWithLikes(sourceUserId);

            var LikedUser=await _userRepository.GetUserByUserNameAsync(userName);
            if(LikedUser==null) return NotFound("User not found");

            var likedUserId=LikedUser.Id;

            var userLike= await _likesRepository.GetUserLike(sourceUserId,likedUserId);
            if(userLike != null) return BadRequest("You already liked this user");

            if(userName==sourceUser.UserName) return BadRequest("You cannot like yourself");

            userLike=new UserLike{
                SourceUser=sourceUser,
                SourceUserId=sourceUserId,
                LikedUser=LikedUser,
                LikedUserId=likedUserId
            };

            sourceUser.LikedUsers.Add(userLike);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId=User.GetUserId();

            var likeDto= await _likesRepository.GetUserLikes(likesParams);
            Response.AddPaginationHeader(likeDto.PageNumber,likeDto.ItemsPerPage,likeDto.TotalItems,likeDto.TotalPages);
            return Ok(likeDto);
        }
    }
}