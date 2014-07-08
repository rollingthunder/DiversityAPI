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
    
    public partial class IdentificationUnit
    {
        public IdentificationUnit()
        {
            this.CollectionSpecimenImage = new HashSet<SpecimenImage>();
            this.Identification = new HashSet<Identification>();
            this.IdentificationUnit1 = new HashSet<IdentificationUnit>();
            this.IdentificationUnit11 = new HashSet<IdentificationUnit>();
            this.IdentificationUnitAnalysis = new HashSet<IdentificationUnitAnalysis>();
            this.IdentificationUnitGeoAnalysis = new HashSet<IdentificationUnitGeoAnalysis>();
        }
    
        public int CollectionSpecimenID { get; set; }
        public int IdentificationUnitID { get; set; }
        public string LastIdentificationCache { get; set; }
        public string FamilyCache { get; set; }
        public string OrderCache { get; set; }
        public string TaxonomicGroup { get; set; }
        public Nullable<bool> OnlyObserved { get; set; }
        public Nullable<int> RelatedUnitID { get; set; }
        public string RelationType { get; set; }
        public string ColonisedSubstratePart { get; set; }
        public string LifeStage { get; set; }
        public string Gender { get; set; }
        public Nullable<short> NumberOfUnits { get; set; }
        public string ExsiccataNumber { get; set; }
        public Nullable<short> ExsiccataIdentification { get; set; }
        public string UnitIdentifier { get; set; }
        public string UnitDescription { get; set; }
        public string Circumstances { get; set; }
        public short DisplayOrder { get; set; }
        public string Notes { get; set; }
        public Nullable<System.DateTime> LogCreatedWhen { get; set; }
        public string LogCreatedBy { get; set; }
        public Nullable<System.DateTime> LogUpdatedWhen { get; set; }
        public string LogUpdatedBy { get; set; }
        public Nullable<int> xx_SubstrateID { get; set; }
        public string xx_SubstrateRelationType { get; set; }
        public Nullable<int> xx_SpecimenPartID { get; set; }
        public Nullable<int> xx_NewUnitID { get; set; }
        public Nullable<int> xx_AlteUnitID { get; set; }
        public System.Guid RowGUID { get; set; }
        public string HierarchyCache { get; set; }
        public Nullable<int> ParentUnitID { get; set; }
    
        public virtual Specimen CollectionSpecimen { get; set; }
        public virtual ICollection<SpecimenImage> CollectionSpecimenImage { get; set; }
        public virtual ICollection<Identification> Identification { get; set; }
        public virtual ICollection<IdentificationUnit> IdentificationUnit1 { get; set; }
        public virtual IdentificationUnit IdentificationUnit2 { get; set; }
        public virtual ICollection<IdentificationUnit> IdentificationUnit11 { get; set; }
        public virtual IdentificationUnit IdentificationUnit3 { get; set; }
        public virtual ICollection<IdentificationUnitAnalysis> IdentificationUnitAnalysis { get; set; }
        public virtual ICollection<IdentificationUnitGeoAnalysis> IdentificationUnitGeoAnalysis { get; set; }
    }
}
