using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using API.Helpers;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context,IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(x=>x.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.Include(x=>x.Photos)
                            .SingleOrDefaultAsync(x=>x.UserName==userName);
        }

        public async Task<AppUser> GetUserByIdAsync(int Id)
        {
            return await _context.Users.FindAsync(Id);
        }

        
        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State=EntityState.Modified;
        }

        public async Task<PagedList<MemberDto>> GetMembersAsyns(UserParams userParams)
        {
           var source= _context.Users.AsQueryable();//.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking();
           source=source.Where(u=>u.UserName != userParams.CurrentUserName);
           source=source.Where(u=>u.Gender == userParams.Gender);
           var minDob=DateTime.Today.AddYears(-userParams.MaxAge-1);
           var maxDob=DateTime.Today.AddYears(-userParams.MinAge);
           source=source.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth <=maxDob);
           source=userParams.OrderBy switch{
               "created"=>source.OrderByDescending(u=>u.Created),
               _ => source.OrderByDescending(u=>u.LastActive)
           };


           return await PagedList<MemberDto>.CreateAsync(source.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),userParams.PageNumber,userParams.PageSize);
        }

        public async Task<MemberDto> GetMemberByUserNameAsync(string userName)
        {
            return await _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x=>x.UserName==userName);
        }
    }
}