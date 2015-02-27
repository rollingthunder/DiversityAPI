namespace DiversityService.API.Services
{
    using DiversityService.API.Model;
    using Microsoft.Owin;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net.Http;
    using System.Web;

    public static class AgentInfoExtensions
    {
        public const string AGENT_INFO_KEY = "agent_info";

        public static AgentInfo GetAgentInfo(this HttpRequestMessage request)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var ctx = request.GetOwinContext();
            if (ctx != null)
            {
                return ctx.Get<AgentInfo>(AGENT_INFO_KEY);
            }

            return null;
        }

        public static void SetAgentInfo(this HttpRequestMessage request, AgentInfo value)
        {
            Contract.Requires<ArgumentNullException>(request != null);

            var ctx = request.GetOwinContext();
            if (ctx != null)
            {
                ctx.Set<AgentInfo>(AGENT_INFO_KEY, value);
            }
        }
    }
}