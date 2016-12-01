
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
        public static List<Tuple<string, string>> GetSort(string SortString, ref string LastSort, ref string LastSortDirection)
        {
            List<Tuple<string, string>> sortExpressions = new List<Tuple<string, string>>();
            if (string.IsNullOrWhiteSpace(SortString))
            {
                // If no sorting string, give a message and return.
                System.Diagnostics.Debug.WriteLine("Please submit in a sorting string.");
                return sortExpressions;
            }

            try
            {
                // Prepare the sorting string into a list of Tuples
                //var sortExpressions = new List<Tuple<string, string>>();
                string[] terms = SortString.Split(',');
                for (int i = 0; i < terms.Length; i++)
                {
                    string[] items = terms[i].Trim().Split('~');
                    var fieldName = items[0].Trim();
                    var sortOrder = (items.Length > 1)
                              ? items[1].Trim().ToLower() : "";
                    if ((sortOrder != "asc") && (sortOrder != "desc"))
                    {
                        //throw new ArgumentException("Invalid sorting order");
                        if (LastSort == fieldName)
                        {
                            if (LastSortDirection == "desc")
                                sortOrder = "asc";
                            else
                                sortOrder = "desc";
                        }
                        else
                            sortOrder = "asc";

                    }
                    LastSort = fieldName;
                    LastSortDirection = sortOrder;
                    sortExpressions.Add(new Tuple<string, string>(fieldName, sortOrder));
                }


            }
            catch (Exception e)
            {
                var msg = "There is an error in your sorting string.  Please correct it and try again - "
              + e.Message;
                System.Diagnostics.Debug.WriteLine(msg);
                sortExpressions.Clear();
            }
            return sortExpressions;
        }

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
                //{
                //    orderedQuery = query.OrderBy(expression);
                //}
                //else
                //{
                //    orderedQuery = query.OrderByDescending(expression);
                //}


                {
                    orderedQuery = (index == 0) ? query.OrderBy(expression)
                      : orderedQuery.ThenBy(expression);
                }
                else
                {
                    orderedQuery = (index == 0) ? query.OrderByDescending(expression)
                             : orderedQuery.ThenByDescending(expression);
                }
           
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