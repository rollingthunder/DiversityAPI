namespace DiversityService.API.Model
{
    using System.Security.Claims;

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