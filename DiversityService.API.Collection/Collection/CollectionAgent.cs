//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiversityService.DB.Collection
{
    using System;
    using System.Collections.Generic;
    
    public partial class CollectionAgent
    {
        public int CollectionSpecimenID { get; set; }
        public string CollectorsName { get; set; }
        public string CollectorsAgentURI { get; set; }
        public Nullable<System.DateTime> CollectorsSequence { get; set; }
        public string CollectorsNumber { get; set; }
        public string Notes { get; set; }
        public string DataWithholdingReason { get; set; }
        public bool xx_IsAvailable { get; set; }
        public System.Guid RowGUID { get; set; }
    
        public virtual Specimen CollectionSpecimen { get; set; }
    }
}
