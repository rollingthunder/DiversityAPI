namespace DiversityService.API.Model
{
    using System;

    public interface ITransactedModel
    {
        Guid TransactionGuid { get; set; }
    }
}
