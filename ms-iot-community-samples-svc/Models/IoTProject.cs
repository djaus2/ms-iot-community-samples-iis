using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ms_iot_community_samples_svc.Utilities;
using System.Reflection;
using Nancy;
using Newtonsoft.Json;

namespace ms_iot_community_samples_svc.Models
{
    [Serializable]
    public class Project
    {
        public string Version { get; set; } = "";
        public string Filename { get; set; } = "";
        public string Title { get; set; } = "";

        public string Description { get; set; } = "";

        public List<string> Developers { get; set; } = new List<string>();

        public string Blog { get; set; } = "";

        public List<string> CodeLanguages { get; set; } = new List<string>();

        public List<string> Targets { get; set; } = new List<string>();

        public string Language { get; set; } = "en-us";

        public string GitHub { get; set; } = "";

        public string HacksterIO { get; set; } = "";

        public string Codeplex { get; set; } = "";

        public List<string> Tags { get; set; } = new List<string>();
    }

    [Serializable]
    public class IoTProject: Project
    {
        private static int Count = 0;
        public int Id { get; set; }

        //<a href="/Show/1">@objUser.Title</a>
        public string TitleId
        {
            get
            {
                string ret = "/ms_iot_Community_Samples/Show/" + Id;
                return ret;
            }
        }
        public string filenameDisp
        {
            get
            {
                string ret = "/ms_iot_Community_Samples/display/" + Filename;
                return ret;
            }
        }

        public string GitHubUrl
        {
            get
            {
                if (GitHub == "")
                    return "";
                string ret = "https://github.com/" + GitHub;
                return ret;
            }
        }
        
        public string OtherGitHub
        {
            get
            {
                if (GitHub == "")
                    return "";
                return "Developer's Other GitHub Projects";
            }
        }

        public string OtherGitHubProjectsUrl
        {
            get
            {
                if (GitHub == "")
                    return "";

                string[] parts = GitHub.Split(new char[] { '/' });
                string developer = parts[0].Trim();
                if (developer == "")
                    return "";

                string ret = "https://github.com/" + developer + "?tab=repositories";
                return ret;
            }
        }

        public string HacksterIOUrl
        {
            get
            {
                if (HacksterIO == "")
                    return "";
                string ret = "https://www.hackster.io/" + HacksterIO;
                return ret;
            }
        }

        public string OtherHacksterIO
        {
            get
            {
                if (HacksterIO == "")
                    return "";
                return "Developer's Other HacksterIO Projects";
            }
        }

        public string OtherHacksterIOProjectsUrl
        {
            get
            {
                if (HacksterIO == "")
                    return "";

                string[] parts = HacksterIO.Split(new char[] { '/' });
                string developer = parts[0].Trim();
                if (developer == "")
                    return "";

                string ret = "https://www.hackster.io/" + developer + "/projects?ref=topnav";
                return ret;
            }
        }

        


        //public string Title { get; set; }
        //public string Summary { get; set; }

        private const int SummarySubLength = 50;

        public string SummarySub
        {
            get
            {

                if (Description.Length > SummarySubLength)
                    return Description.Substring(0, SummarySubLength) + " ...";
                else
                    return Description;

            }
        }

        //public string layout { get; set; } = "";
        //public string filename { get; set; } = "";
        //public string title { get; set; } = "";
        //public string description { get; set; } = "";
        //public string keyword { get; set; } = "";
        //public string permalink { get; set; } = "";
        //public string samplelink { get; set; } = "";
        //public string lang { get; set; } = "en-us";

        //public string Filename { get; set; } = "";
        //public string Title { get; set; } = "";

        //public string Description { get; set; }

        //public List<string> Authors { get; set; }

        //public string Blog { get; set; }

        //public List<string> CodeLanguages { get; set; }

        //public string Language { get; set; } = "en-us";

        //public string GitHubRepository_URL { get; set; }

        //public string HacksterIO_URL { get; set; } = "";

        //public List<string> Tags { get; set; }

        public static List<string> Fields;

        public IoTProject()
        {
            //Tags = new List<string>();
            //Authors = new List<string>();
            //CodeLanguages = new List<string>();
            if (IoTProjects == null)
            {
                ClearIoTProjects();
            }
            Id = Count++;
            IoTProjects.Add(this);
        }

        //This is the underlying collection of BlogPosts that gets filtered and sorted.
        private static List<IoTProject> _IoTProjects = null;
        public static List<IoTProject> IoTProjects {
            get { return _IoTProjects;  }
            set
            {
                _IoTProjects = value;
            }
        }
        
        public string Tagz
        {
            get
            {
                string res="";
                res = String.Join(", ", Tags);

                return res;
            }
        }

        public string CodeLanguagez
        {
            get
            {
                string res = "";
                res = String.Join(", ", CodeLanguages);

                return res;
            }
        }

        public string Targetz
        {
            get
            {
                string res = "";
                res = String.Join(", ", Targets);

                return res;
            }
        }

        public string Developerz
        {
            get
            {
                string res = "";
                res = String.Join(", ", Developers);

                return res;
            }
        }

