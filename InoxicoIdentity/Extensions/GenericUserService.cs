using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace InoxicoIdentity.Extensions
{
    public class GenericUserService : InMemoryUserService
    {
        private readonly List<InMemoryUser> users;

        public GenericUserService(List<InMemoryUser> users) : base(users)
        {
            this.users = users;
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Caller == "refcode_grant")
            {
                var user = GetFromExternalUserId(context.Subject.Claims.First(p => p.Type == "ExternalUserId").Value);
                if (user == null)
                {
                    context.Subject = null;
                    context.IssuedClaims = null;
                    return Task.FromResult(0);
                }
                var adaptedClaims = user.Claims.Union(new[] { new Claim("sub", user.Subject) }).ToList();
                context.Subject = new ClaimsPrincipal(new ClaimsIdentity(adaptedClaims));
                context.IssuedClaims = adaptedClaims;
                return Task.FromResult(0);
            }
            return base.GetProfileDataAsync(context);
        }

        private InMemoryUser GetFromExternalUserId(string externalUserId)
        {
            return this.users.Find(p => p.Claims.Any(q => q.Type == "ExternalUserId" && q.Value == externalUserId));
        }
    }
}