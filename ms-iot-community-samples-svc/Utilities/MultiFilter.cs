using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ms_iot_community_samples_svc.Utilities
{
    public static class MultiFilter
    {
        //Filter the view on one field
        public static List<Models.IoTProject> Filter(List<Tuple<string, string>> filters)
        {
            var lst = (from n in Models.IoTProject.IoTProjects select n).ToList<Models.IoTProject>();

            for (int index = 0; index < filters.Count; index++)
            {
                lst = FilterGen<Models.IoTProject>(lst, filters[index].Item1, filters[index].Item2);
            }
            return lst.ToList<Models.IoTProject>();
        }

        public static List<T> FilterGen<T>(
            List<T> collection,
            string property,
            string filterValue)
        {
            var filteredCollection = new List<T>();
            foreach (T item in collection)
            {
                // To check multiple properties use,
                // item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)

                var propertyInfo =
                    item.GetType()
                        .GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    throw new NotSupportedException("property given does not exists");
                if (    //String use contatins
                        (propertyInfo.PropertyType.Name == "string")
                        ||
                        (propertyInfo.PropertyType.Name == "String")
                    )
                {
                    var propertyValue = (string)propertyInfo.GetValue(item, null);
                    if (propertyValue.ToString().Contains(filterValue))
                        filteredCollection.Add(item);
                }
                else if ( //Id use exact
                            (propertyInfo.PropertyType.Name == "Int64")
                            ||
                            (propertyInfo.PropertyType.Name == "Int32")
                        )
                {
                    int propertyValue2 = (int)propertyInfo.GetValue(item, null);
                    if (propertyValue2.ToString() == filterValue)
                        filteredCollection.Add(item);
                }
                else if (propertyInfo.PropertyType.Name == "List`1")
                {
                    if (property == "CodeLanguages")
                    {
                        var propertyValue3 = (List<string>)propertyInfo.GetValue(item, null);
                        if (propertyValue3.Contains(filterValue))
                            filteredCollection.Add(item);
                    }
                    if (property == "Tags")
                    {
                        var propertyValue4 = (List<string>)propertyInfo.GetValue(item, null);
                        if (propertyValue4.Contains(filterValue))
                            filteredCollection.Add(item);
                    }

                }
            }

            return filteredCollection;
        }

        //    public static List<T> FilterXX<T>(List<T> list, Func<T, bool> filter)
        //    {
        //        var temp = list.AsQueryable().Where(filter);
        //        return temp.Cast<T>().ToList();
        //    }
    }

}