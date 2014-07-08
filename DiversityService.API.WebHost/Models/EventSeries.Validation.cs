namespace DiversityService.API.Model
{
    using DiversityService.API.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public partial class EventSeries : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (!this.StartDateUTC.HasValue)
            {
                yield return new ValidationResult(Messages.Series_NoStartDate);
            }
            else if(this.StartDateUTC.Value > DateTime.UtcNow)
            {
                yield return new ValidationResult(Messages.Series_FutureDate);
            }

            if (this.EndDateUTC.HasValue)
            {
                if(this.EndDateUTC.Value > this.StartDateUTC.Value)
                {
                    yield return new ValidationResult(Messages.Series_EndBeforeStart);
                }
                else if(this.EndDateUTC.Value > DateTime.UtcNow)
                {
                    yield return new ValidationResult(Messages.Series_FutureDate);
                }
            }
            
            if(string.IsNullOrWhiteSpace(this.Description))
            {
                yield return new ValidationResult(Messages.Series_NoDescription);
            }
        }
    }
}
