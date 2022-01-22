using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response,int pageNumber,int itemsPerPage,int totalItems,int totalPages)
        {
         var paginationHeader=new PaginationHeader(pageNumber,itemsPerPage,totalItems,totalPages);

         var jsonOption=new JsonSerializerOptions{
             PropertyNamingPolicy=JsonNamingPolicy.CamelCase
         };
         var jsonResult=JsonSerializer.Serialize(paginationHeader,jsonOption);
         response.Headers.Add("Pagination",jsonResult);
         response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
        
    }
}