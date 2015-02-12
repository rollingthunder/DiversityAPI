﻿namespace DiversityService.API.Model
{
    using DiversityService.API.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class IdentificationBindingModel : IdentificationUpload, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.TransactionGuid == Guid.Empty)
            {
                yield return new ValidationResult(Messages.No_TransactionGuid);
            }
            if (string.IsNullOrWhiteSpace(this.Uri))
            {
                yield return new ValidationResult(Messages.Identification_NoUri);
            }
            if (!(this.RelatedId.HasValue && string.IsNullOrWhiteSpace(this.RelationType)))
            {
                yield return new ValidationResult(Messages.Identification_PartialRelation);
            }
        }
    }
}