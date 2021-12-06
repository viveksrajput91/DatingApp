using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.Json;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUserData(DataContext dataContext)
        {
            if(await dataContext.Users.AnyAsync()) return;

            var userData=  await File.ReadAllTextAsync("./Data/UserSeedData.json");

            var users= JsonSerializer.Deserialize<List<AppUser>>(userData);

            foreach(var user in users)
            {
                user.UserName=user.UserName.ToLower();
                using var hmac=new HMACSHA512();
                user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes("P@$$w0rd"));
                user.PasswordSalt=hmac.Key;

                dataContext.Users.Add(user);
            }

           await dataContext.SaveChangesAsync();
        }
    }
}