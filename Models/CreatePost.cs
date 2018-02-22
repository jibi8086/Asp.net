using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuNetwork.Models
{
    public class CreatePost
    {
        public string post { get; set; }
        public string postUrl { get; set; }
        public string comment { get; set; }
        public string PostMessage { get; set;}
        public int PostId { get; set; }
        public List<SelectListItem> listPost { get; set; }
    }
}