using System;
using System.Collections.Generic;
using System.Linq;

namespace BETest.Extensions
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection) action(item);
        }

        public static IEnumerable<T> DistinctBy<T, TSelector>(this IEnumerable<T> collection, Func<T, TSelector> selectorMethod)
        {
            HashSet<TSelector> seenItems = new HashSet<TSelector>();

            foreach (T item in collection)
            {
                TSelector selector = selectorMethod(item);
                if (seenItems.Contains(selector)) continue;
                seenItems.Add(selector);
                yield return item;
            }
        }

        public static IEnumerable<T> GetPage<T>(this IEnumerable<T> collection, int page, int pageSize)
        {
            return collection.Skip(page * pageSize).Take(pageSize);
        }

        public static int FindPageOfFirstOccurence<T>(this IEnumerable<T> collection, int pageSize, Func<T, bool> criteria)
        {
            T[] collectionArray = collection as T[] ?? collection.ToArray();
            int lastPage = collectionArray.GetLastPage(pageSize);
            for (int page = 0; page <= lastPage; ++page)
            {
                IEnumerable<T> collectionPage = collectionArray.GetPage(page, pageSize).ToArray();
                if (collectionPage.Any(criteria)) return page;
            }
            return -1;
        }

        public static int GetLastPage<T>(this IEnumerable<T> collection, int pageSize)
        {
            return (collection.Count() - 1) / pageSize;
        }
    }
}