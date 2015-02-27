namespace DiversityService.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Web;

    public class AgentNameClaim : Claim
    {
        public const string TYPE = "agent_name";

        public AgentNameClaim(string name)
            : base(TYPE, name)
        {
        }
    }

    public class AgentUriClaim : Claim
    {
        public const string TYPE = "agent_uri";

        public AgentUriClaim(string uri)
            : base(TYPE, uri)
        {
        }
    }
}