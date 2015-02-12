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

    public interface ICompositeIdentifiable<TKey>
        where TKey : ICompositeKey
    {
        TKey CompositeKey();
    }

    public interface IGuidIdentifiable
    {
        Guid TransactionGuid { get; set; }
    }
}