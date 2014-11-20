namespace DiversityService.API.Test
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    internal static class TestHelper
    {
        private static Random rnd = new Random();

        public static int RandomInt()
        {
            return rnd.Next();
        }

        public static async Task<T> GetContentAsync<T>(this Task<IHttpActionResult> This)
        {
            var action = await This;
            var result = await action.ExecuteAsync(CancellationToken.None);
            result.EnsureSuccessStatusCode();
            if (result.Content is ObjectContent<T>)
            {
                var objContent = result.Content as ObjectContent<T>;

                return (T)objContent.Value;
            }
            else
            {
                return await result.Content.ReadAsAsync<T>();
            }
        }

        public static async Task<HttpResponseMessage> GetResponseAsync(this Task<IHttpActionResult> This)
        {
            var result = await This;
            var response = await result.ExecuteAsync(CancellationToken.None);
            return response;
        }

        internal static string RandomRoute()
        {
            return string.Format("https://{0}", Guid.NewGuid());
        }
    }
}