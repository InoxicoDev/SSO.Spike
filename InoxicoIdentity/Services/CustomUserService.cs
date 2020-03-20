using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace InoxicoIdentity.Services
{
    public class CustomUserService : InMemoryUserService
    {
        private readonly List<InMemoryUser> _users;

        public CustomUserService(List<InMemoryUser> users) : base(users)
        {
            _users = users;
        }

        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            var query =
                from u in _users
                where
                    u.Provider == context.ExternalIdentity.Provider &&
                    u.ProviderId == context.ExternalIdentity.ProviderId
                select u;

            var user = query.SingleOrDefault();
            if (user == null)
            {
                context.AuthenticateResult = new AuthenticateResult("External user not found");
                return Task.FromResult(0);
            }

            context.AuthenticateResult = new AuthenticateResult(user.Subject, GetDisplayName(user), identityProvider: context.ExternalIdentity.Provider);

            return Task.FromResult(0);
        }
    }
}