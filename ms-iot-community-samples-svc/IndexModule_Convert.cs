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

namespace ms_iot_community_samples_svc
{
    public partial class IndexModule : NancyModule
    {
        private Models.Errors  Convert()
        {
            Models.Errors errorMsg = new Models.Errors();
            DoStrings();
            if (!(bool)Request.Session["LoggedInStatus"])
            {
                errorMsg.Message = "Not logged in!";
                errorMsg.Source = "/Convert";
                errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                // return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                return errorMsg;
            }

            if (!Directory.Exists(MD))
            {
                errorMsg.Message = "No files to convert...No MD Dir/r/nRun 'Sync with GitHub' first";
                errorMsg.Source = "/Convert";
                errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                // return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                return errorMsg;
            }

            if ((Directory.GetFiles(MD, "*.MD")).Length == 0)
            {
                errorMsg.Message = "No files to convert... No files in MD dir./r/nRun  'Sync with GitHub' first";
                errorMsg.Source = "/Convert";
                errorMsg.LoggedInStatus = (bool)Request.Session["LoggedInStatus"];
                // return View["ms_iot_Community_Samples/ErrorPage", errorMsg];
                return errorMsg;
            }

            if (!Directory.Exists(jsonDirr))
                Directory.CreateDirectory(jsonDirr);
            string[] files0 = Directory.GetFiles(jsonDirr, "*.*");
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
                try
                {
                    string filename = Path.GetFileNameWithoutExtension(file);
                    if (IgnoreMDS.Contains(filename.ToLower()))
                        continue;
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
                    Models.IoTProject iotProject = new Models.IoTProject();
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
                                else if ((vname == "GitHub") || (vname == "HacksterIO") || (vname == "Codeplex") || (vname == "Blog"))
                                    continue;
                                vvalue = "";
                            }
                            else if (parts.Length == 2)
                            {
                                vname = parts[0].Trim();
                                if (vname == "")
                                    continue;
                                else if ((vname == "GitHub") || (vname == "HacksterIO") || (vname == "Codeplex") || (vname == "Blog"))
                                    continue;
                                vvalue = parts[1].Trim();
                            }
                            else
                            {
                                //If value part in line had extra : as in Urls then get more thean 2 parts
                                //In that case merge all parts above index 0.
                                //Actually just remove parts[0] and : at start of line.
                                vname = parts[0].Trim();
                                vvalue = newLine.Substring(vname.Length + 1).Trim();
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
                            if ((vname == "") || (vvalue == ""))
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
                                        List<string> los = aos.ToList<string>();
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
                        //Abort this db record
                        break;
                    }
                    bool gotFile = false;
                    //Tests for an empty project
                    if (iotProject.Title != "")
                        if ((iotProject.HacksterIO + iotProject.GitHub + iotProject.Codeplex).Trim() != "")
                        {
                            string name = Path.GetFileName(file);
                            File.WriteAllText(Path.Combine(MD2, name), fileTxt);
                            gotFile = true;
                        }
                    if (!gotFile)
                    {
                        //Remove this file from list and get next file
                        Models.IoTProject.IoTProjects.Remove(iotProject);
                        continue;
                    }
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

            errorMsg.Message = "OK";
            return errorMsg;

        }
    }
}