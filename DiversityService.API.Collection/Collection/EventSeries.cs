//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiversityService.Collection
{
    using System;
    using System.Collections.Generic;
    
    public partial class EventSeries
    {
        public EventSeries()
        {
            this.Events = new HashSet<Event>();
            this.Images = new HashSet<EventSeriesImage>();
            this.Children = new HashSet<EventSeries>();
        }
    
        public int Id { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public System.Data.Entity.Spatial.DbGeography Geography { get; set; }
        public Nullable<System.DateTime> StartDateUTC { get; set; }
        public Nullable<System.DateTime> EndDateUTC { get; set; }
        public System.Guid TransactionGuid { get; set; }
        private Nullable<int> ParentID { get; set; }
        public Nullable<System.DateTime> DateCache { get; set; }
    
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<EventSeriesImage> Images { get; set; }
        public virtual ICollection<EventSeries> Children { get; set; }
        public virtual EventSeries Parent { get; set; }
    }
}
