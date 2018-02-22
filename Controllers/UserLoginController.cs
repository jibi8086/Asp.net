using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuNetwork.Models;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Hosting;
using Microsoft.AspNet.SignalR;
using NuNetwork.Hubs;

namespace NuNetwork.Controllers
{
    public class UserLoginController : Controller
    {
     
       
       
        // GET: UserLogin
        public ActionResult LoginUser()
        {
            try
            {
                if ((Session["userid"] != null) && (Session["usertype"]) != null && (Session["companyid"] != null))
                {
                    if (Convert.ToInt32(Session["usertype"]) == 1)
                    {
                        return RedirectToAction("CreatePost", "CreatePost");
                    }
                    if (Convert.ToInt32(Session["usertype"]) == 2)
                    {
                        //user
                        return RedirectToAction("CreatePost", "CreatePost");
                    }
                    if (Convert.ToInt32(Session["usertype"]) == 3)
                    {
                        //superadmin
                        return RedirectToAction("UserProfile", "UserRegister");
                    }


                }
                else
                {
                    return View();
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
        public ActionResult LoginUser(LoginModel login)
        {
            DbConnection dbhandle = new DbConnection();

            SqlCommand User = null;
            try
            { 
              if ((Session["userid"] != null) && (Session["usertype"]) != null && (Session["companyid"] != null))
                 {
                    return RedirectToAction("UserProfile", "UserRegister");
                 }

              else
                  {

                    if (login.UserName == null || login.Password == null)
                       {
                      ViewBag.error = "Fields cannot be empty";
                     return View();
                       }
                         else
                        {
                           dbhandle.Connection();
                           User = new SqlCommand("USPLoginProcedure", dbhandle.con);
                           dbhandle.con.Open();
                           User.CommandType = CommandType.StoredProcedure;
                           User.Parameters.Add("@username", SqlDbType.VarChar).Value = login.UserName;
                           User.Parameters.Add("@password", SqlDbType.VarChar).Value = Crypto.Encryption(login.Password);
                           using (SqlDataReader DataReader = User.ExecuteReader())
                            {
                              if (DataReader.HasRows)
                                  {
                                     DataReader.Read();
                                     Session["userid"] = Convert.ToInt32(DataReader["userId"]);
                                     Session["companyid"] = Convert.ToInt32(DataReader["companyId"]);
                                     Session["usertype"] = Convert.ToInt32(DataReader["userType"]);
                                     Session["name"] = Convert.ToString(DataReader["Name"]);
                                     ViewBag.name = Convert.ToString(DataReader["Name"]);
                            }
                           else
                           {
                                ViewBag.error = "Wrong Username or Password";
                                return View();
                           }
                         }
                         dbhandle.con.Close();

                             if (Convert.ToInt32(Session["usertype"]) == 1)
                             {
                        //admin
                        
                              return RedirectToAction("CreatePost", "CreatePost");

                             }
                             if (Convert.ToInt32(Session["usertype"]) == 2)
                             {
                        //user
                       
                                return RedirectToAction("CreatePost", "CreatePost");
                              }
                           if (Convert.ToInt32(Session["usertype"]) == 3)
                             {
                        //superadmin
                        //return RedirectToAction("");
                        return RedirectToAction("UserProfile", "UserRegister");
                    }





                }

            }
        }
               catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                ViewBag.error = "Something went wrong";
                //return View();
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();
            }
            return View();
        }

        public ActionResult SessionClear()
        {

            try
            {

                Session.Abandon();
                Session.RemoveAll();
                Session["userid"] = null;
                Session["companyid"] = null;
                Session["usertype"] = null;
                Session["name"] = null;


            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
            }
            finally
            {
                Dispose();
            }

            return RedirectToAction("LoginUser","UserLogin");
        }

        public ActionResult ForgotPassword()
        {
          

            return View();
            
       }



     

        public ActionResult ForgetPassword(ForgetModel forget)
        {
            DbConnection dbhandle = new DbConnection();
            dbhandle.Connection();
            try
            {
                if (ModelState.IsValid)
                {


                    using (SqlCommand FrgtCmd = new SqlCommand("USPForgotPassword", dbhandle.con))
                    {
                        FrgtCmd.Connection = dbhandle.con;
                        FrgtCmd.CommandType = CommandType.StoredProcedure;
                        FrgtCmd.Parameters.AddWithValue("@EmailId", forget.Email);
                        dbhandle.con.Open();

                        using (SqlDataReader profile = FrgtCmd.ExecuteReader())
                        {
                            if (profile.HasRows)
                            {
                                while (profile.Read())
                                {
                                    FrgtCmd.Connection = dbhandle.con;
                                    string emailId = profile["Email"].ToString();
                                    string Username = profile["UserName"].ToString();
                                    string Password = profile["Password"].ToString();
                                    string message = "Hi,\n Your request for Username and password is recieved.\n \n  Username: " + Username.ToString() + " \n Password : " + Crypto.Decrypt(Password.ToString());
                                    Mail.SendMail(emailId, "Forget Username and Password", message);
                                    ViewBag.message = " Your Password has been sent to your mail";
                                }
                            }
                            else
                            {
                                ViewBag.message = " Email is not Registered";
                                return View("ForgotPassword");

                            }
                        }

                    }
                    dbhandle.con.Close();
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {

                Dispose();
            }
            
            
            return View("ForgotPassword"); 
        }
       
    }
}