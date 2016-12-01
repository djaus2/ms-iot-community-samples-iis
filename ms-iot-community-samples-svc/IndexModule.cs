namespace ms_iot_community_samples_svc
{
    using Nancy;
    using Kiwi.Markdown;
    using Kiwi.Markdown.ContentProviders;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Reflection;
    using Octokit;
    using System.Configuration;
    using System.Collections.Specialized;
    using System.Web;

    public class IndexModule : NancyModule
    {
        private const string DBSep = "---";
        /******************************************************************************************
               //Sportronics View
               return View["default"];

               //MainPage View
               errorMsg.LoggedInStatus = (bool) Request.Session["LoggedInStatus"];
               return View["/ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];

               //IndexList View
               return View["/ms_iot_Community_Samples/" + referer, Models.BlogPost.ViewBlogPostz((string)Request.Session["filter"])];

               //Index View
               string id = parameters.id;
               Models.BlogPost blogPost = Models.BlogPost.Get(id);
               if (blogPost != null)
                   return View["/ms_iot_Community_Samples/Index", blogPost];
               else
                   return View["/ms_iot_Community_Samples/IndexList", Models.BlogPost.ViewBlogPostz((string)Request.Session["filter"])];

               //Login View
               //referer is either "ms_iot_Community_Samples" or "IndexList" view
               return View["/ms_iot_Community_Samples/login", referer];

               //View MD file in /MD
               var contentProvider = new FileContentProvider(MD2, null);
               var converter = new MarkdownService(contentProvider);
               var document = converter.GetDocument(parameters.name);
               return document.Content;
       *
       ***************************************************************************/
        //Running from filesystem
        //C:\Users\david\Documents\GitHub\ms-io-community-samples-svc\ms-iot-community-samples-svc\ms-iot-community-samples-svc

        private string projectName;//= Assembly.GetCallingAssembly().GetName().Name;
        private string projectDir;// = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private string projLocn;// = @"C:/Users/" + me + @"/Documents/GitHub/ms-io-community-samples-svc/";
        private string dirr;
        private string jsonFile;
        private string jsonDirr;
        private string MDDB;
        private string MD2;
        private string MD;

        //When deployed on server
        //private const string jsonFile = @"Data.json";
        //private const string jsonDirr = @"~/Json/";
        //private const string MDDB = jsonDirr + jsonFile;
        //private const string MD2 = @"~/MD2/";
        //private const string MD = @"~/MD/";

        private void DoStrings()
        {
            string me = "David";
            string gh = HttpContext.Current.ApplicationInstance.GetType().Assembly.Location;//..GetName().Name;
            string jk = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
            projectName = Assembly.GetCallingAssembly().GetName().Name;
        projectDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        dirr = @"C:\Users\david\Documents\GitHub\ms-io-community-samples-svc\ms-iot-community-samples-svc\ms-iot-community-samples-svc\";
        //dirr = projectDir + projectName + @"\" + projectName + @"\";
        jsonFile = "Data.json";
        jsonDirr = Path.Combine ( dirr , @"Json\");
        MDDB = Path.Combine(jsonDirr , jsonFile);
        MD2 = Path.Combine(dirr , @"MD2\");
        MD = Path.Combine(dirr , @"MD\");
        }


        public IndexModule()
        {
            //async syntax
            //Get["/aa", true] = async (parameters, ct) => "Hello World!";
            
            Models.Errors errorMsg = new Models.Errors();

            //var ses = Request.Session;
            Get["/"] = _ =>
            {
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;


                return View["default"];
            };
            Get["/ms_iot_Community_Samples"] = _ =>
            {
                DoStrings();
                bool getList = false;
                if (Models.IoTProject.IoTProjects == null)
                    getList = true;
                else if (Models.IoTProject.IoTProjects.Count() == 0)
                    getList = true;
                if (getList)    
                {
                    if (!Directory.Exists(jsonDirr))
                        Directory.CreateDirectory(jsonDirr);
                    if (!File.Exists(MDDB))
                    {
                        errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                        return View["/ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
                    }
                    string[] files1 = Directory.GetFiles(jsonDirr, jsonFile);
                    if (files1.Length != 1)
                        return View["IndexList"];
                    string document = "";
                    document = File.ReadAllText(MDDB);

                    JsonSerializerSettings set = new JsonSerializerSettings();
                    set.MissingMemberHandling = MissingMemberHandling.Ignore;
                    Models.IoTProject[] md = JsonConvert.DeserializeObject<Models.IoTProject[]>(document, set);

                    var mdd = from n in md select n;
                    //Objects.BlogPost.BlogPostz = md.Select(data => data)).ToList()
                    Models.IoTProject.IoTProjects = md.ToList<Models.IoTProject>();
                    Request.Session["filter"] = "";
                }
                errorMsg.LoggedInStatus = (bool) Request.Session["LoggedInStatus"];
                return View["/ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
            };

            Get["/ms_iot_Community_Samples/default"] = _ => {
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;
                return View["default"];
            };

            Get["/ms_iot_Community_Samples/login/{referer}"] = parameters => {
                string referer = parameters.referer;
                if (referer == "0")
                    referer = "ms_iot_Community_Samples";
                else
                    referer = "IndexList";
                return View["/ms_iot_Community_Samples/login",referer];
            };

            Get["/ms_iot_Community_Samples/logout/{referer}"] = parameters => {
                string referer = parameters.referer;
                if (referer == "0")
                    referer = "ms_iot_Community_Samples";
                else
                    referer = "IndexList";
                //Models.Errors.LoggedInStatus = false;
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;
                errorMsg.Message = "Logged out.";
                errorMsg.Source = "/Logout";
                errorMsg.LoggedInStatus = false;
                if (referer == "ms_iot_Community_Samples")
                    return View["/ms_iot_Community_Samples/" + referer, errorMsg];
                else
                {
                    return View["/ms_iot_Community_Samples/" + referer, Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
                }
            };
            Get["/ms_iot_Community_Samples/onlogin/{referer}/{user}/{pwd}"] = parameters => {
                string user = parameters.user;
                string pwd = parameters.pwd;
                string referer = parameters.referer;
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;
#if ISDEPLOYED
                //http://www.dotnetprofessional.com/blog/post/2008/03/03/Encrypt-sections-of-WebConfig-or-AppConfig.aspx
                NameValueCollection secureAppSettings =
                        (NameValueCollection)ConfigurationManager.GetSection("secureAppSettings");
                string admin = (string)secureAppSettings["Admin.UserName"];
                string adminPwd = (string)secureAppSettings["Admin.Pwd"];
#else
                string admin = (string)ConfigurationManager.AppSettings["Admin.UserName"];
                string adminPwd = (string)ConfigurationManager.AppSettings["Admin.Pwd"];
#endif
                user = user.Trim();
                pwd = pwd.Trim();
                if ((user == admin) && (pwd == adminPwd))
                {
                    Request.Session["LoggedInStatus"] = true;
                    errorMsg.Message = "Logged in.";
                    errorMsg.Source = "/OnLogin";
                    errorMsg.LoggedInStatus = true;
                }
                else
                {
                    Request.Session["LoggedInStatus"] = false;
                    errorMsg.Message = "Login failed!";
                    errorMsg.Source = "/OnLogin";
                    errorMsg.LoggedInStatus = false;
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }
                if (referer== "ms_iot_Community_Samples")
                    return View["/ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
                else
                    return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };


            Get["/ms_iot_Community_Samples/convert"] = _ =>
            {
                DoStrings();
                if (!(bool)Request.Session["LoggedInStatus"])
                {
                    errorMsg.Message = "Not logged in!";
                    errorMsg.Source = "/Convert";
                    errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }

                if (!Directory.Exists(MD))
                {
                    errorMsg.Message = "No files to convert./r/nRun 'Sync with GitHub' first";
                    errorMsg.Source = "/Convert";
                    errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }

                if ((Directory.GetFiles(MD, "*.MD")).Length==0)
                {
                    errorMsg.Message = "No files to convert./r/nRun  'Sync with GitHub' first";
                    errorMsg.Source = "/Convert";
                    errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }

                if (!Directory.Exists(jsonDirr))
                    Directory.CreateDirectory(jsonDirr);
                string[] files0= Directory.GetFiles(jsonDirr, "*.*");
                foreach (string file in files0)
                {
                    File.Delete(file);
                }

                if (!Directory.Exists(MD2))
                    Directory.CreateDirectory(MD2);
                string[] files1 = Directory.GetFiles(MD2, "*.*");
                foreach (string file in files1)
                {
                    File.Delete(file);
                }

                char[] lineSep = new char[] { '\r', '\n' };
                string[] files = Directory.GetFiles(MD, "*.MD");
                //File.AppendAllText(MDDB, "[\r\n");

                int count = files.Length;
                bool abortFile = false;
                Models.IoTProject.ClearIoTProjects();
                foreach (string file in files)
                {
                    abortFile = false;
                    try {
                        string filename = Path.GetFileNameWithoutExtension(file);
                        count--;
                        string fileTxt = File.ReadAllText(file);

                        //Get database between 1st and second lines of ---
                        int startIndex = fileTxt.IndexOf(DBSep,0);
                        if (startIndex < 0)
                            continue;

                        int endIndex = fileTxt.IndexOf(DBSep, startIndex + DBSep.Length);
                        if (endIndex < 0)
                            continue;

                        string DB2 = fileTxt.Substring(startIndex, endIndex - startIndex + DBSep.Length) + "\r\n";
                        string DB = fileTxt.Substring(startIndex + DBSep.Length, endIndex - startIndex - DBSep.Length).Trim();
                        fileTxt = fileTxt.Substring(endIndex+ DBSep.Length );
                        string[] lines = DB.Split(lineSep);
                        Models.IoTProject blogpost = new Models.IoTProject();
                        blogpost.filename = filename;
                        foreach (string line in lines)
                        {
                            string newLine = line.Trim();
                            if (newLine != "")
                            {
                                string[] parts = newLine.Split(new char[] { ':' });
                                string vname = parts[0].Trim();
                                string vvalue = parts[1].Trim();
                                try
                                {
                                    var fields = typeof(Models.IoTProject).GetFields(
                                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                    var field = from n in fields where n.Name.Substring(1).Replace(">k__BackingField", "") == vname select n;
                                    if (field.Count()==1)
                                    {
                                        field.First().SetValue(blogpost, vvalue);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //Abort lines loop
                                    abortFile = true;
                                    break;
                                }
                            }
                        }
                        if (abortFile)
                        {
                            //Abort this db record
                            break;
                        }
                        string name = Path.GetFileName(file);
                        File.WriteAllText(Path.Combine(MD2 , name), fileTxt);
                    }
                    catch (Exception ex)
                    {
                        //Skip this file and continue with next
                        continue;
                    }
                }
                Request.Session["filter"] = "";
                string json = JsonConvert.SerializeObject(Models.IoTProject.IoTProjects);
                
                File.AppendAllText(MDDB, json);
                Request.Session["filter"] = "";
                return View["/ms_iot_Community_Samples/IndexList",Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/display/{name}"] = parameters =>
            {
                var contentProvider = new FileContentProvider(MD2, null);
                var converter = new MarkdownService(contentProvider);
                var document = converter.GetDocument(parameters.name);
                return document.Content;
            };
           
  
            Get["/ms_iot_Community_Samples/GitHub",true] = async (parameters, ct) =>
            {
                if (!(bool)Request.Session["LoggedInStatus"])
                {
                    errorMsg.Message = "Not logged in!";
                    errorMsg.Source = "/GitHub";
                    return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                }
                DoStrings();
                if (!Directory.Exists(MD))
                    Directory.CreateDirectory(MD);
                string[] files0 = Directory.GetFiles(MD, "*.*");
                foreach (string file in files0)
                {
                    File.Delete(file);
                }

#if ISDEPLOYED
                NameValueCollection secureAppSettings =
                        (NameValueCollection)ConfigurationManager.GetSection("secureAppSettings");
                string githuUrl =  (string)secureAppSettings["GitHub.Url"];
                string githubRepo = (string)secureAppSettings["GitHub.Repository"];
                string githubUsr = (string)secureAppSettings["GitHub.UserName"];
                string githubPwd = (string)secureAppSettings["GitHub.Pwd"];
#else
                string githuUrl = (string)ConfigurationManager.AppSettings["GitHub.Url"];
                string githubRepo = (string)ConfigurationManager.AppSettings["GitHub.MDsRepository"];
                string githubUsr = (string)ConfigurationManager.AppSettings["GitHub.UserName"];
                string githubPwd = (string)ConfigurationManager.AppSettings["GitHub.Pwd"];
#endif
                //http://haacked.com/archive/2014/04/24/octokit-oauth/
                string clientId = "2c0baac7c20dd4fb52b5";
        string clientSecret = "f14d3e9055a292128abe472ab0b000a2a8c87166";//f14d3e9055a292128abe472ab0b000a2a8c87166
                                                                         //readonly
                GitHubClient client3 =
            new GitHubClient(new ProductHeaderValue(githubRepo), new Uri(githuUrl));
        //https://github.com/octokit/octokit.net
        var github = new GitHubClient(new ProductHeaderValue(githubRepo));
                var user = await github.User.Get(githubUsr);
                var client = new GitHubClient(new ProductHeaderValue(githubRepo));
                var basicAuth = new Credentials(githubUsr, githubPwd); // NOTE: not real credentials
                client3.Credentials = basicAuth;

                //var client = new GitHubClient(new ProductHeaderValue("dotnet-test-functional"));
                //client.Credentials = GithubHelper.Credentials;
                //http://stackoverflow.com/questions/24830617/reading-code-from-repository
                var repos = await client3.Repository.GetAllForCurrent();
                var repo = from n in repos where n.Name== githubRepo select n;
                int count = 0;
                if (repo.Count() == 1)
                {
                    var theRepo = repo.First();
                    var AllContent = await client.Repository.Content.GetAllContents(theRepo.Id);//.GetAllContent(repos[0].Owner.Login, repos[0].Name);
                    foreach (var file in AllContent)
                    {
                        string textOfFirstFileName = file.Name;
                        var content = await client.Repository.Content.GetAllContents(theRepo.Id, textOfFirstFileName);
                        if (content.Count() != 0)
                        {
                            var fileContent = content[0].Content; 
                            File.AppendAllText(Path.Combine(MD , file.Name), fileContent);
                        }
                    }

                    count = (Directory.GetFiles(MD)).Length;
                    
                    //var textOfFirstFile = AllContent[0].Content;
                    //var textOfFirstFileName = AllContent[0].Name;
                    //var AllContent2 = await client.Repository.Content.GetAllContents(repo.First().Id, textOfFirstFileName);
                    //var textOfFirstFile2 = AllContent[1].Content;
                    //var textOfFirstFile2Name = AllContent[1].Name;
                    //var AllContent3 = await client.Repository.Content.GetAllContents(repo.First().Id, textOfFirstFile2Name);
                }

                //var pull = await client3.PullRequest.GetAllForRepository("djaus2", "ms-iot-community-samples");

                //var client2 = new GitHubClient(new ProductHeaderValue("ms-iot-community-samples"));
                //var tokenAuth = new Credentials("token"); // NOTE: not real token
                //client2.Credentials = tokenAuth;

                //Request.Session["LoggedInStatus"] = false;
                Request.Session["filter"] = "";
                errorMsg.LoggedInStatus = (bool) Request.Session["LoggedInStatus"];
                if (count !=0)
                    errorMsg.Message = "Retrieved " + count.ToString() + ".MD files from GitHub\r\nNow run 'Convert' to process the downloaded files.";
                else
                    errorMsg.Message = "No .MD files retreived from GitHub";
                errorMsg.Source = "/GitHub";
                return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
            };
           

            Get["/ms_iot_Community_Samples/Sort/{field}"] = parameters =>
            {
                string sortString = parameters.field;
                Models.FilterAndSortInfo fi;
                if ((string)Request.Session["filter"] == null)
                {
                    fi = new Models.FilterAndSortInfo();
                }
                else if ((string)Request.Session["filter"] == "")
                {
                    fi = new Models.FilterAndSortInfo();
                }
                else
                {
                    JsonSerializerSettings set = new JsonSerializerSettings();
                    set.MissingMemberHandling = MissingMemberHandling.Ignore;
                    fi = JsonConvert.DeserializeObject<Models.FilterAndSortInfo>((string)Request.Session["filter"], set);
                }
                //For filter remove any pre-existing sorting (but for sort don't remove pre-existing sorting).
                string lastSort = fi.LastSort;
                string lastSortDirection = fi.LastSortDirection;
                fi.SortExpressions = Utilities.LinqDynamicMultiSortingUtility.GetSort(sortString, ref lastSort, ref lastSortDirection);
                fi.LastSort = lastSort;
                fi.LastSortDirection = lastSortDirection;


                string json = JsonConvert.SerializeObject(fi);
                Request.Session["filter"] = json;
                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/ClearSort"] = _ =>
            {
                Request.Session["filter"] = "";
                //if ((string)Request.Session["filter"] != null)
                //    if ((string)Request.Session["filter"] == "")
                //    {
                //        ((Models.FilterAndSortInfo)Request.Session["filter"]).SortExpressions = new List<Tuple<string, string>>();
                //    }
                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            
            Get["/ms_iot_Community_Samples/Show/{id}"] = parameters =>
            {
                string id = parameters.id;
                Models.IoTProject blogPost = Models.IoTProject.Get(id);
                if (blogPost != null)
                    return View["/ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/reset"] = _ =>
            {
                Request.Session["filter"] = "";
                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/clear"] = _ =>
            {
                Request.Session["filter"] = "";
                Models.IoTProject.ClearIoTProjects();
                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/list"] = _ =>
            {
                string filter = (string)Request.Session["filter"];
                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/ClearFilter"] = _ =>
            {
                //Same as reset
                Request.Session["filter"] = "";
                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/Filter/{idfilter}/{titlefilter}/{summaryfilter}/{codefilter}/{tagsfilter}/{tagsfilter2}"] = parameters =>
            {
                char[] sep = new char[] { '~' };
                string[] tupl;
                var filters = new List<Tuple<string, string>>();
                
                string filter = parameters.idfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                            if (tupl[1] != "")
                                filters.Add(new Tuple<string, string>(tupl[0],tupl[1]));                               
                }
                filter = parameters.titlefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.summaryfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.codefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("Y", "/");
                filter = filter.Trim();
                if (filter != "")                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.tagsfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("Y", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                filter = parameters.tagsfilter2;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                }
                Models.FilterAndSortInfo fi;
                if ((string)Request.Session["filter"] == null)
                {
                    fi = new Models.FilterAndSortInfo();
                }
                else if ((string)Request.Session["filter"] == "")
                {
                    fi = new Models.FilterAndSortInfo();
                }
                else
                {
                    JsonSerializerSettings set = new JsonSerializerSettings();
                    set.MissingMemberHandling = MissingMemberHandling.Ignore;
                    fi = JsonConvert.DeserializeObject<Models.FilterAndSortInfo>((string)Request.Session["filter"], set);
                }
                //For filter remove any pre-existing sorting (but for sort don't remove pre-existing sorting)
                fi.Filters = filters;
                fi.SortExpressions = new List<Tuple<string, string>>();
                string json = JsonConvert.SerializeObject(fi);
                Request.Session["filter"] = json;


                return View["/ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

        }

    }
}