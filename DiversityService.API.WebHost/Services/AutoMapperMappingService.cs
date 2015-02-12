﻿namespace DiversityService.API.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class AutoMapperMappingService : IMappingService
    {
        public readonly IMappingEngine Engine;

        public AutoMapperMappingService(IMappingEngine mapper)
        {
            Engine = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return Engine.Map<TDestination>(source);
        }

        public IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> sourceQuery)
        {
            return sourceQuery.Project<TSource>(Engine).To<TDestination>();
        }
    }
}