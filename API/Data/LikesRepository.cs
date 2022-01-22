using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId,likedUserId);    
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users= _context.Users.AsQueryable();
            var userLikes=_context.Likes.AsQueryable();

            if(likesParams.Predicate=="liked")
            {
                userLikes=userLikes.Where(x=>x.SourceUserId==likesParams.UserId);
                users=userLikes.Select(x=>x.LikedUser);
            }

            if(likesParams.Predicate=="likedBy")
            {
                userLikes=userLikes.Where(x=>x.LikedUserId==likesParams.UserId);
                users=userLikes.Select(x=>x.SourceUser);
            }

            var likeDto= users.Select(x=>new LikeDto{
                Id=x.Id,
                UserName=x.UserName,
                KnownAs=x.KnownAs,
                Age=x.DateOfBirth.CalculateAge(),
                City=x.City,
                PhotoUrl=x.Photos.FirstOrDefault(x=>x.IsMain).Url
            });

            return await PagedList<LikeDto>.CreateAsync(likeDto,likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}