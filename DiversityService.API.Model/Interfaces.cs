namespace DiversityService.API.Model
{
    using System;

    public interface IIdentifiable
    {
        int Id { get; set; }
    }

    public interface ICompositeKey
    {
        object[] Values();
    }

    public interface ICompositeIdentifiable
    {
        ICompositeKey CompositeKey();
    }

    public interface IGuidIdentifiable
    {
        Guid TransactionGuid { get; set; }
    }
}