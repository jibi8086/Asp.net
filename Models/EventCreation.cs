using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NuNetwork.Models
{
    public class EventCreation
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Invalid Event Name")]
        [MaxLength(35, ErrorMessage = "Maximum Length  is 15")]
        [Display(Name = "Event Name")]
        public string eventName { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Maximum Length  is 50")]
        [Display(Name = "Details")]
        public string details { get; set; }
        [Required]
        [RegularExpression("^[#.0-9a-zA-Z\\s,-:]+$", ErrorMessage = "place is not valid")]
        [MaxLength(20, ErrorMessage = "Maximum Length  is 20")]
        [Display(Name = "Where")]
        public string where { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public string startDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public string endDate { get; set; }
        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2]):[0-5][0-9] (am|pm|AM|PM)$", ErrorMessage = "Invalid Time.")]
        [Display(Name = "Time")]
        public string time { get; set; }
    }
}