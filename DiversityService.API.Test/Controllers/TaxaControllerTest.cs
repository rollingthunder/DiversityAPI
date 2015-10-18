namespace DiversityService.API.Test
{
    using DiversityService.API.Controllers;
    using DiversityService.API.Model.Internal;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;
    using TaxonNames = DiversityService.DB.TaxonNames;

    public class TaxaControllerTest : ControllerTestBase<TaxaController>
    {
        public CollectionServerLogin PublicLogin { get; set; }

        private readonly FakeTaxonServer Local;
        private readonly FakeTaxonServer Public;

        class FakeTaxonModule
        {
            public TaxonNames.TaxonList[] Lists { get; set; }
            public Mock<ITaxa> Mock { get; set; }
        }

        class FakeTaxonServer
        {
            public Mock<IDiscoverDBModules> Discovery { get; set; }
            public IEnumerable<DBModule> Modules { get; set; }
            public IDictionary<string, FakeTaxonModule> ListsForModule { get; set; }
        }

        public TaxaControllerTest()
        {
            PublicLogin = new CollectionServerLogin() { Kind = TaxaController.TAXON_LOGIN_KIND,  Name = "public", Password = "1234", User = "public" };
            SetupMocks.Configuration(Kernel, new[] { PublicLogin });

            Local = TestTaxaFor("", Login);
            Public = TestTaxaFor("public", PublicLogin);

            InitController();
        }

        private FakeTaxonServer TestTaxaFor(string prefix, CollectionServerLogin login )
        {
            var modules = new[] {
                new DBModule(DBModuleType.Collection, "ACollection"),
                new DBModule(DBModuleType.TaxonNames, "BTaxa"),
                new DBModule(DBModuleType.TaxonNames, "CTaxa"),
                new DBModule(DBModuleType.Unknown, "Unknown")
            };

            var listsForModule = new Dictionary<string, TaxonNames.TaxonList[]>()
            {
                {"BTaxa", new[] {
                    new TaxonNames.TaxonList() { ListID = 1, DataSource = prefix + "B1", DisplayText = prefix + "ListB1", TaxonomicGroup = "plants" },
                    new TaxonNames.TaxonList() { ListID = 2, DataSource = prefix + "B2", DisplayText = prefix + "ListB2", TaxonomicGroup = "lichen" },
                }},
                {"CTaxa", new[] {
                    new TaxonNames.TaxonList() { ListID = 3, DataSource = prefix + "C1", DisplayText = prefix + "ListC1", TaxonomicGroup = "plants" },
                    new TaxonNames.TaxonList() { ListID = 4, DataSource = prefix + "C2", DisplayText = prefix + "ListC2", TaxonomicGroup = "plants" },
                }}
            };

            var discovery = SetupMocks.ModuleDiscovery(Kernel, login, modules);

            var fakesForModule = new Dictionary<string, FakeTaxonModule>();
            foreach (var kv in listsForModule)
            {
                var taxa = SetupMocks.Taxa(Kernel, login, kv.Key, kv.Value);

                fakesForModule[kv.Key] = new FakeTaxonModule() { Lists = kv.Value, Mock = taxa };
            }

            return new FakeTaxonServer()
            {
                Discovery = discovery,
                ListsForModule = fakesForModule,
                Modules = modules
            };
        }

        private IEnumerable<TaxonNames.TaxonName> TestNamesForList(Mock<ITaxa> taxa, TaxonNames.TaxonList list)
        {
            var names = new List<TaxonNames.TaxonName>();
            for (int i = 0; i < 10; i++)
            {
                names.Add(
                    new DB.TaxonNames.TaxonName()
                    {
                        AcceptedNameCache = "A",
                        AcceptedNameURI = "Uri",
                        Family = "Fam",
                        GenusOrSupragenericName = "Gen",
                        InfraspecificEpithet = "Infra",
                        NameURI = list.ListID.ToString() + "nameUri" + i.ToString(),
                        Order = "Order",
                        SpeciesEpithet = "Species",
                        Synonymy = "accepted",
                        TaxonNameCache = "tn" + i.ToString(),
                        TaxonNameSinAuthors = "tn"
                    });
            }

            SetupMocks.TaxonNames(taxa, list.ListID, names);

            return names;
        }

        [Fact]
        public async Task CanDiscoverListsOnCurrentServer()
        {
            // Arrange
            var expected = new List<TaxonNames.TaxonList>(
                Local.ListsForModule["BTaxa"].Lists.Concat(Local.ListsForModule["CTaxa"].Lists)
                );

            // Act
            var lists = await Controller.Get();

            // Assert
            Assert.NotEmpty(lists);
            Assert.True(lists.All(l => expected.Any(x => x.DisplayText == l.Name && x.TaxonomicGroup == l.TaxonGroup)));
        }

        [Fact]
        public async Task CanDiscoverListsOnPublicServer()
        {
            // Arrange
            var expected = new List<TaxonNames.TaxonList>(
                Public.ListsForModule["BTaxa"].Lists.Concat(Public.ListsForModule["CTaxa"].Lists)
                );
            // Act
            var lists = await Controller.GetPublic();

            // Assert
            Assert.NotEmpty(lists);
            Assert.True(lists.All(l => expected.Any(x => x.DisplayText == l.Name && x.TaxonomicGroup == l.TaxonGroup)));
        }

        [Fact]
        public async Task CanRetrieveNamesOnCurrentServer()
        {
            // Arrange
            var fakeModule = Local.ListsForModule["BTaxa"];
            var list = fakeModule.Lists[0];
            var mock = fakeModule.Mock;

            var expected = new HashSet<string>(
                    from name in TestNamesForList(mock, list)
                    select name.NameURI
                    );

            var lists = await Controller.Get();
            var id = (from l in lists
                     where l.Name == list.DisplayText
                     select l.Id).First();
        
            // Act
            var names = await Controller.GetList(id, 10, 10);

            // Assert
            Assert.Equal(expected.Count, names.Count());
            Assert.True(names.All(x => expected.Contains(x.URI)));
        }

        [Fact]
        public async Task CanRetrieveNamesOnPublicServer()
        {
            // Arrange

            // Act

            // Assert
            Assert.False(true);
        }
    }
}