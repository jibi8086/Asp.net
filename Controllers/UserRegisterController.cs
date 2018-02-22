using Newtonsoft.Json;
using NuNetwork.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace NuNetwork.Controllers
{
    public class UserRegisterController : Controller
    {

        // GET: UserRegister
     
        public ActionResult UserRegister()
        {
            
            Register objDesignation = new Register();
            try
            {
                if (Session["companyid"] == null)
                {

                    return RedirectToAction("LoginUser", "UserLogin");
                }
                objDesignation.listuserType = drpList("USPUserType", "UserType", "UserTypeID");
                objDesignation.listDesignation = drpList("USPdesignation", "Designation", "DesignationID");
                return View(objDesignation);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);

            }
            finally
            {
                Dispose();
            }
            return View(objDesignation);
        }
       
        [HttpPost]
        public ActionResult UserRegister(Register Register)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                
                 using (SqlCommand UserRegistercmd = new SqlCommand("USPUserRegistration", dbHandle.con))
                    {
                    if (Session["otp"] != null)
                    {
                        Register.UserTypeID = 1;
                    }
                        DateTime dob = Convert.ToDateTime(Register.dateOfBirth);
                        string Random = System.DateTime.Now.ToString("ddMMyyhhmmss");
                        Register.UserPhoto = "../Images/" + Random + Register.userImg.FileName;
                        Register.userImg.SaveAs(Server.MapPath("../Images/") + Random + Register.userImg.FileName);
                        UserRegistercmd.CommandType = CommandType.StoredProcedure;
                        UserRegistercmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = Register.firstName;
                        UserRegistercmd.Parameters.Add("@SecondName", SqlDbType.VarChar).Value =Register.secondName;
                        UserRegistercmd.Parameters.Add("@Dob", SqlDbType.Date).Value = dob;
                        UserRegistercmd.Parameters.Add("Age", SqlDbType.Int).Value = DateTime.Today.Year - dob.Year;
                        UserRegistercmd.Parameters.Add("@Gender", SqlDbType.Int).Value = int.Parse(Register.gender);
                        UserRegistercmd.Parameters.AddWithValue("@DesignationId", SqlDbType.Int).Value = Register.DesignationId;
                        UserRegistercmd.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = Session["companyid"];
                        UserRegistercmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = Register.address;
                        UserRegistercmd.Parameters.Add("@Nationality", SqlDbType.VarChar).Value = Register.nationality;
                        UserRegistercmd.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = Register.mobile;
                        UserRegistercmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = Register.email;
                        UserRegistercmd.Parameters.Add("@UserType", SqlDbType.Int).Value = Register.UserTypeID;
                        UserRegistercmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = Register.username;
                        UserRegistercmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = Crypto.Encryption(Register.password);
                        UserRegistercmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = Register.UserPhoto;
                        UserRegistercmd.Parameters.Add("@UserStatus", SqlDbType.Int).Value = 0;
                        dbHandle.con.Open();
                        UserRegistercmd.ExecuteNonQuery();
                        Session["otp"] = null;
                        dbHandle.con.Close();
                        ViewBag.error = "Company Registration Sucess";

                        Mail.SendMail(Register.email,"Your User Name and Password ","User Name :"+Register.username+"Paassword :"+Register.password);
                    }
            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();
            }
            return RedirectToAction("LoginUser", "UserLogin");
        }

        public JsonResult validateControls(string mailidId,int Type)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand cmd = new SqlCommand("USPValidation", dbHandle.con))
                {
                    try
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        dbHandle.con.Open();
                        cmd.Parameters.AddWithValue("@data", SqlDbType.Int).Value = mailidId;
                        cmd.Parameters.AddWithValue("@type", SqlDbType.Int).Value = Type;
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            int flag = 1;
                            return Json(new { id = flag }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            int flag = 0;
                            return Json(new { id = flag }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception)
                    {
                        return Json(new { id = 404 }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id = 404 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        //created by jibin
        public JsonResult Search(int SearchValue)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                SqlDataReader DataReader;
                dbHandle.Connection();
                using (SqlCommand search = new SqlCommand("USPSearch", dbHandle.con))
                {
                    search.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    search.CommandType = CommandType.StoredProcedure;
                    search.Parameters.Add("@serchValue", SqlDbType.Int).Value = SearchValue;
                    DataReader = search.ExecuteReader();
                    if (DataReader.HasRows)
                    {
                        DataReader.Read();
                        Session["useridSearch"] = Convert.ToInt32(DataReader["userId"]);
                    }
                    else {
                        return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
                }
            }
                    
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id = 404 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }


        public ActionResult UserProfile(Register Register)
        {
                try
                {
                if (Session["userid"] != null)
                {
                    DbConnection dbHandle = new DbConnection();
                    dbHandle.Connection();
                    using (SqlCommand RegCmd = new SqlCommand("USPProfileView", dbHandle.con))
                    {
                        RegCmd.Connection = dbHandle.con;
                        RegCmd.CommandType = CommandType.StoredProcedure;
                        if (Session["useridSearch"] != null)
                        {
                            RegCmd.Parameters.AddWithValue("@UserId", Session["useridSearch"]);
                            Session["useridSearch"] = null;
                        }
                        else if (Session["userid"] != null)
                        {
                            RegCmd.Parameters.AddWithValue("@UserId", Session["userid"]);
                        }
                        dbHandle.con.Open();
                        using (SqlDataReader profile = RegCmd.ExecuteReader())
                        {
                            while (profile.Read())
                            {
                                RegCmd.Connection = dbHandle.con;
                                Register.name = profile["Name"].ToString();
                                DateTime dob = Convert.ToDateTime(profile["DateofBirth"]);
                                Register.dateOfBirth =(dob.Date).ToShortDateString();
                                Register.age = Convert.ToInt32(profile["Age"]);
                                Register.nationality = profile["Nationality"].ToString();
                                Register.address = profile["Address"].ToString();
                                Register.mobile = profile["Mobile"].ToString();
                                Register.email = profile["Email"].ToString();
                                Register.gender = profile["Gender"].ToString();
                                Register.companyName = profile["CompanyName"].ToString();
                                Register.UserPhoto =profile["ImageUrl"].ToString();                             
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("LoginUser", "UserLogin");
                }
               }
                catch (Exception e)
                {
                    ExceptionLog.Log(e, Request.UserHostAddress);
                   
                }
                finally
                {
                    Dispose();
                }
           
            return View(Register);
        }

        private static List<SelectListItem> drpList(string procedureName, string text, string value)
        {           
            SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConNew"].ConnectionString);
            try
            {
                List<SelectListItem> items = new List<SelectListItem>();
                using (SqlCommand cmd = new SqlCommand(procedureName, con))
                {
                    con.Open();
                    using (SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            items.Add(new SelectListItem
                            {
                                Text = sdr[text].ToString(),
                                Value = sdr[value].ToString()
                            });
                        }
                    }
                }
                return items;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult ProfilePost()
        {
            try
            {
                if (Session["companyid"] == null)
                {

                    return RedirectToAction("LoginUser", "UserLogin");
                }
            }
             catch (Exception e)
                {
                    ExceptionLog.Log(e, Request.UserHostAddress);
                   
                }
                finally
                {
                    Dispose();
                }
            return View();
          }


        [HttpPost]
        public JsonResult userProfilePost()
        {
            try
            {
                DataTable profileTable, profileImageTable;
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand profilePostcmd = new SqlCommand("USPUserPofilePost", dbHandle.con))
                {
                    dbHandle.con.Open();
                    profileTable = new DataTable();
                    profilePostcmd.CommandType = CommandType.StoredProcedure;
                    profilePostcmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    profilePostcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    SqlDataReader rd = profilePostcmd.ExecuteReader();
                    profileTable.Load(rd);
                    dbHandle.con.Close();
                                     
                }
                using (SqlCommand getImagecmd = new SqlCommand("USPProfilePostImages", dbHandle.con))
                {
                    dbHandle.con.Open();
                    getImagecmd.CommandType = CommandType.StoredProcedure;
                    getImagecmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    getImagecmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    profileImageTable = new DataTable();
                    SqlDataReader readerImage = getImagecmd.ExecuteReader();
                    profileImageTable.Load(readerImage);

                }

                return Json(new { profileList = JsonConvert.SerializeObject(profileTable), profileImages = JsonConvert.SerializeObject(profileImageTable) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex, Request.UserHostAddress);
                return Json(new { id = 404}, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }

        }

        [HttpPost]
        public JsonResult ProfileDetails()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable dataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand profiledetails = new SqlCommand("USPProfilePost", dbHandle.con))
                {
                    profiledetails.CommandType = CommandType.StoredProcedure;
                   
                    profiledetails.Parameters.AddWithValue("@UserId", Session["userid"]);
                    profiledetails.Parameters.AddWithValue("@CompanyId", Session["companyid"]);

                    dbHandle.con.Open();

                    SqlDataReader reader = profiledetails.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
               
            }
            finally
            {
                Dispose();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ShowProfileComment(string Postid)
        { 
            try
            {
                DataTable dataTable = new DataTable();
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand userpostcomment = new SqlCommand("USPUserPostComment", dbHandle.con))
                {
                    userpostcomment.CommandType = CommandType.StoredProcedure;
                    userpostcomment.Parameters.Add("@PostId", SqlDbType.Int).Value = Postid;
                    userpostcomment.Parameters.AddWithValue("@UserId", Session["userid"]);
                    dbHandle.con.Open();

                    SqlDataReader reader = userpostcomment.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
            }
            finally
            {
                Dispose();
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangePassword()
        {
            if (Session["companyid"] == null)
            {

                return RedirectToAction("LoginUser", "UserLogin");
            }
            return View();
        }
        [HttpPost]
        public JsonResult ShowProfileReplay(string commentid)
        {
          
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable dataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand userpostcomment = new SqlCommand("USPShowReplay", dbHandle.con))
                {
                    userpostcomment.CommandType = CommandType.StoredProcedure;
                    userpostcomment.Parameters.Add("@CommentId", SqlDbType.Int).Value = commentid;
                    userpostcomment.Parameters.Add("@userid", SqlDbType.Int).Value= Convert.ToInt32(Session["userid"]);
                    dbHandle.con.Open();
                    SqlDataReader reader = userpostcomment.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
           
        }
       
        [HttpPost]
        public ActionResult changePassword(changepassword changepassword)
        {
            int passvalue = 0 ;
            try
            {
                if (Session["userid"] != null)
                {
                    DbConnection dbHandle = new DbConnection();
                    dbHandle.Connection();
                    string passwordEncryption = Crypto.Encryption(changepassword.currentPassword);
                    using (SqlCommand chgpass = new SqlCommand("USPProfileView", dbHandle.con))
                    {
                        chgpass.Connection = dbHandle.con;
                        dbHandle.con.Open();
                        chgpass.CommandType = CommandType.StoredProcedure;
                        chgpass.Parameters.Add("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                        SqlDataReader reader = chgpass.ExecuteReader();
                        while (reader.Read())
                        {
                            if (passwordEncryption == reader["Password"].ToString())
                            {
                                passvalue = 1;
                            }
                        }
                        reader.Close();
                        dbHandle.con.Close();
                        if (passvalue == 1)
                        {
                            string newEncrptedPassword = Crypto.Encryption(changepassword.newPassword);
                            dbHandle.Connection();
                            using (SqlCommand changepass = new SqlCommand("USPChangePassword", dbHandle.con))
                            {
                                changepass.CommandType = CommandType.StoredProcedure;
                                changepass.Parameters.Add("@Password", SqlDbType.VarChar).Value = newEncrptedPassword;
                                changepass.Parameters.Add("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                                changepass.Parameters.Add("@CompanyCode", SqlDbType.Int).Value = Convert.ToInt32(Session["companyid"]);
                                dbHandle.con.Open();
                                changepass.ExecuteNonQuery();
                                dbHandle.con.Close();
                            }
                            ViewBag.Message = "changed";
                            return View(changepassword);
                        }
                        else
                        {
                            return RedirectToAction("ChangePassword", "UserRegister");
                        }
                    }
                }

                else
                {
                    return RedirectToAction("LoginUser", "UserLogin");
                }
            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();
            }
           
           
        }
        public ActionResult ChangeProfileImage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ChangeProfilePicture()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                HttpPostedFile pic = System.Web.HttpContext.Current.Request.Files["inputfile"];
                string path = "";
                HttpPostedFile filebase = (HttpPostedFile)(pic);
                string Random = System.DateTime.Now.ToString("ddMMyyhhmmss");
                string fileName = Random + Path.GetFileName(filebase.FileName);
                path = ("../Images/") + fileName;
                filebase.SaveAs(Server.MapPath("../Images/") + fileName);
                using (SqlCommand Imagecmd = new SqlCommand("USPChangeProfileImage", dbHandle.con))
                {
                    Imagecmd.CommandType = CommandType.StoredProcedure;  
                    Imagecmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    Imagecmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = path;
                    Imagecmd.Parameters.Add("@CompanyCode", SqlDbType.Int).Value = Session["companyid"];
                    dbHandle.con.Open();
                    Imagecmd.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(JsonRequestBehavior.AllowGet);
                }               
            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id = 404 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }          
        }

        [HttpPost]
        public JsonResult DeleteUserComment(string CommentId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand deletePost = new SqlCommand("USPDeleteComment", dbHandle.con))
                {
                    deletePost.CommandType = CommandType.StoredProcedure;
                    deletePost.Parameters.Add("@CommentId", SqlDbType.Int).Value = Convert.ToInt32(CommentId);
                    deletePost.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    dbHandle.con.Open();
                    deletePost.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
            
        }
    }
}