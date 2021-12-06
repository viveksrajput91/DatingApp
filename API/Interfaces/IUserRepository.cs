using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.DTOs;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByUserNameAsync(string userName);

        Task<IEnumerable<MemberDto>> GetMembersAsyns();

        Task<MemberDto> GetMemberByUserNameAsync(string userName);

        Task<bool> SaveAllAsync();
        void Update(AppUser user);
    }
}