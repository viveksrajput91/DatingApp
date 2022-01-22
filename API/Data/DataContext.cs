using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser,AppRole,int,IdentityUserClaim<int>,AppUserRole,
        IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>().HasKey(k=>new{k.SourceUserId,k.LikedUserId});

            builder.Entity<UserLike>().HasOne(like=>like.SourceUser).WithMany(y=>y.LikedUsers).HasForeignKey(x=>x.SourceUserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>().HasOne(x=>x.LikedUser).WithMany(y=>y.LikedByUsers).HasForeignKey(z=>z.LikedUserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUserRole>()
            .HasOne(ur=>ur.User)
            .WithMany(u=>u.UserRole)
            .HasForeignKey(fkey=>fkey.UserId)
            .IsRequired();

            builder.Entity<AppRole>()
            .HasMany(ur=>ur.UserRole)
            .WithOne(r=>r.Role)
            .HasForeignKey(fkey=>fkey.RoleId)
            .IsRequired();

            builder.Entity<Message>()
            .HasOne(u=>u.Sender)
            .WithMany(m=>m.MessagesSend)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(u=>u.Recipient)
            .WithMany(m=>m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}