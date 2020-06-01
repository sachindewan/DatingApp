using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            await repo.SaveAll();
        }
    }
    public class AutherizeCurrentLoggedInUser : IAsyncActionFilter {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.RouteValues.ContainsKey("userId"))
            {
                var currentUserIdPassedIn = int.Parse(context.HttpContext.Request.RouteValues["userId"].ToString());
                var userId = int.Parse(context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (userId != currentUserIdPassedIn)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.HttpContext.Response.WriteAsync("Invalid user... Autherization failed");
                }
                else
                {
                    await next();
                }
            }

            else
            {
                await next();
            }
          
        }
    }

}
