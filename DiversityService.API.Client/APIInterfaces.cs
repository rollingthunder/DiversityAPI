﻿namespace DiversityService.API.Client
{
    using DiversityService.API.Model;
    using Refit;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Headers("Authorization: Bearer")]
    public interface ISeriesAPI
    {
        [Get("")]
        Task<IEnumerable<EventSeries>> GetAll(int take = 10, int skip = 0);

        [Get("/{id}")]
        Task<IEnumerable<EventSeries>> Get(int id, int take, int skip);

        [Post("")]
        Task<EventSeries> Create(EventSeriesUpload es);
    }
}