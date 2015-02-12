namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Moq;
    using Moq.Language.Flow;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;

    internal static class TestHelper
    {
        public static void ReturnsInOrder<T, TResult>(
            this ISetup<T, TResult> setup,
            params object[] results) where T : class
        {
            var queue = new Queue(results);
            setup.Returns(() =>
            {
                var result = queue.Dequeue();
                if (result is Exception)
                {
                    throw result as Exception;
                }
                return (TResult)result;
            });
        }

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

        internal static Mock<IStore<T, TKey>> SetupWithFakeData<T, TKey>(this Mock<IStore<T, TKey>> This, IQueryable<T> data, Func<T, object> keySelector = null)
        {
            return SetupWithFakeData<IStore<T, TKey>, T, TKey>(This, data, keySelector);
        }

        private static Type implementedInterfaceType(Type type, Type ifType)
        {
            return (from iface in type.GetInterfaces()
                    where iface.IsGenericType && iface.GetGenericTypeDefinition() == ifType
                    select iface).FirstOrDefault();
        }

        internal static Mock<TStore> SetupWithFakeData<TStore, T, TKey>(this Mock<TStore> This, IQueryable<T> data, Func<T, object> keySelector = null)

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

            if (keySelector == null)
            {
                var ifType = implementedInterfaceType(typeof(T), typeof(ICompositeIdentifiable<>));

                if (ifType != null)
                {
                    var keyGetter = ifType.GetMethod("CompositeKey");

                    keySelector = x => (keyGetter.Invoke(x, null));
                }
            }

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

            return This;
        }

        internal static void SetupInsert<T, TKey>(this Mock<IStore<T, TKey>> This, Action<T> keySetter)
        {
            This.Setup(x => x.InsertAsync(It.IsAny<T>()))
                .Callback(keySetter)
                .Returns(Task.FromResult<object>(null));
        }

        internal static T[] GenerateN<T>(Func<T> generator, int n)
        {
            var res = new T[n];

            for (int i = 0; i < n; i++)
            {
                res[i] = generator();
            }

            return res;
        }

        internal static T NestedResult<T>(IHttpActionResult result)
            where T : class
        {
            if (result is T)
            {
                return result as T;
            }

            var chained = result as IChainedResult;
            if (chained != null)
            {
                return NestedResult<T>(chained.InnerResult);
            }

            return default(T);
        }
    }
}