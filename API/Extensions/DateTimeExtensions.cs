using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateTime DateOfBirth)
        {
            var Today=DateTime.Today;//29-11-202
            var age=Today.Year-DateOfBirth.Year;//30
            if(DateOfBirth.Date > Today.AddYears(-age)) age--;
            return age;
        }
        
    }
}