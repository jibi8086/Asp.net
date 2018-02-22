using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuNetwork.Models
{
    public class Register
    {
        public int UserId { get; set; }

        [Required]
        [RegularExpression("^[.a-zA-Z\\s.]+$", ErrorMessage = "Invalid First Name")]
        
        [MaxLength(15, ErrorMessage = "Maximum Length  is 15")]
        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "Maximum Length  is 15")]
        [RegularExpression("^[.a-zA-Z\\s.]+$", ErrorMessage = "Invalid Second Name")]
        [Display(Name = "Last Name")]
        public string secondName { get; set; }

        [Required]
        [Display(Name = "DOB")]
        public string dateOfBirth { get; set; }
                
        [Required]
        [Display(Name = "Gender")]
        public string gender { get; set; }

        
        [Required]
        [Display(Name = "Nationality")]
        public string nationality { get; set; }

        [Required]
        [Display(Name = "Address")]
        //[RegularExpression(@"^[a-zA-Z,-: ]+$", ErrorMessage = "Invalid Address")]
        [MaxLength(60, ErrorMessage = "Maximum Length  is 60")]
        public string address { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile")]

        [RegularExpression("^([0|\\+[0-9]{1,5})?([7-9][0-9]{9})$", ErrorMessage = "Invalid Phone number")]
        public string mobile { get; set; }

        [Required]
        [Display(Name = "Email")]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Invalid Email")]
        public string email { get; set; }

        [Required]
        [Display(Name ="User Name")]
        public string username { get; set; }

        [Required]
        [Display(Name ="Password")]
        public string password { get; set; }
        public string companyName { get; set; }
        public int age { get; set; }
        public string name { get; set; }
        
        public string UserPhoto { get; set; }
        [Required]
        [Display(Name ="Profile Picture")]
        public HttpPostedFileBase userImg { get; set; }

        [Display(Name = "User Type")]
        public int UserTypeID { get; set; }
        public string UserType { get; set; }
        public List<SelectListItem> listuserType { get; set; }
        [Required]
        [Display(Name = "Designation")]
        public int DesignationId { get; set; }
        public string Designation { get; set; }
        public List<SelectListItem> listDesignation { get; set; }
        public string fullName { get; set; }

    }
    public class changepassword
    {
        [Required]
        [Display(Name = "Current Password")]
        public string currentPassword { get; set; }
        [Required]
        [Display(Name = " New Password")]
        public string newPassword { get; set; }
        [Required]
        [Display(Name = " Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("newPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmPassword { get; set; }
    }
}