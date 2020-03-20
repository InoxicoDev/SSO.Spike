using System;

namespace Common
{
    public static class Addresses
    {
        public const string InoxicoSTSBase = "https://localhost:44301";
        public const string InoxicoTargetAppBase = "https://localhost:44302";
        public const string ThirdPartySTSBase = "https://localhost:44303";
        public const string ThirdPartyAppBase = "https://localhost:44304";

        public const string InoxicoTargetAppAuth = InoxicoTargetAppBase + "/ThirdPartyIntegration/AuthenticateExternalUser";

        public const string IntendedLocation = InoxicoTargetAppBase + "/IntendedLocation";

        public const string InoxicoSTSAuth = InoxicoSTSBase + "/connect/authorize";
    }
}
