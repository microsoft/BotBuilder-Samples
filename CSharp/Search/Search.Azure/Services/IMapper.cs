namespace Search.Azure.Services
{
    public interface IMapper<T, TResult>
    {
        TResult Map(T item);
    }
}