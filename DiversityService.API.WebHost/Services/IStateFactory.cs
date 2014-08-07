namespace DiversityService.API.Services
{
    public interface IStateFactory<TModel, TState>
    {
        TState Map(TModel model);
    }
}
