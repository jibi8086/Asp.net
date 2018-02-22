using NuNetwork.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuNetwork.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public ActionResult ViewCompanies()
        {
              try
                {
                    if (Session["userid"] != null)
                    {

                    DbConnection dbHandle = new DbConnection();
                    dbHandle.Connection();
                    using (SqlCommand getemployees = new SqlCommand("USPCompanyDetailsForApproveOrDisapprove", dbHandle.con))
                    {
                        getemployees.Connection = dbHandle.con;
                        dbHandle.con.Open();
                        getemployees.CommandType = CommandType.StoredProcedure;
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
                    return View();
                }
                finally
                {
                   
                    Dispose();

                }
      
          
        }


        [HttpPost]
        public JsonResult BlockOrUnblockCompany(string CompanyId, string blockOperation)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand blockorunblock = new SqlCommand("USPApproveOrDisApproveCompany", dbHandle.con))
                {
                    blockorunblock.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    blockorunblock.CommandType = CommandType.StoredProcedure;
                    blockorunblock.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(CompanyId);
                    blockorunblock.Parameters.Add("@Operation", SqlDbType.Int).Value = Convert.ToInt32(blockOperation);
                    blockorunblock.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(new { id = 1 },JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                ExceptionLog.Log(ex, Request.UserHostAddress);
                return Json(new { id = ex }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();

            }
        }
    }
}