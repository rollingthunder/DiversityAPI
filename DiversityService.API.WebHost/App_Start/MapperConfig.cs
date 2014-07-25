namespace DiversityService.API.WebHost
{
    using AutoMapper;
    using DiversityService.API.Model;
    using System;

    public static class MapperConfig
    {
        private static DateTime? ToUTC(this DateTime? This )
        {
            if(This.HasValue)
            {
                return This.Value.ToUniversalTime();
            }

            return null;
        }

        private static DateTime? ToLocal(this DateTime? This)
        {
            if (This.HasValue)
            {
                return This.Value.ToLocalTime();
            }

            return null;
        }

        public static void Configure()
        {
            Mapper.CreateMap<CollectionServerElement, CollectionServer>();

            Mapper.CreateMap<Collection.EventSeries, EventSeries>()
                .ForMember(es => es.StartDateUTC, map => map.MapFrom(es => es.StartDateLocal.ToUTC()))
                .ForMember(es => es.EndDateUTC, map => map.MapFrom(es => es.EndDateLocal.ToUTC()));

            Mapper.CreateMap<EventSeriesCommon, Collection.EventSeries>()
                .Include<EventSeriesBindingModel, Collection.EventSeries>()
                .Include<EventSeries, Collection.EventSeries>()
                .ForMember(es => es.StartDateLocal, map => map.MapFrom(es => es.StartDateUTC.ToLocal()))
                .ForMember(es => es.EndDateLocal, map => map.MapFrom(es => es.EndDateUTC.ToLocal()));
            Mapper.CreateMap<EventSeriesBindingModel, Collection.EventSeries>()
                .ForMember(es => es.RowGUID, map => map.MapFrom(es => es.TransactionGuid));
            Mapper.CreateMap<EventSeries, Collection.EventSeries>();
        }
    }
}