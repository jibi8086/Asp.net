using NuNetwork.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace NuNetwork.Controllers
{
    public class CompanyRegisterController : Controller
    {
        // GET: CompanyRegister
        public ActionResult CompanyRegistration()
        {
            try
            {
                CompanyRegistrationModel objuser = new CompanyRegistrationModel();
                if (Session["companyid"] != null)
                {
                  //  return RedirectToAction("UserRegister", "UserRegister");
                }
                objuser.drpStateName = drpList("stateView", "StateName", "StateId");

                return View(objuser);
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
        }


        public ActionResult OtpVerification()
        {
            return View();
        }
        //View Employee
        public ActionResult ViewEmploye()
        {
            try
            {
                if (Session["companyid"] != null)
                {
                    DbConnection dbHandle = new DbConnection();
                    dbHandle.Connection();
                    using (SqlCommand getemployees = new SqlCommand("USPCompanyEmployees", dbHandle.con))
                    {
                        getemployees.Connection = dbHandle.con;
                        dbHandle.con.Open();
                        getemployees.CommandType = CommandType.StoredProcedure;
                        getemployees.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyid"]);
                        using (SqlDataReader blockList = getemployees.ExecuteReader())
                        {
                            DataTable employeeTable = new DataTable();
                            employeeTable.Load(blockList);
                            dbHandle.con.Close();
                            return View(employeeTable);
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
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();

            }           
        }
        //Edit Employee
        public JsonResult EditEmployeeById(int editId)
        {
            try
            {
                Session["editEmployeeId"] = editId; 
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        //Remove Employee
        public JsonResult RemoveEmployeeById(int userId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand removeempolyees = new SqlCommand("USPRemoveEmployee", dbHandle.con))
                {
                    removeempolyees.CommandType = CommandType.StoredProcedure;
                    removeempolyees.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                    dbHandle.con.Open();
                    removeempolyees.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
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
        //Show Employee Details
        public ActionResult EditEmployee()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                Register employee = new Register();
                dbHandle.Connection();
                using (SqlCommand getEmployeeDetails = new SqlCommand("USPProfileForEdit", dbHandle.con))
                {
                    getEmployeeDetails.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    getEmployeeDetails.CommandType = CommandType.StoredProcedure;
                    getEmployeeDetails.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["editEmployeeId"]);
                    using (SqlDataReader employeeData = getEmployeeDetails.ExecuteReader())
                    {
                        if (employeeData.HasRows)
                        {
                            employeeData.Read();
                            employee.firstName = employeeData["FirstName"].ToString();
                            employee.secondName = employeeData["LastName"].ToString();
                            employee.dateOfBirth = employeeData["Dob"].ToString();
                            employee.mobile = employeeData["Mobile"].ToString();
                            employee.email = employeeData["Email"].ToString();
                            employee.address = employeeData["Address"].ToString();
                        }
                    }
                }
                dbHandle.con.Close();
                return View(employee);
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex, Request.UserHostAddress);
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();
            }

        }
        // Edit Employee Details
        [HttpPost]
       public ActionResult EditEmployee(Register updateemployee)
        {
           
            try
            {
                if (Session["userid"] != null && Session["companyid"] != null)
                {
                    DbConnection dbHandle = new DbConnection();
                    dbHandle.Connection();
                using (SqlCommand updateUserDetails = new SqlCommand("USPUpdateUserDetails", dbHandle.con))
                {                    
                    updateUserDetails.CommandType = CommandType.StoredProcedure;
                    updateUserDetails.Parameters.Add("@Userid", SqlDbType.Int).Value = Convert.ToInt32(Session["editEmployeeId"]);
                    updateUserDetails.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = updateemployee.firstName;
                    updateUserDetails.Parameters.Add("@SecondName", SqlDbType.VarChar).Value = updateemployee.secondName;
                    updateUserDetails.Parameters.Add("@CompanyCode", SqlDbType.Int).Value = Convert.ToInt32(Session["companyid"]);
                    updateUserDetails.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = updateemployee.mobile;
                    updateUserDetails.Parameters.Add("@Email", SqlDbType.VarChar).Value = updateemployee.email;
                    updateUserDetails.Parameters.Add("@Address", SqlDbType.VarChar).Value = updateemployee.address;
                    dbHandle.con.Open();
                    updateUserDetails.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return RedirectToAction("ViewEmploye", "CompanyRegister");
                    }
                    //ViewBag.Message = "Registered";
                 
                }
                else
                {
                    return RedirectToAction("ViewEmploye", "CompanyRegister");
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
        }
        //Validate Email Id Is Already Exists
        public JsonResult MailCheck(string mailidId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand checkMail = new SqlCommand("USPCheckMail", dbHandle.con))
                {
                    checkMail.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    checkMail.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@flag", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    checkMail.Parameters.Add(parm);
                    checkMail.Parameters.AddWithValue("@email", SqlDbType.VarChar).Value = mailidId;
                    SqlDataAdapter da = new SqlDataAdapter(checkMail);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    dbHandle.con.Close();
                    return Json(new { id = flag }, JsonRequestBehavior.AllowGet);
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
        // Insert Company Details
        [HttpPost]
        public ActionResult CompanyInsert(CompanyRegistrationModel insert)
        {
            SqlParameter parm;
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand companyRegister = new SqlCommand("USPCompanyRegistration", dbHandle.con))
                {
                    Random random = new Random();
                    int randomNumber = random.Next(999999);
                    randomNumber = 1 + randomNumber;
                    string randomNum = randomNumber.ToString();
                    companyRegister.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    companyRegister.CommandType = CommandType.StoredProcedure;
                    parm = new SqlParameter("@CompanyId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    companyRegister.Parameters.Add(parm);
                    companyRegister.Parameters.Add("@CompanyName", SqlDbType.VarChar).Value = insert.companyName;
                    companyRegister.Parameters.Add("@CompanyCode", SqlDbType.VarChar).Value = insert.companyCode;
                    companyRegister.Parameters.Add("@EmailId", SqlDbType.VarChar).Value = insert.email;
                    companyRegister.Parameters.AddWithValue("@stateId", SqlDbType.Int).Value = insert.stateId;
                    companyRegister.Parameters.Add("@LandLine", SqlDbType.VarChar).Value = insert.landLine;
                    companyRegister.Parameters.Add("@MobileNumber", SqlDbType.VarChar).Value = insert.PhoneNumber;
                    companyRegister.Parameters.Add("@CEO", SqlDbType.VarChar).Value = insert.ceo;
                    companyRegister.Parameters.Add("@otp", SqlDbType.VarChar).Value = randomNum;
                    SqlDataAdapter da = new SqlDataAdapter(companyRegister);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    string message = insert.companyName + " Company registered succesfully, For Register Company admin account click this link http://192.168.1.77:80/CompanyRegister/OtpVerification and OTP is " + randomNum;
                    Mail.SendMail(insert.email, "NuNetwork Company Registration", message);
                    Session["companyid"] = flag;
                    dbHandle.con.Close();
                    return RedirectToAction("OtpVerification", "CompanyRegister");
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
           
        }
        //DropDown List State
        private static List<SelectListItem> drpList(string procedureName, string text, string value)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["ConNew"].ConnectionString);                
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
                con.Close();
                return items;
            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex,"");
                return items;
            }
            finally
            {
                 
            }

        }
        //Otp Validation
        public JsonResult OtpValidation(string otp)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand CheckOtp = new SqlCommand("USPOtpVerification", dbHandle.con))
                {
                    CheckOtp.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    CheckOtp.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@flag", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    CheckOtp.Parameters.Add(parm);
                    CheckOtp.Parameters.AddWithValue("@otpNumber", SqlDbType.VarChar).Value = otp;
                    SqlDataAdapter da = new SqlDataAdapter(CheckOtp);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    Session["companyid"] = flag;
                    Session["otp"] = otp;
                    dbHandle.con.Close();
                    return Json(new { id = flag }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }

            }
        public ActionResult Error_View()
        {
           
                return View();
           
        }
    }

}