        public static List<IoTProject> ViewIoTProjects(string filtersStr)
        {
            var projects = from n in IoTProjects select n;
            List<IoTProject> projectList = projects.ToList<IoTProject>();
            if (filtersStr != "")
            {
                JsonSerializerSettings set = new JsonSerializerSettings();
                set.MissingMemberHandling = MissingMemberHandling.Ignore;
                var filter = JsonConvert.DeserializeObject<FilterAndSortInfo>(filtersStr, set);
                if (filter.Filters != null)
                {
                    //Filter on all IoTProjects
                    projectList = Utilities.MultiFilter.Filter(filter.Filters);
                }
                if (filter.SortExpressions != null)
                {
                    //Sort the sublist
                    var projs = projectList.MultipleSort(filter.SortExpressions);
                    var projsList  = projs.ToList<Models.IoTProject>();
                    projectList = projsList;
                }

            }
            return projectList;
        }

        /// <summary>
        /// Use this in views
        /// </summary>
        ////public static List<IoTProject> ViewIoTProjectsy (string filtersStr)
        ////{
        ////    var blogs = from n in IoTProjects select n;
        ////    List<IoTProject> blogg = blogs.ToList<IoTProject>();
        ////    if (filtersStr != "")
        ////    {
        ////        JsonSerializerSettings set = new JsonSerializerSettings();
        ////        set.MissingMemberHandling = MissingMemberHandling.Ignore;
        ////        var filter = JsonConvert.DeserializeObject<FilterAndSortInfo>(filtersStr, set);
        ////        if (filter.Filters != null)
        ////        {
        ////            blogg = Models.IoTProject.Filter(filter.Filters);
        ////        }
        ////        //if (filter.SortString != "")
        ////        //{
        ////        //    blogg = Sort(blogg, filter.SortString, filter.LastSort, filter.LastSortDirection);
        ////        //}
                
        ////    }
        ////    return blogg;
        ////}

        ////Remove filtering and sotrting from views
        //public static void ResetBlogPostz()
        //{
        //    ////ViewBlogPostz = (from n in BlogPostz select n).ToList<BlogPost>();
        //    //LastSort = "";
        //    //LastSortDirection = "desc";
        //}

        internal static IoTProject Get(string id)
        {
            var bp = from n in IoTProjects where n.Id.ToString() == id select n;
            if (bp.Count() != 0)
                return bp.First();
            else
                return null;
        }
 

        internal static IoTProject GetFirst(string filters)
        {
            var bp = ViewIoTProjects(filters);
            if (bp.Count() != 0)
                return bp.First();
            else
                return null;
        }


        internal static IoTProject PreviousFiltered(string idStr, string filterStr)
        {
            var curProj = Get(idStr);
            if (curProj != null)
            {
                var lst = ViewIoTProjects(filterStr);
                int index = lst.IndexOf(curProj);
                if (index >-1)
                {
                    if (index >0)
                    {
                        return lst[index - 1];
                    }
                }
            }
            return curProj;
        }

        internal static IoTProject NextFiltered(string idStr, string filterStr)
        {
            var curProj = Get(idStr);
            if (curProj != null)
            {
                var lst = ViewIoTProjects(filterStr);
                int index = lst.IndexOf(curProj);
                if (index > -1)
                {
                    if (index < (lst.Count()-1))
                    {
                        return lst[index + 1];
                    }
                }
            }
            return curProj;
        }


