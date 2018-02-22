using Newtonsoft.Json;
using NuNetwork.Hubs;
using NuNetwork.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuNetwork.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
      
        public ActionResult Notification()
        {
            return View();
        }
        public JsonResult GetNotification()
        {
            IEnumerable<Messages> list = GetAllMessages();
            try
            {
                return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                    return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            finally
            {
                Dispose();
            }

        }
        

        public IEnumerable<Messages> GetAllMessages()
        {
            
             List<Messages> messages = new List<Messages>();
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
               
                using (SqlCommand Notificationcommand = new SqlCommand("[dbo].[USPNotificationSelect]", dbHandle.con))
                    {
                    dbHandle.con.Open();
                    
                        Notificationcommand.Notification = null;
                        Notificationcommand.CommandType = CommandType.StoredProcedure;
                        Notificationcommand.Parameters.Add("@NotificationToId", SqlDbType.Int).Value = Session["userid"];
                        Notificationcommand.ExecuteNonQuery();
                        SqlDependency dependency = new SqlDependency(Notificationcommand);
                        dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                        if (dbHandle.con.State == ConnectionState.Closed)
                        dbHandle.con.Open();

                        SqlDataReader reader = Notificationcommand.ExecuteReader();

                        while (reader.Read())
                        {
                        
                            messages.Add(item:
                            new Messages
                            {
                                userId = (int)reader["NotificationToId"],
                                postId=(int)reader["PostId"],
                                viewedBy = (string)reader["NotificationBy"],
                                notificationType = (string)reader["NotificationType"],
                                notificationId = (int)reader["NotificationId"],
                            });

                        }
                    
                    dbHandle.con.Close();
                    return messages;

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
            return messages;
        }
       
        public JsonResult CheckNotification()
        {
            try
            {

                IEnumerable<Messages> list = CheckMessage();
                return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception )
            {
                //ExceptionLog.Log(ex);
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            finally
            {
                Dispose();
            }
        }
        public IEnumerable<Messages> CheckMessage()
        {          
            List<Messages> messages = new List<Messages>();
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand Notificationcommand = new SqlCommand("[dbo].[USPNotificationSelect]", dbHandle.con))
                {
                    dbHandle.con.Open();
                    Notificationcommand.Notification = null;
                    Notificationcommand.CommandType = CommandType.StoredProcedure;
                    Notificationcommand.Parameters.Add("@NotificationToId", SqlDbType.Int).Value = Session["userid"];              
                    Notificationcommand.ExecuteNonQuery();
                    SqlDependency dependency = new SqlDependency(Notificationcommand);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    if (dbHandle.con.State == ConnectionState.Closed)
                     dbHandle.con.Open();
                    SqlDataReader reader = Notificationcommand.ExecuteReader();
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["NotificationToId"]) == Convert.ToInt32(Session["userid"]))
                        {

                            messages.Add(item:
                            new Messages
                            {
                                postId = (int)reader["PostId"],
                                userId = (int)reader["NotificationToId"],
                                viewedBy = (string)reader["NotificationBy"],
                                notificationType = (string)reader["NotificationType"],
                                notificationId = (int)reader["NotificationId"],
                            });

                        }
                    }
                    dbHandle.con.Close();
                    return messages;
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
            return messages;
        }
        [HttpPost]
        public JsonResult NotificationCall()
        {


            
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();

                using (SqlCommand Notificationcommand = new SqlCommand("[dbo].[USPNotificationSelect]", dbHandle.con))
                {
                    dbHandle.con.Open();
                    Notificationcommand.Notification = null;
                    Notificationcommand.CommandType = CommandType.StoredProcedure;
                    Notificationcommand.Parameters.Add("@NotificationToId", SqlDbType.Int).Value = Session["userid"];
                    Notificationcommand.ExecuteNonQuery();
                    SqlDependency dependency = new SqlDependency(Notificationcommand);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    SqlDataReader reader = Notificationcommand.ExecuteReader();
                   
                }
                dbHandle.con.Close();
                return Json(JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception ex)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(ex, Request.UserHostAddress);

            }
            finally
            {
                Dispose();
               
            }

            return Json(JsonRequestBehavior.AllowGet); 

        }
        public JsonResult PostView()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                DataTable postTable, postImageTable;
                using (SqlCommand NotificationPost = new SqlCommand("[dbo].[USPReadStatus]", dbHandle.con))
                {

                    dbHandle.con.Open();

                    NotificationPost.CommandType = CommandType.StoredProcedure;
                    NotificationPost.Parameters.Add("@PostId", SqlDbType.Int).Value = Session["postIdView"];
                    NotificationPost.Parameters.Add("@ReadId", SqlDbType.Int).Value = Session["postReadId"];
                    NotificationPost.ExecuteNonQuery();
                    dbHandle.con.Close();
                }

                using (SqlCommand NotificationPost = new SqlCommand("[dbo].[USPPostView]", dbHandle.con))
                {
                    postTable = new DataTable();
                    dbHandle.con.Open();
                    NotificationPost.Notification = null;
                    NotificationPost.CommandType = CommandType.StoredProcedure;
                    NotificationPost.Parameters.Add("@CompanyIdSession", SqlDbType.Int).Value = Session["companyid"];
                    NotificationPost.Parameters.Add("@PostId", SqlDbType.Int).Value = Session["postIdView"];
                    SqlDataReader readerPost = NotificationPost.ExecuteReader();
                    postTable.Load(readerPost);
                    dbHandle.con.Close();
                }
                using (SqlCommand shwpostImgcmt = new SqlCommand("USPPostImage", dbHandle.con))
                {
                    postImageTable = new DataTable();
                    shwpostImgcmt.CommandType = CommandType.StoredProcedure;
                    shwpostImgcmt.Parameters.AddWithValue("@CompanyId", Session["companyid"]);
                    shwpostImgcmt.Parameters.AddWithValue("@PostId", SqlDbType.Int).Value= Session["postIdView"]; 
                    dbHandle.con.Open();
                    SqlDataReader readerPostImg = shwpostImgcmt.ExecuteReader();
                    postImageTable.Load(readerPostImg);
                    dbHandle.con.Close();

                }
                return Json(new { postList = JsonConvert.SerializeObject(postTable), postImages = JsonConvert.SerializeObject(postImageTable) }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(ex, Request.UserHostAddress);

            }
            finally
            {
                Dispose();

            }

            return Json(JsonRequestBehavior.AllowGet);

        }
        public ActionResult Post()
        {
            return View();
        }
        public void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                if (e.Type == SqlNotificationType.Change)
                {
                    MessagesHub.SendNotification();
                }

            }
            catch(Exception ex)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(ex, Request.UserHostAddress);
            }
        }
        public JsonResult PostData(string id, string notId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand groupsession = new SqlCommand("USPPostSession", dbHandle.con))
                {
                    groupsession.CommandType = CommandType.StoredProcedure;
                    dbHandle.con.Open();
                    Session["postIdView"] = id;
                    Session["postReadId"] = notId;
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

    }
}