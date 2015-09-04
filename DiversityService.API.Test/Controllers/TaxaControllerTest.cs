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
        private readonly Mock<IDiscoverDBModules> Discovery;

        private readonly CollectionServerLogin PublicLogin;
        private readonly IDictionary<string, TaxonNames.TaxonList[]> ListsForModule;

        public TaxaControllerTest()
        {
            var Modules = new[] {
                new DBModule(DBModuleType.Collection, "ACollection"),
                new DBModule(DBModuleType.TaxonNames, "BTaxa"),
                new DBModule(DBModuleType.TaxonNames, "CTaxa"),
                new DBModule(DBModuleType.Unknown, "Unknown")
            };

            ListsForModule = new Dictionary<string, TaxonNames.TaxonList[]>()
            {
                {"BTaxa", new[] {
                    new TaxonNames.TaxonList() { ListID = 0, DataSource = "B1", DisplayText = "ListB1", TaxonomicGroup = "plants" },
                    new TaxonNames.TaxonList() { ListID = 0, DataSource = "B2", DisplayText = "ListB2", TaxonomicGroup = "lichen" },
                }},
                {"CTaxa", new[] {
                    new TaxonNames.TaxonList() { ListID = 0, DataSource = "C1", DisplayText = "ListC1", TaxonomicGroup = "plants" },
                    new TaxonNames.TaxonList() { ListID = 0, DataSource = "C2", DisplayText = "ListC2", TaxonomicGroup = "plants" },
                }}
            };

            Discovery = SetupMocks.ModuleDiscovery(Kernel, this.Login, Modules);
            foreach (var kv in ListsForModule)
            {
                SetupMocks.Taxa(Kernel, Login, kv.Key, kv.Value);
            }
            
            InitController();
        }

        [Fact]
        public async Task CanDiscoverListsOnCurrentServer()
        {
            // Arrange
            var expected = new List<TaxonNames.TaxonList>(
                ListsForModule["BTaxa"].Concat(ListsForModule["CTaxa"])
                );

            // Act
            var lists = await Controller.Get();

            // Assert
            Assert.True(lists.All(l => expected.Any(x => x.DisplayText == l.Name && x.TaxonomicGroup == l.TaxonGroup)));
        }

        [Fact]
        public async Task CanDiscoverListsOnPublicServer()
        {
            // Arrange

            // Act

            // Assert
            Assert.False(true);
        }

        [Fact]
        public async Task CanRetrieveNamesOnCurrentServer()
        {
            // Arrange

            // Act

            // Assert
            Assert.False(true);
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