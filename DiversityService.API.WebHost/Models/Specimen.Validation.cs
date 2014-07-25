namespace DiversityService.API.Model
{
    using DiversityService.API.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    public class SpecimenBindingModel : SpecimenUpload, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!this.CollectionDate.HasValue)
            {
                yield return new ValidationResult(Messages.Specimen_NoCollectionDate);
            }
            else if(this.CollectionDate.Value > DateTime.UtcNow)
            {
                yield return new ValidationResult(Messages.Specimen_FutureDate);
            }
        }
    }
}