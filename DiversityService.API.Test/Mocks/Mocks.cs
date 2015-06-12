namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Moq;
    using Ninject.MockingKernel;
    using Ninject.MockingKernel.Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Mocks
    {
        public static void SetupMocks(this MoqMockingKernel Kernel, TestData data)
        {
            if (data.Servers != null)
            {
                SetupServers(Kernel, data.Servers);
            }

            SetupContext(Kernel);
            SetupContextFactory(Kernel);

            SetupStores(Kernel, data);
        }

        private static void SetupStores(MoqMockingKernel Kernel, TestData data)
        {
            if (data.Series != null)
            {
                Kernel.SetupStore<IStore<Collection.EventSeries, int>, Collection.EventSeries, int>(data.Series.AsQueryable());
            }

            if (data.Events != null)
            {
                Kernel.SetupStore<IStore<Collection.Event, int>, Collection.Event, int>(data.Events.AsQueryable());
            }

            if (data.Specimen != null)
            {
                Kernel.SetupStore<IStore<Collection.Specimen, int>, Collection.Specimen, int>(data.Specimen.AsQueryable());
            }

            if (data.Identification != null)
            {
                Kernel.SetupStore<IStore<Collection.Identification, int>, Collection.Identification, int>(data.Identification.AsQueryable());
            }

            if (data.IdentificationUnit != null)
            {
                Kernel.SetupStore<IStore<Collection.IdentificationUnit, int>, Collection.IdentificationUnit, int>(data.IdentificationUnit.AsQueryable());
            }

            if (data.IdentificationGeoAnalysis != null)
            {
                Kernel.SetupStore<IStore<Collection.IdentificationUnitGeoAnalysis, int>, Collection.IdentificationUnitGeoAnalysis, int>(data.IdentificationGeoAnalysis.AsQueryable());
            }
        }

        public static Mock<TStore> SetupStore<TStore, TEntity, TKey>(this MoqMockingKernel Kernel, IQueryable<TEntity> Data) where TStore : class, IStore<TEntity, TKey>
        {
            var storeMock = Kernel
                .GetMock<TStore>();
            TestHelper.SetupWithFakeData<TStore, TEntity, TKey>(storeMock, Data);

            return storeMock;
        }

        private static Mock<IContextFactory> SetupContextFactory(MoqMockingKernel Kernel)
        {
            var factoryMock = Kernel
                .GetMock<IContextFactory>();
            var ctxMock = Kernel
                .GetMock<IContext>();

            factoryMock.Setup(f => f.CreateContextAsync(It.IsAny<InternalCollectionServer>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(ctxMock.Object));

            return factoryMock;
        }

        public static Mock<IConfigurationService> SetupServers(MoqMockingKernel Kernel, InternalCollectionServer[] Servers)
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

        public static Mock<IContext> SetupContext(MoqMockingKernel Kernel)
        {
            var mock = Kernel.GetMock<IContext>();

            // Transaction

            var transaction = Kernel.GetMock<ITransaction>();

            mock.Setup(x => x.BeginTransaction())
                .Returns(transaction.Object);

            // Stores

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
    }
}