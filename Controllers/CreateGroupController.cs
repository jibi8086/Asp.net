using Newtonsoft.Json;
using NuNetwork.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NuNetwork.Controllers
{
    public class CreateGroupController : Controller
    {
       
        // GET: CreateGroup
        public ActionResult CreateGroup()
        {
            return View();
        }
        [HttpPost]
        public JsonResult InsertGroupPost()
        {
            string fileName = "";
            string randomNo = "";
            string path = "";
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                string PostMessages = System.Web.HttpContext.Current.Request.Form["postmessage"];
             
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase filebase =Request.Files[i];
                        randomNo = System.DateTime.Now.ToString("ddMMyyhhmmss");
                        fileName = randomNo + Path.GetFileName(filebase.FileName);
                        path = path + "../Images/" + fileName +',';
                        filebase.SaveAs(Server.MapPath("../Images/") + fileName);

                       
                    }
                }

                using (SqlCommand creategroupost = new SqlCommand("USPCreateGroupPost", dbHandle.con))
                {

                    creategroupost.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@PostId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    creategroupost.Parameters.Add(parm);
                    creategroupost.Parameters.Add("@FK_GroupId", SqlDbType.Int).Value = Session["groupId"];
                    creategroupost.Parameters.Add("@GroupPostMessage", SqlDbType.VarChar).Value = PostMessages;
                    creategroupost.Parameters.Add("@FK_GroupMemberId", SqlDbType.Int).Value = Session["userid"];
                    creategroupost.Parameters.Add("@GroupPostUrl", SqlDbType.VarChar).Value = path;
                    creategroupost.Parameters.Add("@FK_GroupPostType", SqlDbType.Int).Value = 1;
                    creategroupost.Parameters.Add("@Fk_CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    dbHandle.con.Open();
                    creategroupost.ExecuteNonQuery();
                    int flag = Convert.ToInt32(parm.Value);
                    Session["grouppostid"] = flag;
                    dbHandle.con.Close();
                   
                }
                return Json(new { id=1}, JsonRequestBehavior.AllowGet);

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
        public JsonResult InsertComment(string CommentText, string PostId)
        {
            
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand groupcommentpost = new SqlCommand("USPGroupCommentPosting", dbHandle.con))
                {
                    groupcommentpost.CommandType = CommandType.StoredProcedure;
                    groupcommentpost.Parameters.Add("GroupPostId", SqlDbType.Int).Value = Convert.ToInt32(PostId);
                    groupcommentpost.Parameters.Add("@Comment", SqlDbType.VarChar).Value = CommentText;
                    groupcommentpost.Parameters.Add("@CommentedUserId", SqlDbType.Int).Value = Session["userid"];
                    groupcommentpost.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    groupcommentpost.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    dbHandle.con.Open();
                    groupcommentpost.ExecuteNonQuery();
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

        [HttpPost]
        public JsonResult ShowProfileComment(string Postid)
        {
           
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable dataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand shwgrpostcmt = new SqlCommand("USPShowGroupPostComment", dbHandle.con))
                {
                    shwgrpostcmt.CommandType = CommandType.StoredProcedure;
                    shwgrpostcmt.Parameters.Add("@PostId", SqlDbType.Int).Value = Postid;
                    shwgrpostcmt.Parameters.AddWithValue("@GroupId", Session["groupId"]);
                    dbHandle.con.Open();

                    SqlDataReader reader = shwgrpostcmt.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
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
        public JsonResult ShowProfileReplay(string commentid)
        {
            
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable dataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand userpostcomment = new SqlCommand("UspShowGroupReplay", dbHandle.con))
                {
                    userpostcomment.CommandType = CommandType.StoredProcedure;
                    userpostcomment.Parameters.Add("@CommentId", SqlDbType.Int).Value = commentid;
                    userpostcomment.Parameters.AddWithValue("@userid", Session["userid"]);
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
                return Json(new { id = 404 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
         
        }
        [HttpPost]
        public JsonResult Insertlike(string PostId)
        {
           
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand insertlike = new SqlCommand("USPGroupLike", dbHandle.con))
                {
                    insertlike.CommandType = CommandType.StoredProcedure;
                    insertlike.Parameters.Add("@FK_GroupId", SqlDbType.Int).Value = Session["groupId"];
                    insertlike.Parameters.Add("@FK_PostId", SqlDbType.Int).Value = Convert.ToInt32(PostId);
                    insertlike.Parameters.Add("@FK_UserId", SqlDbType.Int).Value = Session["userid"];
                    insertlike.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    insertlike.Parameters.Add("@Operation", SqlDbType.Int).Value = 1;
                    dbHandle.con.Open();
                    insertlike.ExecuteNonQuery();
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
        [HttpPost]
        public JsonResult Insertdislike(string PostId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand groupdislike = new SqlCommand("USPGroupLike", dbHandle.con))
                {
                    groupdislike.CommandType = CommandType.StoredProcedure;
                    groupdislike.Parameters.Add("@FK_GroupId", SqlDbType.Int).Value = Session["groupId"];
                    groupdislike.Parameters.Add("@FK_PostId", SqlDbType.Int).Value = Convert.ToInt32(PostId);
                    groupdislike.Parameters.Add("@FK_UserId", SqlDbType.Int).Value = Session["userid"];
                    groupdislike.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    groupdislike.Parameters.Add("@Operation", SqlDbType.Int).Value = 2;
                    dbHandle.con.Open();
                    groupdislike.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(new { id=1}, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id=404}, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
            
        }
        //by sanal
        [HttpPost]
        public JsonResult GroupDetails()
        {
            
            try
            {
                DataTable dataTable = new DataTable();
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand singlegroupview = new SqlCommand("USPSingleGroup", dbHandle.con))
                {
                    singlegroupview.CommandType = CommandType.StoredProcedure;
                    singlegroupview.Parameters.AddWithValue("@GroupId", Session["groupId"]);
                    dbHandle.con.Open();
                    SqlDataReader reader = singlegroupview.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id=404}, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
           
        }
        [HttpPost]
        public JsonResult InsertReplay(string CommentId, string CommentText)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand commentpost = new SqlCommand("USPGroupReplay", dbHandle.con))
                {
                    dbHandle.con.Open();
                    commentpost.CommandType = CommandType.StoredProcedure;
                    commentpost.Parameters.Add("@CommentedId", SqlDbType.Int).Value = Convert.ToInt32(CommentId);
                    commentpost.Parameters.Add("@Replay", SqlDbType.VarChar).Value = CommentText;
                    commentpost.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    commentpost.Parameters.Add("@CompanyCode", SqlDbType.Int).Value = Session["companyid"];
                    commentpost.Parameters.Add("@groupid", SqlDbType.Int).Value = Session["groupId"];
                    commentpost.ExecuteNonQuery();
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
       

        [HttpPost]
        public JsonResult GroupPost()
        {
            try
            {
                DataTable groupTable, groupImageTable;
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand answer = new SqlCommand("USPShowGroupPost", dbHandle.con))
                {
                    dbHandle.con.Open();
                    groupTable = new DataTable();
                    answer.CommandType = CommandType.StoredProcedure;
                    answer.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    answer.Parameters.Add("@groupid", SqlDbType.Int).Value = Session["groupId"];
                    SqlDataReader rd = answer.ExecuteReader();
                    groupTable.Load(rd);
                    dbHandle.con.Close();                
                }
                using (SqlCommand getImagecmd = new SqlCommand("USPGroupPostImages", dbHandle.con))
                {
                    dbHandle.con.Open();
                    getImagecmd.CommandType = CommandType.StoredProcedure;
                    getImagecmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    groupImageTable = new DataTable();
                    SqlDataReader readerImage = getImagecmd.ExecuteReader();
                    groupImageTable.Load(readerImage);

                }

                return Json(new { groupList = JsonConvert.SerializeObject(groupTable), groupImages = JsonConvert.SerializeObject(groupImageTable) }, JsonRequestBehavior.AllowGet);
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
    
