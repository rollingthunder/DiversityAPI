namespace DiversityService.API.Test
{
    using DiversityService.API.Services;
    using DiversityService.DB.Collection;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Xunit;

    public class StoreTest : TestBase
    {
        private Mock<DiversityCollection> Context;
        private Mock<DbSet<Event>> SimpleKeySet;
        private Mock<DbSet<IdentificationUnit>> CompositeKeySet;

        private readonly Store<Event, int> SimpleStore;
        private readonly Store<IdentificationUnit, IdentificationUnitKey> CompositeStore;

        public StoreTest()
        {
            SimpleKeySet = new Mock<DbSet<Event>>();
            CompositeKeySet = new Mock<DbSet<IdentificationUnit>>();

            Context = new Mock<DiversityCollection>();
            Context.Setup(x => x.Set<Event>())
                .Returns(SimpleKeySet.Object);
            Context.Setup(x => x.Set<IdentificationUnit>())
                .Returns(CompositeKeySet.Object);

            SimpleStore = new Store<Event, int>(Context.Object);
            CompositeStore = new Store<IdentificationUnit, IdentificationUnitKey>(Context.Object);
        }

        [Fact]
        public async Task CanFindByKeySimple()
        {
            // Arrange
            var data = new[] {
                new Event(){ Id = TestHelper.RandomInt() },
                new Event(){ Id = TestHelper.RandomInt() },
                new Event(){ Id = TestHelper.RandomInt() },
                new Event(){ Id = TestHelper.RandomInt() },
            };
            SetupDbSet(SimpleKeySet, data.AsQueryable());

            var expected = data[0];
            var id = expected.Id;

            SimpleKeySet.Setup(x => x.FindAsync(id))
                .ReturnsAsync(expected);

            // Act
            var ev = await SimpleStore.GetByIDAsync(id);

            // Assert
            Assert.Equal(expected, ev);
        }

        [Fact]
        public async Task CanFindByKeyComposite()
        {
            // Arrange
            var data = new[] {
                new IdentificationUnit(){ Id = TestHelper.RandomInt(), SpecimenId = TestHelper.RandomInt() },
                new IdentificationUnit(){ Id = TestHelper.RandomInt(), SpecimenId = TestHelper.RandomInt() },
                new IdentificationUnit(){ Id = TestHelper.RandomInt(), SpecimenId = TestHelper.RandomInt() },
                new IdentificationUnit(){ Id = TestHelper.RandomInt(), SpecimenId = TestHelper.RandomInt() },
            };
            SetupDbSet(CompositeKeySet, data.AsQueryable());

            var expected = data[0];
            var id = expected.CompositeKey();

            CompositeKeySet.Setup(x => x.FindAsync(It.Is<object[]>(y => Enumerable.SequenceEqual(y, id.Values()))))
                .ReturnsAsync(expected);

            // Act
            var iu = await CompositeStore.GetByIDAsync(id);

            // Assert
            Assert.Equal(expected, iu);
        }

        private void SetupDbSet<T>(Mock<DbSet<T>> mockSet, IQueryable<T> data)
            where T : class
        {
            mockSet.As<IDbAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        }
    }
}