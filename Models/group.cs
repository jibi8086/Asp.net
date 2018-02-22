using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace NuNetwork.Models
{
    public class group
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "Company Name")]
        public string groupName2 { get; set; }
        public string addGroupMembers { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "Group Name")]
        public string groupName { get; set; }
        public string txtGroup { get; set; }


        [Required]
      //  [Display(Name = "Profile Pic")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase groupImageData { get; set; }
        public string groupPhoto { get; set; }

    }

}