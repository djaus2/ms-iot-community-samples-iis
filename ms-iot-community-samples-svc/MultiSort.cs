
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Ref: http://www.codeproject.com/Articles/280952/Multiple-Column-Sorting-by-Field-Names-Using-Linq
/// </summary>
namespace ms_iot_community_samples_svc.Utilities
{
    public static class LinqDynamicMultiSortingUtility
    {
        /// <summary>
        /// 1. The sortExpressions is a list of Tuples, the first item of the 
        ///    tuples is the field name,
        ///    the second item of the tuples is the sorting order (asc/desc) case sensitive.
        /// 2. If the field name (case sensitive) provided for sorting does not exist 
        ///    in the object,
        ///    exception is thrown
        /// 3. If a property name shows up more than once in the "sortExpressions", 
        ///    only the first takes effect.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="sortExpressions"></param>
        /// <returns></returns>
        public static IEnumerable<T> MultipleSort<T>(this IEnumerable<T> data,
          List<Tuple<string, string>> sortExpressions)
        {
            // No sorting needed
            if ((sortExpressions == null) || (sortExpressions.Count <= 0))
            {
                return data;
            }

            // Let us sort it
            IEnumerable<T> query = from item in data select item;
            IOrderedEnumerable<T> orderedQuery = null;

            for (int i = 0; i < sortExpressions.Count; i++)
            {
                // We need to keep the loop index, not sure why it is altered by the Linq.
                var index = i;
                Func<T, object> expression = item => item.GetType()
                                .GetProperty(sortExpressions[index].Item1)
                                .GetValue(item, null);

                if (sortExpressions[index].Item2 == "asc")
                {
                    orderedQuery = query.OrderBy(expression);
                }
                else
                {
                    orderedQuery = query.OrderByDescending(expression);
 
                }
                //{
                //    orderedQuery = (index == 0) ? query.OrderBy(expression)
                //      : orderedQuery.ThenBy(expression);
                //}
                //else
                //{
                //    orderedQuery = (index == 0) ? query.OrderByDescending(expression)
                //             : orderedQuery.ThenByDescending(expression);
                //}
            }

            query = orderedQuery;

            return query;
        }


        public static IEnumerable<T> MultipleFilter<T>(this IEnumerable<T> data,
  List<Tuple<string, string>> filterExpressions)
        {
            // No filtering needed
            if ((filterExpressions == null) || (filterExpressions.Count <= 0))
            {
                return data;
            }

            // Let us sort it
            IEnumerable<T> query = from item in data select item;
            IOrderedEnumerable<T> orderedQuery = null;

            for (int i = 0; i < filterExpressions.Count; i++)
            {
                // We need to keep the loop index, not sure why it is altered by the Linq.
                var index = i;
                Func<T, object> expression = item => item.GetType()
                                .GetProperty(filterExpressions[index].Item1)
                                .GetValue(item, null);

                //var neworderedQuery = from n in orderedQuery where expression. == sortExpressions[index].Item2 select n;
                //orderedQuery = neworderedQuery;

                //if (filterExpressions[index].Item2 == "asc")
                //{
                //    orderedQuery = (index == 0) ? query.OrderBy(expression)
                //      : orderedQuery.ThenBy(expression);
                //}
                //else
                //{
                //    orderedQuery = (index == 0) ? query.OrderByDescending(expression)
                //             : orderedQuery.ThenByDescending(expression);
                //}
            }

            query = orderedQuery;

            return query;
        }
    }

    
}