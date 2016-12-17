using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ms_iot_community_samples_svc.Models
{
    /* Used in Javascript to turn a string parameter passed (which is passed as a literal) to .cshtml page, as the (string) name of a view.
    *
    * In IndexModule.cs Pass the calling page to the login page as a string parameter
              Get["/ms_iot_Community_Samples/login/{referer}"] = parameters => {
                   string referer = parameters.referer;
                   if (referer == "0")
                       referer = "ms_iot_Community_Samples";
                   else
                       referer = "IndexList";
                   return View["/ms_iot_Community_Samples/login",referer];
              };


    * In login.cshtml   The parameter @Model gets interpretted as a literal for a string in the Javascript
    @inherits Nancy.ViewEngines.Razor.NancyRazorViewBase<string>
              var referer = msiotCommunitySamples.Models.Views.@Model; 
              var windowlocation = "/ms_iot_Community_Samples/onlogin/" + referer + "/" + user +"/" + pwd;

    * In IndexModule.cs  This is where it finally gets used.
    *   
                Get["/ms_iot_Community_Samples/onlogin/{referer}"] = parameters => {
                   string referer = parameters.referer;
 ...
 ...
 ...
                if (referer== "ms_iot_Community_Samples")
                    return View["/ms_iot_Community_Samples/" + referer, errorMsg];
                else
                    return View["/ms_iot_Community_Samples/" + referer", Models.BlogPost.ViewBlogPostz((string)Request.Session["filter"])];
            };

    *
    */
    public static class Views
    {



        const string ms_iot_Community_Samples = "ms_iot_Community_Samples";
        const string IndexList = "IndexList";
        const string Index = "Index";
        const string ErrorPage = "ErrorPage";
        const string Login = "Login";
    }
}