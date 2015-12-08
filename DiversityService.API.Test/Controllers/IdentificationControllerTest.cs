namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model;
    using DiversityService.API.Results;
    using DiversityService.API.Services;
    using Moq;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;
    using Xunit;
    using Collection = DiversityService.DB.Collection;

    internal static class IdentificationTestHelper
    {
        public static Collection.IdentificationUnit FakeUnit(int? sid)
        {
            return new Collection.IdentificationUnit()
            {
                Id = TestHelper.RandomInt(),
                SpecimenId = sid ?? TestHelper.RandomInt(),
                TransactionGuid = Guid.NewGuid()
            };
        }
    }

    internal class CRUDValidator
    {
        public static void ValidateGetAll()
        { }

        public static void ValidateNotFound(IHttpActionResult result)
        {
            var notfound = result as NotFoundResult;

            Assert.NotNull(result);
        }

        internal static void ValidateRedirect(IHttpActionResult result)
        {
            var redirect = result as RedirectResult;

            Assert.NotNull(result);

            // TODO More thorough validation
        }

        internal static void ValidateCreated<T>(IHttpActionResult result)
        {
            var created = result as CreatedAtRouteNegotiatedContentResult<T>;

            Assert.NotNull(created);
            Assert.NotNull(created.Content);
        }
    }

    public class IdentificationControllerTest : ControllerTestBase<IdentificationController>
    {
        protected readonly Mock<IFieldDataContext> MockContext;
        protected readonly Mock<ITransaction> MockTransaction;
        protected readonly Mock<IStore<Collection.IdentificationUnit, Collection.IdentificationUnitKey>> MockIUStore;
        protected readonly Mock<IStore<Collection.Identification, Collection.IdentificationKey>> MockIDStore;
        protected readonly Mock<IStore<Collection.IdentificationUnitGeoAnalysis, Collection.IdentificationGeoKey>> MockIUGANStore;

        public IdentificationControllerTest()
        {
            MockIUStore = Kernel.GetMock<IStore<Collection.IdentificationUnit, Collection.IdentificationUnitKey>>();
            MockIDStore = Kernel.GetMock<IStore<Collection.Identification, Collection.IdentificationKey>>();
            MockIUGANStore = Kernel.GetMock<IStore<Collection.IdentificationUnitGeoAnalysis, Collection.IdentificationGeoKey>>();
            InitController();
            MockContext = Kernel.GetMock<IFieldDataContext>();
            MockTransaction = Kernel.GetMock<ITransaction>();
        }

        public class TheUnqualifiedGet : IdentificationControllerTest
        {
            [Fact]
            public async Task ReturnsAPagedResult()
            {
                // Arrange
                var sid = 1;
                var fakeEntities = TestHelper.GenerateN(() => IdentificationTestHelper.FakeUnit(sid), 5);
                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid);

                // Assert
                var paged = TestHelper.NestedResult<PagingResult<Collection.IdentificationUnit>>(result);
                Assert.NotNull(paged);
            }

            [Fact]
            public async Task ReturnsAllForParent()
            {
                // Arrange
                var sid = 1;
                var expectedcount = 5;
                var fakeEntities =
                    Enumerable.Concat(
                        Enumerable.Range(0, expectedcount).Select(_ => sid as int?),
                        Enumerable.Range(0, 5).Select(_ => sid + TestHelper.RandomInt() as int?)
                    )
                    .Select(x => IdentificationTestHelper.FakeUnit(x))
                    .ToArray();

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid) as IQueryResult<Identification>;
                var content = result.Query.ToList();

                // Assert
                Assert.Equal(expectedcount, result.Query.Count());
                Assert.False(content.Any(x => x.SpecimenId != sid));
            }

            [Fact]
            public async Task OrdersById()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Range(0, 6)
                    .Select(_ => IdentificationTestHelper.FakeUnit(sid))
                    .ToArray();

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid) as IQueryResult<Identification>;
                var content = result.Query.ToList();

                // Assert
                Assert.Equal(content.OrderBy(x => x.Id), content);
            }

            [Fact]
            public async Task RestoresTheLocalization()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Range(0, 6)
                    .Select(_ => IdentificationTestHelper.FakeUnit(sid))
                    .ToArray();
                var expected = fakeEntities.First();

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid, expected.Id) as OkNegotiatedContentResult<Identification>;

                // Assert
                throw new NotImplementedException();
                Assert.Equal(expected.Id, result.Content.Id);
            }
        }

        public class TheGetById : IdentificationControllerTest
        {
            [Fact]
            public async Task ReturnsTheMatchingResult()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Range(0, 6)
                    .Select(_ => IdentificationTestHelper.FakeUnit(sid))
                    .ToArray();
                var expected = fakeEntities.First();

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid, expected.Id) as OkNegotiatedContentResult<Identification>;

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expected.Id, result.Content.Id);
            }

            [Fact]
            public async Task ReturnsNotFoundIfNecessary()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Range(0, 6)
                    .Select(_ => IdentificationTestHelper.FakeUnit(sid))
                    .ToArray();
                var expected = fakeEntities.First();

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid + 1, expected.Id);

                // Assert
                CRUDValidator.ValidateNotFound(result);
            }

            [Fact]
            public async Task RestoresTheLocalization()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Range(0, 6)
                    .Select(_ => IdentificationTestHelper.FakeUnit(sid))
                    .ToArray();
                var expected = fakeEntities.First();

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Get(sid, expected.Id) as OkNegotiatedContentResult<Identification>;

                // Assert
                throw new NotImplementedException();
                Assert.Equal(expected.Id, result.Content.Id);
            }
        }

        public class ThePost : IdentificationControllerTest
        {
            [Fact]
            public async Task RedirectsIfNecessary()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Range(0, 6)
                    .Select(_ => IdentificationTestHelper.FakeUnit(sid))
                    .ToArray();
                var expected = fakeEntities.First();

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid
                };

                this.MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                CRUDValidator.ValidateRedirect(result);
            }

            [Fact]
            public async Task PersistsTheIdentification()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = null
                };

                int callId = 0, beginId = 0, insertIUId = 0, insertIDId = 0, insertIUGANId = 0, commitId = int.MaxValue, disposeId = 0;

                MockTransaction
                    .Setup(x => x.Commit())
                    .Callback(() => { commitId = callId++; });

                MockTransaction
                    .Setup(x => x.Dispose())
                    .Callback(() => { disposeId = callId++; });

                MockContext
                    .Setup(x => x.BeginTransaction())
                    .Callback(() => { beginId = callId++; })
                    .Returns(MockTransaction.Object);

                MockIUStore
                    .SetupWithFakeData(fakeEntities.AsQueryable());

                MockIUStore
                    .Setup(x => x.InsertAsync(It.Is<Collection.IdentificationUnit>(y => y.SpecimenId == expected.SpecimenId)))
                    .Callback((Collection.IdentificationUnit iu) => { iu.Id = expected.Id; insertIUId = callId++; })
                    .Returns(Task.FromResult(0));

                MockIDStore
                    .Setup(x => x.InsertAsync(It.Is<Collection.Identification>(y => y.IdentificationUnitID == expected.Id)))
                    .Callback(() => { insertIDId = callId++; })
                    .Returns(Task.FromResult(0));

                MockIUGANStore
                    .Setup(x => x.InsertAsync(It.Is<Collection.IdentificationUnitGeoAnalysis>(y => y.IdentificationUnitId == expected.Id)))
                    .Callback(() => { insertIUGANId = callId++; })
                    .Returns(Task.FromResult(0));

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIUStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnit>(y => y.SpecimenId == expected.SpecimenId)), Times.Once(), "IU not inserted");
                MockIDStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.Identification>(y => y.SpecimenID == expected.SpecimenId && y.IdentificationUnitID == expected.Id)), Times.Once(), "ID not inserted");
                MockIUGANStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnitGeoAnalysis>(y => y.SpecimenId == expected.SpecimenId && y.IdentificationUnitId == expected.Id)), Times.Once(), "IUGAN not inserted");

                Assert.True(commitId < disposeId, "transaction not committed");

                Assert.True(
                    new[] { insertIUId, insertIDId, insertIUGANId }
                    .All(x => x < commitId && x > beginId)
                    , "insert not covered by transaction");

                CRUDValidator.ValidateCreated<Identification>(result);
            }

            [Fact]
            public async Task SetsIdentCacheOnTheIU()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = null
                };

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIUStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnit>(y => !string.IsNullOrWhiteSpace(y.LastIdentificationCache))));
            }

            [Fact]
            public async Task SetsCustomFieldsOnTheID()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = null
                };

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIDStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.Identification>(y => !string.IsNullOrWhiteSpace(y.IdentificationCategory))));
                MockIDStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.Identification>(y => !string.IsNullOrWhiteSpace(y.Notes))));
            }

            [Fact]
            public async Task SetsResponsableInfoOnTheID()
            {
                // Arrange

                // AgentInfo is set in Test Base Constructor
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = null
                };

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIDStore
                   .Verify(x => x.InsertAsync(It.Is<Collection.Identification>(y => !string.IsNullOrWhiteSpace(y.ResponsibleName))));
                MockIDStore
                   .Verify(x => x.InsertAsync(It.Is<Collection.Identification>(y => !string.IsNullOrWhiteSpace(y.ResponsibleAgentURI))));
            }

            [Fact]
            public async Task SetsANoteOnTheIUGAN()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = null
                };

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIUGANStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnitGeoAnalysis>(y => !string.IsNullOrWhiteSpace(y.Notes))));
            }

            [Fact]
            public async Task SetsResponsableInfoOnTheIUGAN()
            {
                // Arrange

                // AgentInfo is set in Test Base Constructor
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = null
                };

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIUGANStore
                   .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnitGeoAnalysis>(y => !string.IsNullOrWhiteSpace(y.ResponsibleName))));
                MockIUGANStore
                   .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnitGeoAnalysis>(y => !string.IsNullOrWhiteSpace(y.ResponsibleAgentURI))));
            }

            [Fact]
            public async Task SetsTheGeographyOnTheIUGAN()
            {
                // Arrange
                var sid = 1;
                var fakeEntities =
                    Enumerable.Empty<Collection.IdentificationUnit>()
                    .ToArray();

                var expected = IdentificationTestHelper.FakeUnit(sid);

                var upload = new IdentificationBindingModel()
                {
                    TransactionGuid = expected.TransactionGuid,
                    Uri = "testuri",
                    SpecimenId = sid,
                    TaxonomicGroup = "plant",
                    Name = "testident",
                    Localization = new Localization()
                    {
                        Altitude = 120.0,
                        Latitude = 74.1,
                        Longitude = 34.1
                    }
                };

                // Act
                var result = await Controller.Post(sid, upload);

                // Assert
                MockIUGANStore
                    .Verify(x => x.InsertAsync(It.Is<Collection.IdentificationUnitGeoAnalysis>(y => upload.Localization.Equals(y.Geography.ToLocalization()))));
            }
        }
    }
}