        //Clear rhe collection and therefore the view
        public static void ClearIoTProjects()
        {
            Count = 0;
            //Use Project not IoTProject type here
            var fields = typeof(Project).GetFields(
BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            var names = Array.ConvertAll(fields, field => field.Name.Substring(1).Replace(">k__BackingField", ""));

            Fields = names.ToList<string>();
            IoTProjects = new List<IoTProject>();
        }

        //public static List<Tuple<string,string>> GetSort(string SortString, ref string LastSort, ref string LastSortDirection)
        //{
        //    List<Tuple<string, string>> sortExpressions = new List<Tuple<string, string>>();
        //    if (string.IsNullOrWhiteSpace(SortString))
        //    {
        //        // If no sorting string, give a message and return.
        //        System.Diagnostics.Debug.WriteLine("Please submit in a sorting string.");
        //        return sortExpressions;
        //    }

        //    try
        //    {
        //        // Prepare the sorting string into a list of Tuples
        //        //var sortExpressions = new List<Tuple<string, string>>();
        //        string[] terms = SortString.Split(',');
        //        for (int i = 0; i < terms.Length; i++)
        //        {
        //            string[] items = terms[i].Trim().Split('~');
        //            var fieldName = items[0].Trim();
        //            var sortOrder = (items.Length > 1)
        //                      ? items[1].Trim().ToLower() : "";
        //            if ((sortOrder != "asc") && (sortOrder != "desc"))
        //            {
        //                //throw new ArgumentException("Invalid sorting order");
        //                if (LastSort == fieldName)
        //                {
        //                    if (LastSortDirection == "desc")
        //                        sortOrder = "asc";
        //                    else
        //                        sortOrder = "desc";
        //                }
        //                else
        //                    sortOrder = "asc";

        //            }
        //            LastSort = fieldName;
        //            LastSortDirection = sortOrder;
        //            sortExpressions.Add(new Tuple<string, string>(fieldName, sortOrder));
        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        var msg = "There is an error in your sorting string.  Please correct it and try again - "
        //      + e.Message;
        //        System.Diagnostics.Debug.WriteLine(msg);
        //        sortExpressions.Clear();
        //    }
        //    return sortExpressions;
        //}


        ////Sort the view on one field
        //public static List<IoTProject> Sort(List<IoTProject> viewBlogPostz, string SortString, string LastSort, string LastSortDirection)
        //{

        //    if (string.IsNullOrWhiteSpace(SortString))
        //    {
        //        // If no sorting string, give a message and return.
        //        System.Diagnostics.Debug.WriteLine("Please type in a sorting string.");
        //        return viewBlogPostz;
        //    }

        //    try
        //    {
        //        // Prepare the sorting string into a list of Tuples
        //        var sortExpressions = new List<Tuple<string, string>>();
        //        string[] terms = SortString.Split(',');
        //        for (int i = 0; i < terms.Length; i++)
        //        {
        //            string[] items = terms[i].Trim().Split('~');
        //            var fieldName = items[0].Trim();
        //            var sortOrder = (items.Length > 1)
        //                      ? items[1].Trim().ToLower() : "";
        //            if ((sortOrder != "asc") && (sortOrder != "desc"))
        //            {
        //                //throw new ArgumentException("Invalid sorting order");
        //                if (LastSort == fieldName)
        //                {
        //                    if (LastSortDirection == "desc")
        //                        sortOrder = "asc";
        //                    else
        //                        sortOrder = "desc";
        //                }
        //                else
        //                    sortOrder = "asc";

        //            }
        //            LastSort = fieldName;
        //            LastSortDirection = sortOrder;
        //            sortExpressions.Add(new Tuple<string, string>(fieldName, sortOrder));
        //        }

        //        // Apply the sorting
        //        viewBlogPostz = viewBlogPostz.MultipleSort(sortExpressions).ToList();
        //        return viewBlogPostz;
        //    }
        //    catch (Exception e)
        //    {
        //        var msg = "There is an error in your sorting string.  Please correct it and try again - "
        //      + e.Message;
        //        System.Diagnostics.Debug.WriteLine(msg);
        //        return IoTProjects;
        //    }
        //}

    //    //Filter the view on one field
    //    public static List<IoTProject> Filter(List<Tuple<string, string>> filters)
    //    {
    //        var lst = (from n in IoTProjects select n).ToList<IoTProject>();

    //        for (int index = 0; index < filters.Count; index++)
    //        {
    //            lst = FilterGen<IoTProject>(lst, filters[index].Item1, filters[index].Item2);
    //        }
    //        return lst.ToList<IoTProject>();
    //    }

    //    public static List<T> FilterGen<T>(
    //        List<T> collection,
    //        string property,
    //        string filterValue)
    //    {
    //        var filteredCollection = new List<T>();
    //        foreach (T item in collection)
    //        {
    //            // To check multiple properties use,
    //            // item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)

    //            var propertyInfo =
    //                item.GetType()
    //                    .GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
    //            if (propertyInfo == null)
    //                throw new NotSupportedException("property given does not exists");
    //            if (    //String use contatins
    //                    (propertyInfo.PropertyType.Name == "string")
    //                    ||
    //                    (propertyInfo.PropertyType.Name == "String")
    //                )
    //            {
    //                var propertyValue = (string)propertyInfo.GetValue(item, null);
    //                if (propertyValue.ToString().Contains(filterValue))
    //                    filteredCollection.Add(item);
    //            }
    //            else if ( //Id use exact
    //                        (propertyInfo.PropertyType.Name == "Int64")
    //                        ||
    //                        (propertyInfo.PropertyType.Name == "Int32")
    //                    )
    //            {
    //                int propertyValue2 = (int)propertyInfo.GetValue(item, null);
    //                if (propertyValue2.ToString() == filterValue)
    //                    filteredCollection.Add(item);
    //            }
    //            else if (propertyInfo.PropertyType.Name == "List`1")
    //            {
    //                if (property == "CodeLanguages")
    //                {
    //                    var propertyValue3 = (List<string>)propertyInfo.GetValue(item, null);
    //                    if (propertyValue3.Contains(filterValue))
    //                        filteredCollection.Add(item);
    //                }
    //                if (property == "Tags")
    //                {
    //                    var propertyValue4 = (List<string>)propertyInfo.GetValue(item, null);
    //                    if (propertyValue4.Contains(filterValue))
    //                        filteredCollection.Add(item);
    //                }

    //            }
    //        }

    //        return filteredCollection;
    //    }

    //    //    public static List<T> FilterXX<T>(List<T> list, Func<T, bool> filter)
    //    //    {
    //    //        var temp = list.AsQueryable().Where(filter);
    //    //        return temp.Cast<T>().ToList();
    //    //    }
    }


}


