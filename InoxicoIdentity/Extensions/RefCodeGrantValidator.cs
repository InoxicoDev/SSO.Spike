using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace InoxicoIdentity.Extensions
{
    public class RefCodeGrantValidator : ICustomGrantValidator
    {
        private readonly IUserService _users;
        private readonly RefCodeRegistry _refCodeRegistry;

        public RefCodeGrantValidator(IUserService users, RefCodeRegistry refCodeRegistry)
        {
            _users = users;
            _refCodeRegistry = refCodeRegistry;
        }

        public string GrantType => "refcode_grant";

        public async Task<CustomGrantValidationResult> ValidateAsync(ValidatedTokenRequest request)
        {
            var refCode = request.Raw.Get("refCode");
            if (string.IsNullOrEmpty(refCode))
            {
                return new CustomGrantValidationResult("refCode not specified");
            }

            var externalUserID = _refCodeRegistry.GetExternalUserId(refCode);
            if (externalUserID == null)
            {
                throw new Exception("Invalid RefCode");
            }

            var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("ExternalUserId", externalUserID) })), request.Client, GrantType);
            await _users.GetProfileDataAsync(context);

            if (context.IssuedClaims == null)
            {
                return new CustomGrantValidationResult("Invalid user");
            }

            return new CustomGrantValidationResult(context.Subject.Claims.First(p => p.Type == "sub").Value, "external", context.IssuedClaims);
        }
    }
}