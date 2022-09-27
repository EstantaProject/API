using API.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Paseto;
using Paseto.Builder;
using Paseto.Cryptography.Key;
using Paseto.Protocol;

namespace API.Attributes;

public class AuthorizeAttribute : TypeFilterAttribute
{
    public AuthorizeAttribute() : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[] {};
    }
}

public class ClaimRequirementFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        using (var db = new Context())
        {
            var token = context.HttpContext.Request.Headers["authEstantaToken"].ToString();

            Session session = db.Sessions.FirstOrDefault(w => w.Token == token);

            var key = session?.Key;
            var pasetoSymmetricKey = new PasetoSymmetricKey(key, new Version4());

            var result = new PasetoBuilder().Use(ProtocolVersion.V4, Purpose.Local)
                .WithKey(pasetoSymmetricKey)
                .Decode(token);

            if (!result.IsValid)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}