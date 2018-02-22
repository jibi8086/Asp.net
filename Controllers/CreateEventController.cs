using Newtonsoft.Json;
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
    public class CreateEventController : Controller
    {
        public ActionResult CreateEvent()
        {
            if (Session["userid"] == null)
            {
                return RedirectToAction("LoginUser", "UserLogin");

            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult CreateEvent(EventCreation eventcreation )
        { 
            try
            {
                DbConnection dbHandle = new DbConnection();
                if (Session["userid"] != null  && Session["companyid"] != null)
                {
                    dbHandle.Connection();
                    using (SqlCommand createevent = new SqlCommand("USPEventCreation", dbHandle.con))
                    {
                        DateTime startdate = Convert.ToDateTime(eventcreation.startDate);
                        DateTime enddate = Convert.ToDateTime(eventcreation.endDate);
                        createevent.CommandType = CommandType.StoredProcedure;
                        createevent.Parameters.Add("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                        createevent.Parameters.Add("@EventName", SqlDbType.VarChar).Value = eventcreation.eventName;
                        createevent.Parameters.Add("@EventStatusId", SqlDbType.Int).Value = 1;
                        createevent.Parameters.Add("@EventDescription", SqlDbType.VarChar).Value =eventcreation.details;
                        createevent.Parameters.Add("@StartDate", SqlDbType.Date).Value = startdate;
                        createevent.Parameters.Add("@EndDate", SqlDbType.Date).Value = enddate;
                        createevent.Parameters.Add("@Location", SqlDbType.VarChar).Value = eventcreation.where;
                        createevent.Parameters.Add("@Time", SqlDbType.VarChar).Value = eventcreation.time;
                        createevent.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                        createevent.Parameters.Add("@ModifyBy", SqlDbType.Int).Value = Convert.ToInt32(Session["userid"]);
                        createevent.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Convert.ToInt32(Session["companyid"]);
                        dbHandle.con.Open();
                        createevent.ExecuteNonQuery();
                        dbHandle.con.Close();
                    }
                    ViewBag.Message = "Event Created";
                    return View(eventcreation);
                }
                else
                {
                    return RedirectToAction("CreatePost", "CreatePost");
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
               // return RedirectToAction("CreatePost", "CreatePost");
               return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();
             
            }
        }
        [HttpPost]
        public JsonResult GroupDisplay()
        {   
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable GroupDisplydataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand groupdisplay = new SqlCommand("USPGroupView", dbHandle.con))
                {
                    groupdisplay.CommandType = CommandType.StoredProcedure;
                    groupdisplay.Parameters.AddWithValue("@UserId", Session["userid"]); 
                    dbHandle.con.Open();
                    SqlDataReader reader = groupdisplay.ExecuteReader();
                    GroupDisplydataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(GroupDisplydataTable), JsonRequestBehavior.AllowGet);
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
        public JsonResult EventDisplay()
        {    
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable eventDisplydataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand eventdisplay = new SqlCommand("USPEventView", dbHandle.con))
                {
                    eventdisplay.CommandType = CommandType.StoredProcedure;
                    eventdisplay.Parameters.AddWithValue("@CompanyId",Session["companyid"]);
                    dbHandle.con.Open();
                    SqlDataReader reader = eventdisplay.ExecuteReader();
                    eventDisplydataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(eventDisplydataTable), JsonRequestBehavior.AllowGet);
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
        public JsonResult Eventdata( string EventId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand eventview = new SqlCommand("USPEventView", dbHandle.con))
                {
                    eventview.CommandType = CommandType.StoredProcedure;
                    dbHandle.con.Open();
                   
                    Session["eventId"] = EventId;
                    dbHandle.con.Close();
                    return Json(new { id=1}, JsonRequestBehavior.AllowGet);
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
        public JsonResult Groupdata(string GroupId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand groupsession = new SqlCommand("USPGroupsession", dbHandle.con))
                {
                    groupsession.CommandType = CommandType.StoredProcedure;
                    dbHandle.con.Open();
                    Session["groupId"] = GroupId;
                    dbHandle.con.Close();
                    return Json(new { id=1}, JsonRequestBehavior.AllowGet);
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


        public ActionResult EventDetails()
        {
            if (Session["companyid"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("LoginUser", "UserLogin");
            }
            
        }
        public ActionResult GroupDetails()
        {
          
            if (Session["groupId"] == null && Session["companyid"] ==null)
            {

                return RedirectToAction("LoginUser", "UserLogin");
            }
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand showgrpost = new SqlCommand("USPShowGroupPost", dbHandle.con))
                {
                    showgrpost.Connection = dbHandle.con;
                    showgrpost.CommandType = CommandType.StoredProcedure;
                    showgrpost.Parameters.AddWithValue("@CompanyId", Session["companyid"]);
                    showgrpost.Parameters.AddWithValue("@GroupId", Session["groupId"]);
                    dbHandle.con.Open();
                    using (SqlDataReader comList = showgrpost.ExecuteReader())
                    {
                        DataTable comTable = new DataTable();
                        comTable.Load(comList);
                        dbHandle.con.Close();
                        return View(comTable);
                    }
                  
                }
              
            }
            catch (Exception e)
            {

                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                ViewBag.name = Session["name"];
                return RedirectToAction("Error_View", "CompanyRegister");
            }
            finally
            {
                Dispose();

            }

           
           
        }

        public JsonResult EventProfile()
        { 
            try
            {
              
                if (Session["companyid"] != null)
                {
                    DbConnection dbHandle = new DbConnection();
                    DataTable eventProfiledataTable = new DataTable();
                    dbHandle.Connection();
                    using (SqlCommand eventprofile = new SqlCommand("USPSingleEventView", dbHandle.con))
                    {
                        eventprofile.CommandType = CommandType.StoredProcedure;
                        eventprofile.Parameters.AddWithValue("@CompanyId", Session["companyid"]);
                        eventprofile.Parameters.AddWithValue("@EventId", Session["eventId"]);
                        dbHandle.con.Open();
                        SqlDataReader reader = eventprofile.ExecuteReader();
                        eventProfiledataTable.Load(reader);
                        dbHandle.con.Close();
                        return Json(JsonConvert.SerializeObject(eventProfiledataTable), JsonRequestBehavior.AllowGet);
                    }
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
    }
}