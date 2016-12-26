namespace Newsie.Utilities
{
    public interface ICacheManager
    {
        void Write<T>(string key, T value);

        bool TryRead<T>(string key, out T value);
    }
}
