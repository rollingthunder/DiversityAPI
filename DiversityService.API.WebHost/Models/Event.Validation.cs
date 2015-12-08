﻿namespace DiversityService.API.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DiversityService.API.Resources;

    public class EventBindingModel : EventUpload, IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.TimeStamp > DateTime.UtcNow)
            {
                yield return new ValidationResult(Messages.Event_FutureDate);
            }

            if (string.IsNullOrWhiteSpace(this.LocalityDescription))
            {
                yield return new ValidationResult(Messages.Event_NoLocation);
            }

            if (!this.TimeStamp.HasValue)
            {
                yield return new ValidationResult(Messages.Event_NoTimeStamp);
            }

            if (this.TransactionGuid == Guid.Empty)
            {
                yield return new ValidationResult(Messages.No_TransactionGuid);
            }
        }
    }
}