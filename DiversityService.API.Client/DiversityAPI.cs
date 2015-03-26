namespace DiversityService.API.Client
{
    using System;

    public class DiversityAPI
    {
        private string Token;

        public DiversityAPI(string access_token)
        {
            Token = access_token;
        }
    }
}