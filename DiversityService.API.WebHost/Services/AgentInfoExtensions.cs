namespace DiversityService.API.Services
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Net.Http;
    using System.Web;
    using DiversityService.API.Model;

    public static class AgentInfoExtensions
    {
        public const string AgentInfoKey = "agent_info";

        public static AgentInfo GetAgentInfo(this HttpRequestMessage request)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var ctx = request.GetOwinContext();
            if (ctx != null)
            {
                return ctx.Get<AgentInfo>(AgentInfoKey);
            }

            return null;
        }

        public static void SetAgentInfo(this HttpRequestMessage request, AgentInfo value)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var ctx = request.GetOwinContext();
            if (ctx != null)
            {
                ctx.Set<AgentInfo>(AgentInfoKey, value);
            }
        }
    }
}