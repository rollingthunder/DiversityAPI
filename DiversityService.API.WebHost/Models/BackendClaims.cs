namespace DiversityService.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;

    public class BackendCredentialsClaim : Claim
    {
        public const string TYPE = "backend_credentials";
        private const char VALUE_SPLIT = ':';

        public readonly string User, Password;

        public BackendCredentialsClaim(string user, string password)
            : base(TYPE, FormatValue(user, password))
        {
        }

        public BackendCredentialsClaim(Claim claim)
            : base(claim.Type, claim.Value)
        {
            Contract.Requires<ArgumentException>(claim.Type == TYPE, "Invalid Claim Type");

            SplitValue(claim.Value, out User, out Password);
        }

        private static string FormatValue(string user, string password)
        {
            Contract.Requires<ArgumentNullException>(user != null, "user");
            Contract.Requires<ArgumentNullException>(password != null, "password");
            Contract.Requires<ArgumentException>(!user.Contains(VALUE_SPLIT), "user may not contain the split character");

            return string.Format("{0}:{1}", user, password);
        }

        private static void SplitValue(string value, out string user, out string password)
        {
            Contract.Requires<ArgumentNullException>(value != null, "value");

            var splitidx = value.IndexOf(VALUE_SPLIT);

            if (splitidx < 0)
            {
                throw new ArgumentException("value of unexpected format");
            }

            user = value.Substring(0, splitidx);
            password = value.Substring(splitidx + 1);
        }
    }

    public static class BackendCredentialsClaimExtensions
    {
        public static BackendCredentialsClaim GetBackendCredentialsClaim(this ClaimsIdentity This)
        {
            Contract.Requires<ArgumentNullException>(This != null, "This");

            var claim = This.FindFirst(BackendCredentialsClaim.TYPE);

            if (claim != null)
            {
                return new BackendCredentialsClaim(claim);
            }
            else
            {
                return null;
            }
        }
    }
}