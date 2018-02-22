using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Configuration;
using System.Data;
using NuNetwork.Models;

namespace NuNetwork.Controllers
{
    public class NewsFeedsController : Controller
    {
      
        // GET: NewsFeeds
        public ActionResult NewsFeed()
        {
            return View();
        }

        public JsonResult Search(string prefix)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand searchCmd = new SqlCommand("USPSearchAutoComplete", dbHandle.con))
                {
                    searchCmd.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    searchCmd.CommandType = CommandType.StoredProcedure;
                    searchCmd.Parameters.Add("@serchValue", SqlDbType.VarChar).Value = prefix;
                    searchCmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(searchCmd);
                    da.Fill(ds);
                    List<search> searchlist = new List<search>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        searchlist.Add(new search
                        {
                            Name = dr["FirstName"].ToString(),
                            LastName= dr["LastName"].ToString(),
                            Sr_no = dr["UserId"].ToString()
                        });
                    }
                    return Json(searchlist, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
         }
        }
    }
