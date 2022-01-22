using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.DTOs;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int Id);

        Task<AppUser> GetUserByUserNameAsync(string userName);

        Task<PagedList<MemberDto>> GetMembersAsyns(UserParams userParams);

        Task<MemberDto> GetMemberByUserNameAsync(string userName);

        Task<bool> SaveAllAsync();
        void Update(AppUser user);
    }
}