namespace DiversityService.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DiversityService.API.Resources;

    public class SpecimenBindingModel : SpecimenUpload, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!this.CollectionDate.HasValue)
            {
                yield return new ValidationResult(Messages.Specimen_NoCollectionDate);
            }

            if (this.TransactionGuid == Guid.Empty)
            {
                yield return new ValidationResult(Messages.No_TransactionGuid);
            }
        }
    }
}