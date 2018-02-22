using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuNetwork.Models
{
    public class CompanyRegistrationModel
    {

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "Company Name")]
        public string companyName { get; set; }

        [Required]
        [Display(Name = "Company Code")]
        public string companyCode { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "City")]
        public string city { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        public int lblEmail { get; set; }
        //[Display(Name = "Name")]
        //public string lblEmail { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "City Name")]
        public string cityId { get; set; }
        public List<SelectListItem> drpCityName { get; set; }



        [Required, Range(1, int.MaxValue, ErrorMessage = "Error: Must Choose a Country")]
  
        public int stateId { get; set; }
        [Display(Name = "State")]
        public string State { get; set; }
        public List<SelectListItem> drpStateName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "CEO")]
        public string ceo { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Land Line")]
        [Required(ErrorMessage = "Land Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string landLine { get; set; }


        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression("^([0|\\+[0-9]{1,5})?([7-9][0-9]{9})$", ErrorMessage = "Invalid Phone number")]

        public string PhoneNumber { get; set; }


        [Required]
        [Display(Name = "Mobile")]
        public string mobile { get; set; }


        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        [Display(Name = "City")]
        public string cityName { get; set; }
    }
}