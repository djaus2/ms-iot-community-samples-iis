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
    using System.Web.Security;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;


    public partial class IndexModule : NancyModule
    {

        private    List<string> IgnoreMDS = new  List <string> {"readme", "template" };
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
        private string jsonFile = "Data.json";
        private const string srcDirName = "MD";//.MD files from GitHub are synced with this folder
        private const string destDirName = "MD2";//Files from MD are stripped of "db" at top (used to create json db) and placed here.
        private const string jsonDirName = "Json";
        private const string MyGitHubRepositories = @"C:\Users\David\Documents\GitHub";
      
        private string siteName;
        
        //Folder paths
        private string SiteDir;
        private string MD2;
        private string MD;
        private string JsonDir;
        private string ContentDir;
        private string ContentCommunityDir;

        //The json file path
        private string MDDB;

        private void DoStrings() //(IRootPathProvider pathProvider)
        {
            siteName = Assembly.GetCallingAssembly().GetName().Name;

#if !DEBUG
            string appRoot = Environment.GetEnvironmentVariable("Home");
            string Home = Path.Combine(appRoot,@"\Site\wwwroot");
            Home = @"D:\Home\Site\wwwroot";
#else
            string Home = @"C:\Users\David\Documents\GitHub\ms-io-community-samples-svc\ms-iot-community-samples-svc\ms-iot-community-samples-svc";
#endif

            MDDB = Home + @"\json\Data.json";
            JsonDir = Home + @"\json";
            MD2 = Home + @"\MD2";
            MD = Home + @"\MD";
            ContentDir = Home + @"\Content";
            ContentCommunityDir = Home + @"\Content\ms_iot_Community_Samples";
        }


        public IndexModule()//(IRootPathProvider pathProvider)
        {
           //IRootPathProvider pathProvider = null;
            //async syntax
            //Get["/aa", true] = async (parameters, ct) => "Hello World!";

            Models.Errors errorMsg = new Models.Errors();

            Get["ms_iot_Community_Samples/MDFileTemplate"] = _ =>
            {
                return View["ms_iot_Community_Samples/MDTemplate"];
            };
            Get["/Template"] = _ =>
            {
                return View["Template"];
            };
            Get["/12"] = _ =>
            {
                return View["onetwo"];
            };

            Get["/Dirs"] = _ =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                //appRoot = Path.Combine(appRoot, "Site\\wwwroot");
                string [] dirs = Directory.GetDirectories(appRoot);
                List<string> directories = dirs.ToList < string>();
                return View["AList", directories];
            };
            Get["/Dirs/{SubDir1}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1);
                string[] dirs = Directory.GetDirectories(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Dirs/{SubDir1}/{SubDir2}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "/" + parameters.SubDir2);
                string[] dirs = Directory.GetDirectories(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Dirs/{SubDir1}/{SubDir2}/{SubDir3}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "/" + parameters.SubDir2 + "/" + parameters.SubDir3);
                string[] dirs = Directory.GetDirectories(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Dirs/{SubDir1}/{SubDir2}/{SubDir3}/{SubDir4}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "/" + parameters.SubDir2 + "/" + parameters.SubDir3 + "/" + parameters.SubDir4);
                string[] dirs = Directory.GetDirectories(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Dirs/{SubDir1}/{SubDir2}/{SubDir3}/{SubDir4}/{SubDir5}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "/" + parameters.SubDir2 + "/" + parameters.SubDir3 + "/" + parameters.SubDir4 + "/" + parameters.SubDir5);
                string[] dirs = Directory.GetDirectories(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Files/{SubDir1}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1);
                string[] dirs = Directory.GetFiles(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Files/{SubDir1}/{SubDir2}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "\\" + parameters.SubDir2);
                string[] dirs = Directory.GetFiles(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Files/{SubDir1}/{SubDir2}/{SubDir3}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "\\" + parameters.SubDir2 + "\\" + parameters.SubDir3);
                string[] dirs = Directory.GetFiles(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Files/{SubDir1}/{SubDir2}/{SubDir3}/{SubDir4}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "\\" + parameters.SubDir2 + "\\" + parameters.SubDir3 + "\\" + parameters.SubDir4);
                string[] dirs = Directory.GetFiles(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/Files/{SubDir1}/{SubDir2}/{SubDir3}/{SubDir4}/{SubDir5}"] = parameters =>
            {
                string appRoot = Environment.GetEnvironmentVariable("Home");
                appRoot = Path.Combine(appRoot, parameters.SubDir1 + "\\" + parameters.SubDir2 + "\\" + parameters.SubDir3 + "\\" + parameters.SubDir4 + "\\" + parameters.SubDir5);
                string[] dirs = Directory.GetFiles(appRoot);
                List<string> directories = dirs.ToList<string>();
                return View["AList", directories];
            };
            Get["/ms_iot_Community_Samples/Test2"] = _ =>
            {
                return View["Test2"];
            };
            Get["/ms_iot_Community_Samples/Test1"] = _ =>
            {
                return View["ms_iot_Community_Samples/Test1"];
            };
            Get["/"] = _ =>
            {
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;


                return View["default"];
            };
            Get["ms_iot_Community_Samples/About"] = _ =>
            {
                return View["ms_iot_Community_Samples/About"];
            };
            Get["/Default"] = _ =>
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
                var cnt = 0;
                if (getList)
                {
                    if (!Directory.Exists(JsonDir))
                    {
                        Directory.CreateDirectory(JsonDir);
                    }
                    if (!Directory.Exists(MD))
                    {
                        Directory.CreateDirectory(MD);
                    }
                    if (!Directory.Exists(MD2))
                    {
                        Directory.CreateDirectory(MD2);
                    }

                    if (!File.Exists(MDDB))
                    {
                        errorMsg.Message = "No Json File.Do Github and Convert first.<br/>Admin moed required.";
                        errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                        string emptyJsonArray = "[]";
                        File.WriteAllText(MDDB, emptyJsonArray);
                        Models.IoTProject.IoTProjects = new List<Models.IoTProject>();

                        Request.Session["filter"] = "";
                        return View["ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
                    }
                    
                    string[] files1 = Directory.GetFiles(JsonDir, jsonFile);
                    if (files1.Length != 1)
                    {
                        
                        return View["IndexList"];
                    }
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
                cnt = Models.IoTProject.IoTProjects.Count();
                errorMsg.Message = "Loaded " + cnt.ToString() + " project/s";
                errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                //return View["ms_iot_Community_Samples/Test1"];
                return View["ms_iot_Community_Samples/ms_iot_Community_Samples", errorMsg];
            };

            Get["/ms_iot_Community_Samples/default"] = _ =>
            {
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;
                return View["default"];
            };

            Get["/ms_iot_Community_Samples/Admin"] = _ =>
            {
                errorMsg.Message = "Logging in";
                errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                errorMsg.Source = "/ms_iot_Community_Samples/Admin";
                return View["ms_iot_Community_Samples/Admin", errorMsg];
            };

            Get["/ms_iot_Community_Samples/login/{referer}"] = parameters =>
            {
                string referer = parameters.referer;
                if (referer == "0")
                    referer = "ms_iot_Community_Samples_Admin";
                else
                    referer = "IndexList";
                return View["ms_iot_Community_Samples/login", referer];
            };
            Get["/ms_iot_Community_Samples/login2/{referer}"] = parameters =>
            {
                string referer = parameters.referer;
                if (referer == "0")
                    referer = "ms_iot_Community_Samples_Admin";
                else
                    referer = "IndexList";
                return View["ms_iot_Community_Samples/login2", referer];
            };

            Get["/ms_iot_Community_Samples/logout/{referer}"] = parameters =>
            {
                string referer = parameters.referer;
                if (referer == "0")
                    referer = "ms_iot_Community_Samples_Admin";
                else
                    referer = "IndexList";
                //Models.Errors.LoggedInStatus = false;
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;
                errorMsg.Message = "Logged out.";
                errorMsg.Source = "/Logout";
                errorMsg.LoggedInStatus = false;
                if (referer == "ms_iot_Community_Samples_Admin")
                    return View["ms_iot_Community_Samples/" + "ms_iot_Community_Samples", errorMsg];
                else
                {
                    return View["ms_iot_Community_Samples/" + referer, Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
                }
            };
            Get["/ms_iot_Community_Samples/onlogin/{referer}/{user}/{pwd}"] = parameters =>
            {
                string user = parameters.user;
                string pwd = parameters.pwd;
                string referer = parameters.referer;
                Request.Session["filter"] = "";
                Request.Session["LoggedInStatus"] = false;

                string admin = (string)ConfigurationManager.AppSettings["Admin.UserName"];
                string adminPwd = (string)ConfigurationManager.AppSettings["Admin.Pwd"];

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
                    return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                }
                if (referer == "ms_iot_Community_Samples_Admin")
                    return View["ms_iot_Community_Samples/Admin" , errorMsg];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };


            Get["/ms_iot_Community_Samples/convert"] = _ =>
            {
                DoStrings();
                if (!(bool)Request.Session["LoggedInStatus"])
                {
                    errorMsg.Message = "Not logged in!";
                    errorMsg.Source = "/Convert";
                    errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                    return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                }

                if (!Directory.Exists(MD))
                {
                    errorMsg.Message = "No files to convert...No MD Dir/r/nRun 'Sync with GitHub' first";
                    errorMsg.Source = "/Convert";
                    errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                    return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                }

                if ((Directory.GetFiles(MD, "*.MD")).Length == 0)
                {
                    errorMsg.Message = "No files to convert... No files in MD dir./r/nRun  'Sync with GitHub' first";
                    errorMsg.Source = "/Convert";
                    errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                    return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                }

                if (!Directory.Exists(JsonDir))
                    Directory.CreateDirectory(JsonDir);
                string[] files0 = Directory.GetFiles(JsonDir, "*.*");
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
                
                int count = files.Length;
                
                bool abortFile = false;
                Models.IoTProject.ClearIoTProjects();
                foreach (string file in files)
                {

                    abortFile = false;
                    Models.IoTProject iotProject=null;
                    try
                    {
                        string filename = Path.GetFileNameWithoutExtension(file);
                        if (IgnoreMDS.Contains(filename.ToLower()))
                        {
                            continue;
                        }
                        count--;
                        string fileTxt = File.ReadAllText(file);

                        //Get database between 1st and second lines of ---
                        int startIndex = fileTxt.IndexOf(DBSep, 0);
                        if (startIndex < 0)
                            continue;

                        int endIndex = fileTxt.IndexOf(DBSep, startIndex + DBSep.Length);
                        if (endIndex < 0)
                            continue;

                        string DB2 = fileTxt.Substring(startIndex, endIndex - startIndex + DBSep.Length) + "\r\n";
                        string DB = fileTxt.Substring(startIndex + DBSep.Length, endIndex - startIndex - DBSep.Length).Trim();
                        fileTxt = fileTxt.Substring(endIndex + DBSep.Length);
                        
                        string[] lines = DB.Split(lineSep);
                        iotProject = new Models.IoTProject();
                        iotProject.Filename = filename;

                        foreach (string line in lines)
                        {
                            string newLine = line.Trim();
                            if (newLine != "")
                            {
                                string vname = "";
                                string vvalue = "";
                                string[] parts = newLine.Split(new char[] { ':' });
                                if (parts.Length == 1)
                                {
                                    vname = parts[0].Trim();
                                    if (vname == "")
                                        continue;
                                    else if ((vname == "GitHub") || (vname == "HacksterIO") || (vname == "Codeplex") ||(vname== "Blog"))
                                        continue;
                                    vvalue = "";
                                }
                                else if (parts.Length == 2)
                                {
                                    vname = parts[0].Trim();
                                    if (vname == "")
                                        continue;
                                    else if ((vname == "GitHub") || (vname == "HacksterIO") || (vname == "Codeplex") || (vname=="Blog"))
                                        continue;
                                    vvalue = parts[1].Trim();
                                }
                                else
                                {
                                    //If value part in line had extra : as in Urls then get more thean 2 parts
                                    //In that case merge all parts above index 0.
                                    //Actually just remove parts[0] and : at start of line.
                                    vname = parts[0].Trim();
                                    vvalue = newLine.Substring(vname.Length+1).Trim();
                                    if (vname == "")
                                        continue;
                                    else if (vname == "GitHub")
                                    {
                                        if (vvalue.ToLower().IndexOf("https://github.com/") == 0)
                                        {
                                            vvalue = vvalue.Substring("https://github.com/".Length);
                                        }
                                        else
                                            continue;
                                    }
                                    else if (vname == "HacksterIO")
                                    {
                                        if (vvalue.ToLower().IndexOf("https://www.hackster.io/") == 0)
                                        {
                                            vvalue = vvalue.Substring("https://www.hackster.io/".Length);
                                        }
                                        else
                                            continue;
                                    }

                                }
                                if ((vname=="") || (vvalue==""))
                                {
                                    continue;
                                }
                                if (vname == "Codeplex")
                                {
                                    //^(https\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$
                                    if (vvalue.IndexOf("https://") == 0)
                                    {
                                        int codp = vvalue.ToLower().IndexOf(".codeplex.com") + ".codeplex.com".Length;
                                        if (vvalue.Length != codp)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                        continue;
                                }
                                try
                                {
                                    //Note must useModels.Project here not Models.IoTProject
                                    var fields = typeof(Models.Project).GetFields(
                                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                    var field = from n in fields where n.Name.Substring(1).Replace(">k__BackingField", "").ToLower() == vname.ToLower() select n;
                                    if (field.Count() == 1)
                                    {
                                        var fld = field.First();
                                        string fldName = fld.Name.Substring(1).Replace(">k__BackingField", "");
                                        if (fld.FieldType.Name == "List`1")
                                        {
                                            string[] aos = vvalue.Split(new char[] { ',' });
                                            var trimmedAos = from n in aos select n.Trim();
                                            List < string > los = trimmedAos.ToList<string>();
                                            fld.SetValue(iotProject, los);
                                            //if (fldName == "CodeLanguages")
                                            //{

                                            //}
                                            //else if (fld.Name == "Tags")
                                            //{

                                            //}
                                            //else if (fld.Name == "Authors")
                                            //{

                                            //}

                                        }
                                        else
                                            fld.SetValue(iotProject, vvalue);
                                    }
                                    else
                                    {
                                        
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
                            Models.IoTProject.IoTProjects.Remove(iotProject);
                            //Abort this db record
                            break;
                        }
                        bool gotFile = false;
                        //Tests for an empty project
                        if (iotProject.Title != "")
                        if ( (iotProject.HacksterIO+iotProject.GitHub + iotProject.Codeplex).Trim() != "")
                        {
                            string name = Path.GetFileName(file);
                            File.WriteAllText(Path.Combine(MD2, name), fileTxt);
                            gotFile = true;
                        }
                        if (!gotFile)
                        {
                            Models.IoTProject.IoTProjects.Remove(iotProject);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (iotProject!=null)
                            Models.IoTProject.IoTProjects.Remove(iotProject);
                        //Skip this file and continue with next
                        continue;
                    }
                }

                Request.Session["filter"] = "";
                string json = JsonConvert.SerializeObject(Models.IoTProject.IoTProjects);
                File.AppendAllText(MDDB, json);
                Request.Session["filter"] = "";
                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            //Render .md project files in browser
            Get["/ms_iot_Community_Samples/display/{name}"] = parameters =>
            {
                DoStrings();
                var contentProvider = new FileContentProvider(MD2, null);
                var converter = new MarkdownService(contentProvider);
                var document = converter.GetDocument(parameters.name);
                return document.Content;
            };
            Get["/ms_iot_Community_Samples/display/{dir}/{name}"] = parameters =>
            {
                DoStrings();
                var contentProvider = new FileContentProvider(MD2, null);
                var converter = new MarkdownService(MD2 + @"\" + parameters.dir);
                var document = converter.GetDocument(parameters.name);
                return document.Content;
            };
            //Render about information from (from .MD file) on ms_iot_Community_Samples.cshtml
            Get["/Site_Content/{dir}/{name}"] = parameters =>
            {
                DoStrings();
                var contentProvider = new FileContentProvider(ContentDir + @"\" + parameters.dir, null);
                var converter = new MarkdownService(contentProvider);
                var document = converter.GetDocument(parameters.name);
                return document.Content;
            };
            Get["/Site_Content/{name}"] = parameters =>
            {
                DoStrings();
                var contentProvider = new FileContentProvider(ContentDir, null);
                var converter = new MarkdownService(contentProvider);
                var document = converter.GetDocument(parameters.name);
                return document.Content;
            };


            Get["/ms_iot_Community_Samples/GitHub/{Mode}", true] = async (parameters, ct) =>
             {
                 
                 if (!(bool)Request.Session["LoggedInStatus"])
                 {
                     errorMsg.Message = "Not logged in!";
                     errorMsg.Source = "/GitHub";
                     return View["/ms_iot_Community_Samples/ErrorPage", errorMsg];
                 }

                 string mode = parameters.Mode;
                 if (mode=="1")
                 {
                    //Reget repository
                    ConfigurationManager.AppSettings["GitHub.LatestCommitSha"] = "";
                 }
                 DoStrings();



                 string githuUrl = (string)ConfigurationManager.AppSettings["GitHub.Url"];
                 string githubRepo = (string)ConfigurationManager.AppSettings["GitHub.MDsRepository"];
                 string githubUsr = (string)ConfigurationManager.AppSettings["GitHub.UserName"];
                 string githubPwd = (string)ConfigurationManager.AppSettings["GitHub.Pwd"];
                 string githubLatestCommitSha = (string)ConfigurationManager.AppSettings["GitHub.LatestCommitSha"];
                 ClientId = (string)ConfigurationManager.AppSettings["GitHub.ClientId"];
                 ClientSecret = (string)ConfigurationManager.AppSettings["GitHub.ClientSecret"];

                 //string githubLatestCommitCountStr = (string)ConfigurationManager.AppSettings["GitHub.LatestCommitCountStrStr"];
                 //int githubLatestCommitCount = 0;
                 //bool res = int.TryParse(githubLatestCommitCountStr, out githubLatestCommitCount);
                 //if (!res)
                 //{
                 //    githubLatestCommitCount = 0;
                 //    ConfigurationManager.AppSettings["GitHub.LatestCommitCountStrStr"] = "0";
                 //}

                 if (githubLatestCommitSha == null)
                     githubLatestCommitSha = "";
                 //http://haacked.com/archive/2014/04/24/octokit-oauth/
                 //string clientId = "2c0baac7c20dd4fb52b5";
                 // string clientSecret = "f14d3e9055a292128abe472ab0b000a2a8c87166";//f14d3e9055a292128abe472ab0b000a2a8c87166
                 //readonly
                 //GitHubClient clientg =
                 ////////clientg =
                 ////////        new GitHubClient(new ProductHeaderValue(githubRepo), new Uri(githuUrl));
                 ////////var accessToken = Session["OAuthToken"] as string;
                 ////////if (accessToken != null)
                 ////////{
                 ////////   // This allows the client to make requests to the GitHub API on the user's behalf
                 ////////   // without ever having the user's OAuth credentials.
                 ////////   clientg.Credentials = new Credentials(accessToken);
                 ////////}
                 ////////try
                 ////////{
                 ////////   // The following requests retrieves all of the user's repositories and
                 ////////   // requires that the user be logged in to work.
                 ////////   var repositories = await clientg.Repository.GetAllForCurrent();
                 ////////    var repo2 = from n in repositories where n.Name == githubRepo select n;

                 ////////    if (repo2.Count() == 1)
                 ////////    {
                 ////////        errorMsg.Message = "Got repo2";
                 ////////        errorMsg.Source = "GitHub";
                 ////////        return View["ErrorPage", errorMsg];
                 ////////    }
                 ////////}
                 ////////catch (AuthorizationException)
                 ////////{
                 ////////   // Either the accessToken is null or it's invalid. This redirects
                 ////////   // to the GitHub OAuth login page. That page will redirect back to the
                 ////////   // Authorize action.
                 ////////   //return Redirect(GetOauthLoginUrl());
                 ////////    string urll = GetOauthLoginUrl();
                 ////////    await Authorize(urll, "");
                 ////////    errorMsg.Message = "AuthorizationException after alled GetOauthLoginUrl";
                 ////////    errorMsg.Source = "GitHub";
                 ////////    return View["ErrorPage", errorMsg];
                 ////////}

                 
                 basicGitHubClient =
                        new GitHubClient(new ProductHeaderValue(githubRepo), new Uri(githuUrl));
                
                 //https://github.com/octokit/octokit.net
                 //GitHubClient github = new GitHubClient(new ProductHeaderValue(githubRepo));
                 var user = await basicGitHubClient.User.Get(githubUsr);
                 
                 //var client = new GitHubClient(new ProductHeaderValue(githubRepo));
                 var basicAuth = new Credentials(githubUsr, githubPwd); // NOTE: not real credentials
                 basicGitHubClient.Credentials = basicAuth;

                //var client = new GitHubClient(new ProductHeaderValue("dotnet-test-functional"));
                //client.Credentials = GithubHelper.Credentials;
                //http://stackoverflow.com/questions/24830617/reading-code-from-repository
                var repos = await basicGitHubClient.Repository.GetAllForCurrent();
                 var repo = from n in repos where n.Name == githubRepo select n;
                 int count = 0;
                 if (repo.Count() == 1)
                 {
                     var theRepo = repo.First();
                     var commits = await basicGitHubClient.Repository.Commit.GetAll(theRepo.Id);
                     if (commits.Count() != 0)
                     {
                         var latest = commits.OrderByDescending(t => t.Commit.Author.Date);

                         //group n by n.Commit.Author.Date into g
                         //select n OrderByDescending(t => t.Commit.Author.Date);
                         //var lat = latest.First();
                         //string latestCommit = commits.First().Sha;
                         //int commitCount = commits.Count();
                         //var com = await basicGitHubClient.Repository.Commit.Get(theRepo.Id, commits.First().Sha);
                         //if (commitCount > githubLatestCommitCount)
                         //{
                         //if (commitCount > githubLatestCommitCount)
                         if (latest.Count() != 0)
                         {
                             var last = latest.First();
                             if (last.Sha != githubLatestCommitSha)
                             {

                                 if (!Directory.Exists(MD))
                                     Directory.CreateDirectory(MD);
                                 string[] files0 = Directory.GetFiles(MD, "*.*");
                                 foreach (string file in files0)
                                 {
                                     File.Delete(file);
                                 }
                                 var AllContent = await basicGitHubClient.Repository.Content.GetAllContents(theRepo.Id);//.GetAllContent(repos[0].Owner.Login, repos[0].Name);
                                 int cntr = 0;
                                 foreach (var file in AllContent)
                                 {
                                     //if (++cntr > 3)
                                     //    break;
                                     string textOfFirstFileName = file.Name;
                                     if (IgnoreMDS.Contains(textOfFirstFileName.ToLower().Replace(".md", "")))
                                     {
                                         if (textOfFirstFileName.ToLower() == "readme.md")
                                         {
                                             var contentRM = await basicGitHubClient.Repository.Content.GetAllContents(theRepo.Id, textOfFirstFileName);
                                             if (contentRM.Count() != 0)
                                             {
                                                 if (!Directory.Exists(ContentCommunityDir))
                                                     Directory.CreateDirectory(ContentCommunityDir);
                                                 string[] ReadMeMDs = Directory.GetFiles(ContentCommunityDir, "ReadMe.MD");
                                                 foreach (string ReadMeMD in ReadMeMDs)
                                                 {
                                                     File.Delete(ReadMeMD);
                                                 }
                                                 var fileContent = contentRM[0].Content;
                                                 File.AppendAllText(Path.Combine(ContentCommunityDir, textOfFirstFileName), fileContent);
                                             }
                                         }
                                         continue;
                                     }
                                     var content = await basicGitHubClient.Repository.Content.GetAllContents(theRepo.Id, textOfFirstFileName);
                                     if (content.Count() != 0)
                                     {
                                         var fileContent = content[0].Content;
                                         File.AppendAllText(Path.Combine(MD, file.Name), fileContent);
                                     }
                                 }
                                 ConfigurationManager.AppSettings["GitHub.LatestCommitSha"] = last.Sha;
                                 count = (Directory.GetFiles(MD)).Length;
                             }
                             else
                                 count = -1;
                        
                         }
                     }
                }


                 Request.Session["filter"] = "";
                 errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                 if (count ==-1)
                     errorMsg.Message = "No .MD files retreived from GitHub as local copy is in Sync.";
                 else if (count != 0)
                     errorMsg.Message = "Retrieved " + count.ToString() + ".MD files from GitHub\r\nNow run 'Convert' on Admin page to process the downloaded files.";
                 else
                     errorMsg.Message = "No .MD files retreived from GitHub. Either the repository had no .MD files, or there was an error.";
                 errorMsg.Source = "/GitHub";
                 return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
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
                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/ClearSort"] = _ =>
            {
                Request.Session["filter"] = "";
                //if ((string)Request.Session["filter"] != null)
                //    if ((string)Request.Session["filter"] == "")
                //    {
                //        ((Models.FilterAndSortInfo)Request.Session["filter"]).SortExpressions = new List<Tuple<string, string>>();
                //    }
                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };


            Get["/ms_iot_Community_Samples/Show/{id}"] = parameters =>
            {
                string idStr = parameters.id;
                Models.IoTProject blogPost = Models.IoTProject.Get(idStr);
                if (blogPost != null)
                    return View["ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/Browse"] = _ =>
            {
                string filter = (string)Request.Session["filter"];
                Models.IoTProject blogPost = Models.IoTProject.GetFirst(filter);
                if (blogPost != null)
                    return View["ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            //Get["/ms_iot_Community_Samples/Previous/{id}"] = parameters =>
            //{
            //    string idStr = parameters.id;
            //    int id;
            //    bool res = int.TryParse(idStr, out id);
            //    if (res)
            //    {
            //        id--;
            //        if (id > -1)
            //        {
            //            idStr = id.ToString();
            //            Models.IoTProject blogPost = Models.IoTProject.Get(idStr);
            //            if (blogPost != null)
            //                return View["ms_iot_Community_Samples/Index", blogPost];
            //        }
            //    }

            //    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            //};




            //Get["/ms_iot_Community_Samples/Next/{id}"] = parameters =>
            //{
            //    string filter = (string)Request.Session["filter"];
            //    string idStr = parameters.id;
            //    int id;
            //    bool res = int.TryParse(idStr, out id);
            //    Models.IoTProject blogPost = null;
            //    if (res)
            //    {
            //        if (filter != "")
            //        {
            //            blogPost = Models.IoTProject.NextFiltered(idStr, filter);
            //            if (blogPost != null)
            //                return View["ms_iot_Community_Samples/Index", blogPost];
            //        }
            //        else
            //        {
            //            id++;
            //            if (id < Models.IoTProject.IoTProjects.Count())
            //            {
            //                idStr = id.ToString();
            //                blogPost = Models.IoTProject.Get(idStr);
            //                if (blogPost != null)
            //                    return View["ms_iot_Community_Samples/Index", blogPost];
            //            }
            //        }
            //    }
            //    //If no next previous return current
            //    blogPost = Models.IoTProject.Get(idStr);
            //    if (blogPost != null)
            //        return View["ms_iot_Community_Samples/Index", blogPost];
            //    else
            //        return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            //};

            Get["/ms_iot_Community_Samples/Previous/{id}"] = parameters =>
            {
                string filter = (string)Request.Session["filter"];
                string idStr = parameters.id;
                int id;
                bool res = int.TryParse(idStr, out id);
                Models.IoTProject blogPost = null;
                if (res)
                {
                    if ((id < Models.IoTProject.IoTProjects.Count() ) && (id > 0))
                    {
                        if (filter != "")
                        {
                            blogPost = Models.IoTProject.PreviousFiltered(idStr, filter);
                            if (blogPost != null)
                                return View["ms_iot_Community_Samples/Index", blogPost];
                        }
                        else
                        {
                            id--;
                            if (id > -1)
                            {
                                if (id < Models.IoTProject.IoTProjects.Count())
                                {
                                    idStr = id.ToString();
                                    blogPost = Models.IoTProject.Get(idStr);
                                    if (blogPost != null)
                                        return View["ms_iot_Community_Samples/Index", blogPost];
                                }
                            }
                        }
                    }
                }
                //If no next previous return current
                blogPost = Models.IoTProject.Get(idStr);
                if (blogPost != null)
                    return View["ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/Next/{id}"] = parameters =>
            {
                string filter = (string)Request.Session["filter"];
                string idStr = parameters.id;
                int id;
                bool res = int.TryParse(idStr, out id);
                Models.IoTProject blogPost = null;
                if (res)
                { 
                    if ( (id < Models.IoTProject.IoTProjects.Count() - 1) && (id>-1))
                    {
                        if (filter != "")
                        {
                            blogPost = Models.IoTProject.NextFiltered(idStr, filter);
                            if (blogPost != null)
                                return View["ms_iot_Community_Samples/Index", blogPost];
                        }
                        else
                        {
                            id++;
                            if (id < Models.IoTProject.IoTProjects.Count())
                            {
                                idStr = id.ToString();
                                blogPost = Models.IoTProject.Get(idStr);
                                if (blogPost != null)
                                    return View["ms_iot_Community_Samples/Index", blogPost];
                            }
                        }
                    }
                }
                //If no next previous return current
                blogPost = Models.IoTProject.Get(idStr);
                if (blogPost!=null)
                    return View["ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };


            Get["/ms_iot_Community_Samples/Content/{id}"] = parameters =>
            {
                string id = parameters.id;
                Models.IoTProject blogPost = Models.IoTProject.Get(id);
                if (blogPost != null)
                    return View["ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/Reset"] = _ =>
            {
                Request.Session["filter"] = "";
                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/Reset/{Id}"] = parameters =>
            {
                Request.Session["filter"] = "";
                string id = parameters.Id;
                Models.IoTProject blogPost = Models.IoTProject.Get(id);
                if (blogPost != null)
                    return View["ms_iot_Community_Samples/Index", blogPost];
                else
                    return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/clear"] = _ =>
            {
                Request.Session["filter"] = "";
                Models.IoTProject.ClearIoTProjects();
                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
            Get["/ms_iot_Community_Samples/list"] = _ =>
            {
                string filter = (string)Request.Session["filter"];
                List<Models.IoTProject> IoTProjs = Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"]);
                return View["ms_iot_Community_Samples/IndexList", IoTProjs];
            };

            Get["/ms_iot_Community_Samples/ClearFilter"] = _ =>
            {
                //Same as reset
                Request.Session["filter"] = "";
                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };

            Get["/ms_iot_Community_Samples/SetFilter"] = _ =>
            {
                //Same as reset
                string filters = (string)Request.Session["filter"];
                return View["ms_iot_Community_Samples/SetFilter", filters];
            };
            

            Get["/ms_iot_Community_Samples/Filter/{idfilter}/{titlefilter}/{summaryfilter}/{codefilter}/{tagsfilter}/{tagsfilter2}"] = parameters =>
            {
                if (Models.IoTProject.Fields == null)
                    Models.IoTProject.Fields = new List<string>();
                if (Models.IoTProject.Fields.Count() == 0)
                {
                    var fields = typeof(Models.Project).GetFields(
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    ;
                    foreach (var fld in fields)
                    {

                        string fldName = fld.Name.Substring(1).Replace(">k__BackingField", "");
                        Models.IoTProject.Fields.Add(fldName);
                    }
                }

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
                    {
                        tupl[0] = tupl[0].Trim();
                        tupl[1] = tupl[1].Trim();
                        if (tupl[0] != "")
                            //if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                    }
                }
                filter = parameters.titlefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                    {
                        tupl[0] = tupl[0].Trim();
                        tupl[1] = tupl[1].Trim();
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                    }
                }
                filter = parameters.summaryfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                    {
                        tupl[0] = tupl[0].Trim();
                        tupl[1] = tupl[1].Trim();
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                    }
                }
                filter = parameters.codefilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("Y", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                    {
                        tupl[0] = tupl[0].Trim();
                        tupl[1] = tupl[1].Trim();
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")

                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                    }
                }
                filter = parameters.tagsfilter;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("Y", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                    {
                        tupl[0] = tupl[0].Trim();
                        tupl[1] = tupl[1].Trim();
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                    }
                }
                filter = parameters.tagsfilter2;
                filter = filter.Replace("Z", "+");
                filter = filter.Replace("z", "/");
                filter = filter.Trim();
                if (filter != "")
                {
                    tupl = filter.Split(sep);
                    if (tupl.Length == 2)
                    {
                        tupl[0] = tupl[0].Trim();
                        tupl[1] = tupl[1].Trim();
                        if (tupl[0] != "")
                            if (Models.IoTProject.Fields.Contains(tupl[0]))
                                if (tupl[1] != "")
                                    filters.Add(new Tuple<string, string>(tupl[0], tupl[1]));
                    }
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


                return View["ms_iot_Community_Samples/IndexList", Models.IoTProject.ViewIoTProjects((string)Request.Session["filter"])];
            };
        }

        GitHubClient basicGitHubClient;
        GitHubClient clientg;
        string ClientId = "";
        string ClientSecret = "";


        // This is the Callback URL that the GitHub OAuth Login page will redirect back to.
        public async Task Authorize(string code, string state)
        {
            if (!String.IsNullOrEmpty(code))
            {
                //var expectedState = Session["CSRF:State"] as string;
                //if (state != expectedState) throw new InvalidOperationException("SECURITY FAIL!");
                Session["CSRF:State"] = null;

                var token = await clientg.Oauth.CreateAccessToken(
                    new OauthTokenRequest(ClientId, ClientSecret, code)
                    {
                        RedirectUri = new Uri("http://localhost:12116/ms_iot_Community_Samples")
                    });
                Session["OAuthToken"] = token.AccessToken;
            }

            return;// RedirectToAction("Index");
        }


        private string GetOauthLoginUrl()
        {
            string csrf = Membership.GeneratePassword(24, 1);
            Session["CSRF:State"] = csrf;

            // 1. Redirect users to request GitHub access
            var request = new OauthLoginRequest(ClientId)
            {
                Scopes = { "user", "notifications" },
                State = csrf
            };
            var oauthLoginUrl = clientg.Oauth.GetGitHubLoginUrl(request);
            return oauthLoginUrl.ToString();
        }


    }



}

//http://www.jhovgaard.com/from-aspnet-mvc-to-nancy-part-2/