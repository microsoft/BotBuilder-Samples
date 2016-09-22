namespace ContosoFlowers.Services
{
    using System;

    public interface IRepository<T>
    {
        PagedResult<T> RetrievePage(int pageNumber, int pageSize, Func<T, bool> predicate = default(Func<T, bool>));

        T GetByName(string name);
    }
}