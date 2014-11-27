namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
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

        public static bool AreSetEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            var set = new HashSet<T>(a);

            return b.All(x => set.Contains(x));
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

        internal static void SetupWithFakeData<TStore, T, TKey>(this Mock<TStore> This, IQueryable<T> data, Func<T, object> keySelector = null)
            where TStore : class, IReadOnlyStore<T, TKey>
        {
            This
                .Setup(x => x.GetAsync(It.IsAny<Expression<Func<T, bool>>>(), null, null, ""))
                .Returns((
                        Expression<Func<T, bool>> filter,
                        Func<IQueryable<T>, IQueryable<T>> restrict,
                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                        string includeProperties
                    ) =>
                    {
                        filter = filter ?? (x => true);
                        return Task.FromResult(data.Where(filter).ToList().AsEnumerable());
                    });

            if (keySelector == null && typeof(IIdentifiable).IsAssignableFrom(typeof(T)))
            {
                keySelector = x => ((x as IIdentifiable).Id);
            }

            if (keySelector != null)
            {
                This
                .Setup(x => x.GetByIDAsync(It.IsAny<TKey>()))
                .Returns((TKey key) =>
                {
                    return Task.FromResult(data.Where(x => keySelector(x).Equals(key)).SingleOrDefault());
                });
            }
            else
            {
                This
                .Setup(x => x.GetByIDAsync(It.IsAny<TKey>()))
                .Throws(new NotImplementedException());
            }

            This
                .Setup(x => x.GetQueryableAsync())
                .ReturnsAsync(data);
        }
    }
}