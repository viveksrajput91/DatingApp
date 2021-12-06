using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using API.Error;
using System.Text.Json;

namespace API.MiddleWare
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleWare(RequestDelegate next,ILogger<ExceptionMiddleWare> logger,IHostEnvironment env)
        {
            this._next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                logger.LogError(ex,ex.Message);
                context.Response.ContentType="application/json";
                context.Response.StatusCode=(int) HttpStatusCode.InternalServerError;

                var response= env.IsDevelopment()?
                                new ApiException(context.Response.StatusCode,ex.Message,ex.StackTrace?.ToString())
                                :new ApiException(context.Response.StatusCode,"Internal Server Error");

                var jsonOption=new JsonSerializerOptions{PropertyNamingPolicy=JsonNamingPolicy.CamelCase};

                var jsonResponse= JsonSerializer.Serialize(response,jsonOption);

                await context.Response.WriteAsync(jsonResponse);

            }
        }
    }
}