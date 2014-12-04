﻿namespace DiversityService.API.Controllers
{
    using DiversityService.API.Filters;
    using DiversityService.API.Model;
    using DiversityService.API.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    [CollectionAPI("series")]
    public class SeriesController : DiversityController
    {
        private readonly IMappingService Mapper;

        private IStore<Collection.EventSeries, int> SeriesStore
        {
            get
            {
                return Request.GetCollectionContext().Series;
            }
        }

        public SeriesController(
            IMappingService mapper
            )
        {
            this.Mapper = mapper;
        }

        public async Task<IEnumerable<EventSeries>> Get(string query = null)
        {
            var seriesQuery = await SeriesStore.GetQueryableAsync();

            if (!string.IsNullOrEmpty(query))
            {
                seriesQuery = seriesQuery
                .Where(x => x.Code.Contains(query));
            }

            return Mapper.Project<Collection.EventSeries, EventSeries>(seriesQuery);
        }

        public async Task<IHttpActionResult> Get(int id)
        {
            var series = await SeriesStore.GetByIDAsync(id);

            if (series == null)
            {
                return NotFound();
            }

            var dto = Mapper.Map<EventSeries>(series);

            return Ok(dto);
        }

        public async Task<IHttpActionResult> Post(EventSeriesBindingModel value)
        {
            var existing = await RedirectToExisting(SeriesStore, value.TransactionGuid);

            if (existing != null)
            {
                return existing;
            }

            var series = Mapper.Map<Collection.EventSeries>(value);

            await SeriesStore.InsertAsync(series);

            return CreatedAtRoute(Route.DEFAULT_API, Route.GetById(series), series.Id);
        }
    }
}