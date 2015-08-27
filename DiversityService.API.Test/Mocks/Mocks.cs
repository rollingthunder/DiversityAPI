namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Model.Internal;
    using DiversityService.API.Services;
    using DiversityService.API.WebHost;
    using DiversityService.API.WebHost.Models;
    using Microsoft.AspNet.Identity;
    using Moq;
    using Ninject;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Collection = DiversityService.DB.Collection;

    public static class SetupMocks
    {
        public static Mock<T> GetMock<T>(this IKernel Kernel) where T : class
        {
            var obj = Kernel.TryGet<T>();

            if (obj != default(T))
            {
                try
                {
                    return Mock.Get(obj);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException("Service Instance is not mocked.", ex);
                }
            }
            else
            {
                var mock = new Mock<T>();

                mock.DefaultValue = DefaultValue.Empty;

                Kernel
                    .Bind<T>()
                    .ToConstant(mock.Object);

                return mock;
            }
        }

        public static void UserStore(IKernel Kernel)
        {
            var store = Kernel
                .GetMock<IUserStore<ApplicationUser>>();

            Kernel.Bind<AccountController>()
                .ToSelf()
                .WithPropertyValue("UserManager", (ctx) => ctx.Kernel.Get<ApplicationUserManager>());
        }

        public static void FieldDataStores(IKernel Kernel, TestData data)
        {
            if (data.Series != null)
            {
                Store<IStore<Collection.EventSeries, int>, Collection.EventSeries, int>(Kernel, data.Series.AsQueryable());
            }

            if (data.Events != null)
            {
                Store<IStore<Collection.Event, int>, Collection.Event, int>(Kernel, data.Events.AsQueryable());
            }

            if (data.Specimen != null)
            {
                Store<IStore<Collection.Specimen, int>, Collection.Specimen, int>(Kernel, data.Specimen.AsQueryable());
            }

            if (data.Identification != null)
            {
                Store<IStore<Collection.Identification, int>, Collection.Identification, int>(Kernel, data.Identification.AsQueryable());
            }

            if (data.IdentificationUnit != null)
            {
                Store<IStore<Collection.IdentificationUnit, int>, Collection.IdentificationUnit, int>(Kernel, data.IdentificationUnit.AsQueryable());
            }

            if (data.IdentificationGeoAnalysis != null)
            {
                Store<IStore<Collection.IdentificationUnitGeoAnalysis, int>, Collection.IdentificationUnitGeoAnalysis, int>(Kernel, data.IdentificationGeoAnalysis.AsQueryable());
            }
        }

        public static Mock<TStore> Store<TStore, TEntity, TKey>(IKernel Kernel, IQueryable<TEntity> Data) where TStore : class, IStore<TEntity, TKey>
        {
            var storeMock = Kernel
                .GetMock<TStore>();
            TestHelper.SetupWithFakeData<TStore, TEntity, TKey>(storeMock, Data);

            return storeMock;
        }

        public static Mock<IContextFactory> ContextFactory(IKernel Kernel, TestData Data)
        {
            var factoryMock = Kernel
                .GetMock<IContextFactory>();
            var ctxMock = Kernel
                .GetMock<IContext>();
            var servers = Data.Servers ?? Enumerable.Empty<InternalCollectionServer>();
            factoryMock.Setup(f => f.CreateContextAsync(It.Is<CollectionServerLogin>(l => servers.Any(s => s.Equals(l)))))
                .Returns(Task.FromResult(ctxMock.Object));

            return factoryMock;
        }

        public static Mock<IConfigurationService> Servers(IKernel Kernel, InternalCollectionServer[] Servers)
        {
            Kernel.Unbind<IConfigurationService>();

            var cfgMock = Kernel
                .GetMock<IConfigurationService>();
            cfgMock
                .Setup(x => x.GetCollectionServers())
                .Returns(Servers);

            Kernel
                .Rebind<IConfigurationService>()
                .ToConstant(cfgMock.Object);

            return cfgMock;
        }

        public static Mock<IContext> Context(IKernel Kernel)
        {
            var mock = Kernel.GetMock<IContext>();

            // Transaction

            var transaction = Kernel.GetMock<ITransaction>();

            mock.Setup(x => x.BeginTransaction())
                .Returns(transaction.Object);

            // FieldDataStores

            mock.SetupGet(x => x.Projects)
                .Returns(Kernel.GetMock<IProjectStore>().Object);

            mock.SetupGet(x => x.Series)
               .Returns(Kernel.GetMock<IStore<Collection.EventSeries, int>>().Object);

            mock.SetupGet(x => x.Events)
               .Returns(Kernel.GetMock<IStore<Collection.Event, int>>().Object);

            mock.SetupGet(x => x.Specimen)
               .Returns(Kernel.GetMock<IStore<Collection.Specimen, int>>().Object);

            mock.SetupGet(x => x.IdentificationUnits)
               .Returns(Kernel.GetMock<IStore<Collection.IdentificationUnit, Collection.IdentificationUnitKey>>().Object);

            mock.SetupGet(x => x.Identifications)
               .Returns(Kernel.GetMock<IStore<Collection.Identification, Collection.IdentificationKey>>().Object);

            mock.SetupGet(x => x.IdentificationGeoAnalyses)
               .Returns(Kernel.GetMock<IStore<Collection.IdentificationUnitGeoAnalysis, Collection.IdentificationGeoKey>>().Object);

            return mock;
        }

        public static Mock<IDiscoverDBModules> ModuleDiscovery(IKernel Kernel, CollectionServerLogin login,
            IEnumerable<DBModule> Modules)
        {
            var Discovery = Kernel.GetMock<IDiscoverDBModules>();

            Discovery.SetReturnsDefault<IEnumerable<DBModule>>(null);

            Discovery.Setup(x => x.DiscoverModules(It.Is<CollectionServerLogin>(l => login.Equals(l))))
                .ReturnsAsync(Modules);

            return Discovery;
        }

        public static Mock<ITaxa> Taxa(IKernel Kernel, CollectionServerLogin Login,
            string Catalog,
            IEnumerable<TaxonList> Lists)
        {
            var Factory = Kernel.GetMock<ITaxaFactory>();
            var Taxa = new Mock<ITaxa>();
            var loginWithCatalog = Login.Clone();
            loginWithCatalog.Catalog = Catalog;

            Factory
                .Setup(x => x.GetTaxa(It.Is<CollectionServerLogin>(l => loginWithCatalog.Equals(l))))
                .Returns(Taxa.Object);

            Taxa
                .Setup(x => x.GetListsForUserAsync())
                .Returns(() => Task.FromResult(Lists.AsEnumerable()));

            return Taxa;
        }
    }
}