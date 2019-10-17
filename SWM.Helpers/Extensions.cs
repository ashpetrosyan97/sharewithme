using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SWM.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Filteres a sequence of values based on predicate if condition is true
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (!condition)
                return source;
            else
                return source
                    .Where(predicate);
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (!condition)
                return source;
            else
                return source
                    .Where(predicate);
        }

        /// <summary>
        /// Updates 'oldValue' object's values from 'newValue'
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static void UpdateProps(object oldValue, object newValue)
        {
            Dictionary<object, object> changes = new Dictionary<object, object>();

            foreach (var property in newValue.GetType().GetProperties())
            {
                if (property.GetValue(newValue, null) != null || property.GetType().IsGenericType)
                {
                    property.SetValue(oldValue, newValue.GetType().GetProperty(property.Name).GetValue(newValue, null));
                    changes.Add(property.Name, oldValue.GetType().GetProperty(property.Name).GetValue(oldValue, null));
                }

            }


        }
        /// <summary>
        /// Performs the specified action on each element of the List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector">delegate for list item and index</param>
        public static void ForEach<T>(this IList<T> source, Action<T, int> selector)
        {
            if (!source.Any())
                return;

            for (int i = 0; i < source.Count(); i++)
            {
                selector(source[i], i);
            }
        }
    }
}
