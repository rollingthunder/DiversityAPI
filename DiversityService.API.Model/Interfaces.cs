namespace DiversityService.API.Model
{
    using System;

    public interface IIdentifiable
    {
        int Id { get; set; }
    }

    public interface IGuidIdentifiable
    {
        Guid TransactionGuid { get; set; }
    }
}