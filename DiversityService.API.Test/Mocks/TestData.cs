namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using Collection = DiversityService.DB.Collection;

    public class TestData
    {
        public InternalCollectionServer[] Servers { get; set; }

        // Database Contents
        public Collection.EventSeries[] Series { get; set; }

        public Collection.Event[] Events { get; set; }

        public Collection.Specimen[] Specimen { get; set; }

        public Collection.Identification[] Identification { get; set; }

        public Collection.IdentificationUnit[] IdentificationUnit { get; set; }

        public Collection.IdentificationUnitGeoAnalysis[] IdentificationGeoAnalysis { get; set; }

        public static TestData Default()
        {
            return new TestData()
            {
                Servers = new[] {
                    new InternalCollectionServer() {
                        Address = "diversityapi.de",
                        Catalog = "CollectionTest",
                        Id = 0,
                        Name = "Test",
                        Port = 1123
                    }
                },
                Series = new[] {
                    new Collection.EventSeries() { Code = "Code1", Id = 0},
                    new Collection.EventSeries() { Code = "Code2", Id = 4},
                    new Collection.EventSeries() { Code = "Code3", Id = 2},
                    new Collection.EventSeries() { Code = "Code4", Id = 1}
                },
                Events = new[] {
                    new Collection.Event() {
                        Id = 0
                }
                }
            };
        }
    }
}