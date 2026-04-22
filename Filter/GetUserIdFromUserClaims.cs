using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class GetUserIdFromUserClaims() :Attribute,IAsyncResourceFilter
{
   

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        Claim? adminIdClaim = context.HttpContext.User.Claims.FirstOrDefault(value=>value.Type==JwtRegisteredClaimNames.NameId);

        if (!Guid.TryParse(adminIdClaim?.Value, out Guid id))
        {
            context.Result = new  UnauthorizedObjectResult("Invalid Admin Id");
            return;
        }
        context.HttpContext.Items["id"]= id;
        
        await next();
        throw new NotImplementedException();
    }
}