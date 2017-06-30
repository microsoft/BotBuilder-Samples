using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;

namespace Microsoft.LUIS.API
{
    public interface IEnumeratorPageAsync<T> : IEnumerator<T>
    {
        Task<ICollection<T>> NextPageAsync();
    }

    public interface IEnumerablePage<T> : IEnumerable<T>
    {
        IEnumeratorPageAsync<T> GetPageEnumerator();
    }

    public class EnumeratorAsync<T> : IEnumeratorPageAsync<T>
    {
        private Func<int, Task<ICollection<T>>> _iterator;
        private CancellationToken _ct;

        private int _skip;
        private IEnumerator<T> _page;

        public EnumeratorAsync(Func<int, Task<ICollection<T>>> iterator, CancellationToken ct)
        {
            _iterator = iterator;
            _ct = ct;
            _page = null;
        }

        T IEnumerator<T>.Current
        {
            get
            {
                if (_page == null)
                {
                    throw new ArgumentOutOfRangeException("No page to enumerate.");
                }
                return _page.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return (this as IEnumerator<T>).Current;
            }
        }

        public void Dispose()
        {
            if (_page != null)
            {
                _page.Dispose();
                _page = null;
            }
        }

        public bool MoveNext()
        {
            bool more = _page != null && _page.MoveNext();
            if (!more)
            {
                var enumerable = NextPageAsync().Result;
                if (enumerable != null)
                {
                    _page = enumerable.GetEnumerator();
                    more = _page.MoveNext();
                }
                else
                {
                    _page = null;
                }
            }
            return more;
        }

        public void Reset()
        {
            _skip = 0;
            _page = null;
        }

        public async Task<ICollection<T>> NextPageAsync()
        {
            var enumerable = await _iterator(_skip);
            if (enumerable != null)
            {
                _skip += enumerable.Count();
            }
            return enumerable;
        }
    }

    public class EnumerableAsync<T> : IEnumerablePage<T>
    {
        private Func<int, Task<ICollection<T>>> _iterator;
        private CancellationToken _ct;

        public EnumerableAsync(Func<int, Task<ICollection<T>>> iterator, CancellationToken ct)
        {
            _iterator = iterator;
            _ct = ct;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new EnumeratorAsync<T>(_iterator, _ct) as IEnumerator;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new EnumeratorAsync<T>(_iterator, _ct) as IEnumerator<T>;
        }

        public IEnumeratorPageAsync<T> GetPageEnumerator()
        {
            return new EnumeratorAsync<T>(_iterator, _ct);
        }
    }

    public static class Extensions
    {
        public static async Task<T> FirstAsync<T>(this IEnumerablePage<T> enumerable, Func<T, bool> func)
        {
            T result = default(T);
            var pages = enumerable.GetPageEnumerator();
            ICollection<T> page;
            while (result == null && (page = await pages.NextPageAsync()) != null)
            {
                foreach (var val in page)
                {
                    if (func(val))
                    {
                        result = val;
                        break;
                    }
                }
            }
            return result;
        }

        public static async Task<ICollection<T>> AllAsync<T>(this IEnumerablePage<T> enumerable, Func<T, bool> func)
        {
            var result = new List<T>();
            var pages = enumerable.GetPageEnumerator();
            ICollection<T> page;
            while ((page = await pages.NextPageAsync()) != null)
            {
                foreach (var val in page)
                {
                    if (func(val))
                    {
                        result.Add(val);
                    }
                }
            }
            return result;
        }
    }
}
