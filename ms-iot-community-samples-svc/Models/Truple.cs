using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ms_iot_community_samples_svc.Models
{
    public class AuthenticateTruple
    {
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Repo { get; set; }
    }

    public class GetRepoMDQuaduple
    {
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Repo { get; set; }
        public string Filename { get; set; }
    }
}