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
    using Collection = DiversityService.DB.Collection;

    public class TaxaControllerTest : ControllerTestBase<TaxaController>
    {
        private readonly Mock<IDiscoverDBModules> Discovery;

        private readonly IDictionary<string, Model.TaxonList[]> ListsForModule;

        public TaxaControllerTest()
        {
            var Modules = new[] {
                new DBModule(DBModuleType.Collection, "ACollection"),
                new DBModule(DBModuleType.TaxonNames, "BTaxa"),
                new DBModule(DBModuleType.TaxonNames, "CTaxa"),
                new DBModule(DBModuleType.Unknown, "Unknown")
            };

            ListsForModule = new Dictionary<string, Model.TaxonList[]>()
            {
                {"BTaxa", new[] {
                    new Model.TaxonList() { Catalog = "BTaxa", Id = 0, Name = "B1", TaxonGroup = "plants" },
                    new Model.TaxonList() { Catalog = "BTaxa", Id = 0, Name = "B2", TaxonGroup = "lichen" },
                }},
                {"CTaxa", new[] {
                    new Model.TaxonList() { Catalog = "CTaxa", Id = 0, Name = "C1", TaxonGroup = "plants" },
                    new Model.TaxonList() { Catalog = "CTaxa", Id = 0, Name = "C2", TaxonGroup = "lichen" },
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
            var expected = new HashSet<Model.TaxonList>(
                ListsForModule["BTaxa"].Concat(ListsForModule["CTaxa"])
                );

            // Act
            var lists = await Controller.Get();

            // Assert
            Assert.Equal(expected, lists);
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