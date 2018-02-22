using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuNetwork.Models;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace NuNetwork.Controllers
{
    public class ChattingController : Controller
    {
        // GET: Chatting`
    
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult chatting()
        {
            return View();
        }
        public ActionResult chat()
        {
            return View();
        }
        [HttpPost]
        public JsonResult FetchMessage(int fromId,int ToId,int cId)
        {
            DbConnection dbHandle = new DbConnection();
            dbHandle.Connection();
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlCommand PersonalChatSelectcmd = new SqlCommand("USPSelectPersonalChatMessage", dbHandle.con))
                {

                    PersonalChatSelectcmd.CommandType = CommandType.StoredProcedure;
                    PersonalChatSelectcmd.Parameters.AddWithValue("@UserIdFrom", SqlDbType.Int).Value = fromId;
                    PersonalChatSelectcmd.Parameters.AddWithValue("@UserIdTo", SqlDbType.Int).Value = ToId;
                    PersonalChatSelectcmd.Parameters.AddWithValue("@companyid", SqlDbType.Int).Value = cId;
                    dbHandle.con.Open();
                    using (SqlDataReader reader = PersonalChatSelectcmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }

                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();  
             
               
            }
           
        }

        [HttpPost]
        public JsonResult FetchGroupMessage(int groupId, int cId,int userId)
        {
            DbConnection dbHandle = new DbConnection();
            dbHandle.Connection();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlCommand groupChatcmd = new SqlCommand("USPSelectGroupChatMessage", dbHandle.con))
                {

                    groupChatcmd.CommandType = CommandType.StoredProcedure;
                    groupChatcmd.Parameters.AddWithValue("@userid", SqlDbType.Int).Value = userId;
                    groupChatcmd.Parameters.AddWithValue("@groupid", SqlDbType.Int).Value = groupId;

                    groupChatcmd.Parameters.AddWithValue("@companyid", SqlDbType.Int).Value = cId;
                    dbHandle.con.Open();
                    using(SqlDataReader reader = groupChatcmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }

                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();
             
            }

        }

        [HttpPost]
        public JsonResult FetchGroup(int uId)
        {
            DbConnection dbHandle = new DbConnection();
            dbHandle.Connection();
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlCommand selectGroupcmd = new SqlCommand("USPSelectGroup", dbHandle.con))
                {

                    selectGroupcmd.CommandType = CommandType.StoredProcedure;
                    selectGroupcmd.Parameters.AddWithValue("@userid", SqlDbType.Int).Value = uId;
                    dbHandle.con.Open();
                    using (SqlDataReader reader = selectGroupcmd.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }

                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();
             
            }

        }

        [HttpPost]
        public JsonResult FetchUserList(int uId, int cId)
        {
            DbConnection dbHandle = new DbConnection();

            dbHandle.Connection();
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlCommand listEmpcmd = new SqlCommand("USPEmployeeList", dbHandle.con))
                {

                    listEmpcmd.CommandType = CommandType.StoredProcedure;
                    listEmpcmd.Parameters.AddWithValue("@CompanyId", SqlDbType.Int).Value = cId;
                    listEmpcmd.Parameters.AddWithValue("@UserId", SqlDbType.Int).Value = uId;
                    dbHandle.con.Open();
                    using (SqlDataReader reader = listEmpcmd.ExecuteReader())
                    {

                        dataTable.Load(reader);
                    }

                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();
              
            }
        }
        [HttpPost]
        public JsonResult MessageCount(int uId, int cId,int fId)
        {
            DbConnection dbHandle = new DbConnection();

            dbHandle.Connection();
            int flag=0;
            try
            {
                using (SqlCommand countMsgcmd = new SqlCommand("USPMessageCount", dbHandle.con))
                {

                    countMsgcmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@flag", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    countMsgcmd.Parameters.Add(parm);                  
                    countMsgcmd.Parameters.AddWithValue("@FromId", SqlDbType.Int).Value = fId;
                    countMsgcmd.Parameters.AddWithValue("@ToId", SqlDbType.Int).Value = uId;
                    dbHandle.con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(countMsgcmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    flag = Convert.ToInt32(parm.Value);
                }
                return Json(new { id = flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();

            }
        }
        [HttpPost]
        public JsonResult MessageRead(int tId, int cId, int fId)
        {
            DbConnection dbHandle = new DbConnection();

            dbHandle.Connection();
            int flag = 0;
            try
            {
                using (SqlCommand msgReadcmd = new SqlCommand("USPMessageRead", dbHandle.con))
                {

                    msgReadcmd.CommandType = CommandType.StoredProcedure;
                    msgReadcmd.Parameters.AddWithValue("@FromId", SqlDbType.Int).Value = fId;
                    msgReadcmd.Parameters.AddWithValue("@ToId", SqlDbType.Int).Value = tId;
                    dbHandle.con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(msgReadcmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                }
                return Json(new { id = flag }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();

            }
        }

        [HttpPost]
        public JsonResult OfflineChat(int fromId, int toId,string name ,string message, int cId)
        {
            DbConnection dbHandle = new DbConnection();

            dbHandle.Connection();
            DataTable dataTable = new DataTable();

            try
            {
                ChatModel.PersonalChatDb(fromId, toId, message,cId);
                return Json(new { id = toId,name=name,msg=message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbHandle.con.Close();
                Dispose();
           
            }

        }


    }
}