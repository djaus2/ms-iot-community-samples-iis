using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ms_iot_community_samples_svc.Models
{
    public class Errors
    {
        public string Title { get; set; } = "Error Message";
        public string Message { get; set; } = "";
        public string Source { get; set; } = "";
        public bool LoggedInStatus { get; set; } = false;

        //public bool loggedInStatus
        //{
        //    get { return LoggedInStatus; }
        //}

        public string GetLogLink(bool loggedInStatus)
        {
            if (!LoggedInStatus)
                return "/ms_iot_Community_Samples/LoginPost";
            else
                return "/ms_iot_Community_Samples/Logout/0";
        }

        public string LogLink
        {
            get {
                if (!LoggedInStatus)
                    return "/ms_iot_Community_Samples/LoginPost";
                else
                    return "/ms_iot_Community_Samples/Logout/0";
            }
        }
        public string LogLabel
        {
            get
            {
                if (!LoggedInStatus)
                    return "Login";
                else
                    return "Logout";
            }
        }
    }
}