using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiversityService.API.Controllers
{
    public static class Parameters
    {
        private const string BASE_URL = Route.DOCUMENTATION_BASE + "params/";

        public static readonly Uri SERIES_ID = new Uri(BASE_URL + "trip_id", UriKind.Absolute);
    }
}