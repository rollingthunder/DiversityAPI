namespace DiversityService.API.Test
{
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using Ninject;
    using NodaTime;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class MappingTest : TestBase
    {
        protected readonly AutoMapperMappingService Mapper;

        public MappingTest()
        {
            Mapper = (AutoMapperMappingService)Kernel.Get<IMappingService>();
        }

        [Fact]
        public void ConfiguratonValid()
        {
            // Arrange

            // Act

            // Assert
            Mapper.Engine.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public class MapEventSeries : MappingTest
        {
            [Fact]
            public void ToDTO()
            {
                // Arrange
                var es = new Collection.EventSeries()
                {
                    Code = "seriesCode",
                    Description = "seriesDescription",
                    EndDateUTC = DateTime.UtcNow,
                    // TODO Geography =
                    Id = 10101,
                    StartDateUTC = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                    // TransactionGuid = Guid.NewGuid()
                };

                // Act
                var dto = Mapper.Map<EventSeries>(es);

                // Assert
                Assert.Equal(es.Code, dto.Code);
                Assert.Equal(es.Description, dto.Description);
                Assert.Equal(es.EndDateUTC, dto.EndDateUTC);
                Assert.Equal(es.Id, dto.Id);
                Assert.Equal(es.StartDateUTC, dto.StartDateUTC);
            }

            [Fact]
            public void ToLocalization()
            {
                // Arrange
                var es = new Collection.EventSeries()
                {
                    Code = "seriesCode",
                    Description = "seriesDescription",
                    EndDateUTC = DateTime.UtcNow,
                    // TODO Geography =
                    Id = 10101,
                    StartDateUTC = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                    // TransactionGuid = Guid.NewGuid()
                };

                // Act
                var dto = Mapper.Map<EventSeries>(es);

                // Assert
                Assert.True(false);
            }

            [Fact]
            public void FromDTO()
            {
                // Arrange
                var dto = new EventSeriesBindingModel()
                {
                    Code = "seriesCode",
                    Description = "seriesDescription",
                    EndDateUTC = DateTime.UtcNow,
                    Id = 10101,
                    StartDateUTC = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),

                    // BindingModel
                    TransactionGuid = Guid.NewGuid()
                };

                // Act
                var entity = Mapper.Map<Collection.EventSeries>(dto);

                // Assert
                Assert.Equal(dto.Code, entity.Code);
                Assert.Equal(dto.Description, entity.Description);
                Assert.Equal(dto.EndDateUTC, entity.EndDateUTC);
                Assert.Equal(dto.Id, entity.Id);
                Assert.Equal(dto.StartDateUTC, entity.StartDateUTC);

                Assert.Equal(dto.TransactionGuid, entity.TransactionGuid);
            }
        }

        public class MapIdentification : MappingTest
        {
            [Fact]
            public void IUToDTO()
            {
                // Arrange
                var iu = new Collection.IdentificationUnit()
                {
                    Circumstances = "trying",
                    // ColonisedSubstratePart
                    Id = 1098293,
                    //OnlyObserved = true,
                    RelatedUnitId = 1234,
                    RelationType = "growingon",
                    SpecimenId = 12435,
                    TaxonomicGroup = "homini",
                    //UnitDescription = "descriptive string",
                    LastIdentificationCache = "testident"
                };

                // Act
                var dto = Mapper.Map<Identification>(iu);

                // Assert
                // TODO Assert.Equal(id.Circumstances, dto.Circumstances);
                Assert.Equal(iu.Id, dto.Id);
                Assert.Equal(iu.RelatedUnitId, dto.RelatedId);
                Assert.Equal(iu.RelationType, dto.RelationType);
                Assert.Equal(iu.SpecimenId, dto.SpecimenId);
                Assert.Equal(iu.TaxonomicGroup, dto.TaxonomicGroup);

                Assert.Null(dto.Localization); // not part of mapping
                Assert.Null(dto.Name); // not part of mapping
                Assert.Null(dto.RelatedId); // not part of mapping
                Assert.Null(dto.RelationType); // not part of mapping
                Assert.Null(dto.TaxonomicGroup); // not part of mapping
            }

            [Fact]
            public void IDToDTO()
            {
                // Arrange
                var idDate = DateTime.UtcNow.AddDays(3);
                var id = new Collection.Identification()
                {
                    Id = 109,
                    //IdentificationCategory = "determination",
                    IdentificationDate = idDate,
                    //IdentificationDateCategory = "actual",
                    //IdentificationDateSupplement
                    IdentificationDay = (byte?)idDate.Day,
                    IdentificationMonth = (byte?)idDate.Month,
                    //IdentificationQualifier =
                    IdentificationUnitID = TestHelper.RandomInt(),
                    IdentificationYear = (short?)idDate.Year,
                    NameURI = "testuri",
                    //Notes =
                    //ReferenceDetails
                    //ReferenceTitle
                    //ReferenceURI
                    ResponsibleAgentURI = "testpersonuri",
                    ResponsibleName = "testperson",
                    SpecimenID = TestHelper.RandomInt(),
                    TaxonomicName = "testplant",
                    TransactionGuid = Guid.NewGuid(),
                    //TypeNotes
                    //TypeStatus
                    //VernacularTerm
                };

                // Act
                var dto = Mapper.Map<Identification>(id);

                // Assert
                Assert.NotEqual(id.Id, dto.Id);
                Assert.NotNull(id.IdentificationDate);
                Assert.NotEqual(id.IdentificationDate.Value, dto.Date.AtMidnight().ToDateTimeUnspecified());
                Assert.Equal(id.IdentificationUnitID, dto.Id);
                Assert.Equal(id.NameURI, dto.Uri);
                Assert.Equal(id.SpecimenID, dto.SpecimenId);

                Assert.Null(dto.Localization); // not part of mapping
                Assert.Null(dto.Name); // not part of mapping
                Assert.Null(dto.RelatedId); // not part of mapping
                Assert.Null(dto.RelationType); // not part of mapping
                Assert.Null(dto.TaxonomicGroup); // not part of mapping
            }

            [Fact]
            public void IUGANToDTO()
            {
                // Arrange
                var geo = DbGeography.PointFromText("POINT(1 4 3)", DbGeography.DefaultCoordinateSystemId);

                var iugan = new Collection.IdentificationUnitGeoAnalysis()
                {
                    AnalysisDate = DateTime.UtcNow,
                    CollectionSpecimenID = TestHelper.RandomInt(),
                    Geography = geo,
                    // Geometry
                    IdentificationUnitID = TestHelper.RandomInt(),
                    //Notes
                    //ResponsibleAgentURI,
                    //ResponsibleName,
                    RowGUID = Guid.NewGuid()
                };

                // Act
                var dto = Mapper.Map<Identification>(iugan);

                // Assert
                Assert.Equal(iugan.CollectionSpecimenID, dto.SpecimenId);
                Assert.NotNull(dto.Localization);
                Assert.Equal(geo.Latitude.Value, dto.Localization.Latitude);
                Assert.Equal(geo.Longitude.Value, dto.Localization.Longitude);
                Assert.Equal(geo.Elevation.Value, dto.Localization.Altitude);
                Assert.Equal(iugan.IdentificationUnitID, dto.Id);

                Assert.Null(dto.Date); // Not part of mapping
            }

            [Fact]
            public void FromDTO()
            {
                // Arrange
                var dto = new EventSeriesBindingModel()
                {
                    Code = "seriesCode",
                    Description = "seriesDescription",
                    EndDateUTC = DateTime.UtcNow,
                    Id = 10101,
                    StartDateUTC = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),

                    // BindingModel
                    TransactionGuid = Guid.NewGuid()
                };

                // Act
                var entity = Mapper.Map<Collection.EventSeries>(dto);

                // Assert
                Assert.Equal(dto.Code, entity.Code);
                Assert.Equal(dto.Description, entity.Description);
                Assert.Equal(dto.EndDateUTC, entity.EndDateUTC);
                Assert.Equal(dto.Id, entity.Id);
                Assert.Equal(dto.StartDateUTC, entity.StartDateUTC);

                Assert.Equal(dto.TransactionGuid, entity.TransactionGuid);
            }
        }

        public class ToLocalization
        {
            [Fact]
            public void Point()
            {
                // Arrange (Lon Lat Z M)
                var geo = DbGeography.PointFromText("POINT(1 4 3)", DbGeography.DefaultCoordinateSystemId);

                // Act
                var dto = geo.ToLocalization();

                // Assert
                Assert.Equal(geo.Longitude.Value, dto.Longitude);
                Assert.Equal(geo.Latitude.Value, dto.Latitude);
                Assert.Equal(geo.Elevation, dto.Altitude);
            }
        }

        public class ToTour
        {
            [Fact]
            public void Linestring()
            {
                // Arrange
                var geo = DbGeography.FromText("LINESTRING(0 0 0, 0 1 1, 1 1 1, 1 0 1, 0 0 0 ) ", DbGeography.DefaultCoordinateSystemId);
                var pts = new[]{
                    new Localization(0, 0, 0),
                    new Localization(0, 1, 1),
                    new Localization(1, 1, 1),
                    new Localization(1, 0, 1),
                    new Localization(0, 0, 0),
                } as IEnumerable<Localization>;

                // Act
                var tour = geo.ToTour();

                // Assert
                Assert.Equal(pts, tour);
            }
        }
    }
}