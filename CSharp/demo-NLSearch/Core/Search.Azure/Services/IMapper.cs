namespace Search.Azure.Services
{
    public interface IMapper<TArg, TResult>
    {
        TResult Map(TArg item);
    }
}