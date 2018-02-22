using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Data;
using NuNetwork.Models;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.IO;

namespace NuNetwork.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        public ActionResult AddGroup()
        {
            try
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
        public ActionResult AddGroupMember()
        {
            try
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
        public ActionResult RemoveGroupMember()
        {
            try
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
        public ActionResult ChangeGroupProfileImage()
        {
            try
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
        //Search Employee Name
        public JsonResult SearchWithName()
        {
            try
            {
            DbConnection dbHandle = new DbConnection();
            dbHandle.Connection();
            DataTable dataTable = new DataTable();           
                using (SqlCommand searchwithnames = new SqlCommand("USPAdminAddGroupMembers", dbHandle.con))
                {
                    searchwithnames.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    searchwithnames.CommandType = CommandType.StoredProcedure;
                    searchwithnames.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    searchwithnames.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    SqlDataReader reader = searchwithnames.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new {}, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        //Search Group Member Name
        public JsonResult SearchWithNameGroupMember(string procedureName)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
            dbHandle.Connection();
            DataTable dataTable = new DataTable();
             using (SqlCommand searchbygrpmember = new SqlCommand(procedureName, dbHandle.con))
                {
                    searchbygrpmember.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    searchbygrpmember.CommandType = CommandType.StoredProcedure;
                    searchbygrpmember.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    searchbygrpmember.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    SqlDataReader reader = searchbygrpmember.ExecuteReader();
                    dataTable.Load(reader);
                    dbHandle.con.Close();
                }
                return Json(JsonConvert.SerializeObject(dataTable), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        //Crete New Group
        public JsonResult CreateGroup()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                HttpPostedFile groupPhoto = System.Web.HttpContext.Current.Request.Files["inputfile"];
                string userTockenId = System.Web.HttpContext.Current.Request.Form["tagIds"];
                string groupName = System.Web.HttpContext.Current.Request.Form["txtGroup"];
                string path = "";
                if (groupPhoto != null)
                {
                    HttpPostedFile filebase = (HttpPostedFile)(groupPhoto);
                    string Random= System.DateTime.Now.ToString("ddMMyyhhmmss");
                    string fileName = Random+Path.GetFileName(filebase.FileName);
                    path = ("../Images/") + fileName;
                    filebase.SaveAs(Server.MapPath("../Images/") + fileName);
                }
                using (SqlCommand grpCreation = new SqlCommand("USPGroupCreation", dbHandle.con))
                {
                    grpCreation.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    grpCreation.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@userId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    grpCreation.Parameters.Add(parm);
                    grpCreation.Parameters.Add("@GroupName", SqlDbType.VarChar).Value = groupName;
                    grpCreation.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userTockenId;
                    grpCreation.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Session["userid"];
                    grpCreation.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    grpCreation.Parameters.Add("@GroupImage", SqlDbType.VarChar).Value = path;
                    SqlDataAdapter da = new SqlDataAdapter(grpCreation);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    dbHandle.con.Close();
                    return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
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
        //Search group Name
        public JsonResult SearchGroup(string prefix)
        {            
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand searchGroup = new SqlCommand("USPSearchGroupName", dbHandle.con))
                {
                    searchGroup.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    searchGroup.CommandType = CommandType.StoredProcedure;
                    searchGroup.Parameters.Add("@serchValue", SqlDbType.VarChar).Value = prefix;
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(searchGroup);
                    da.Fill(ds);
                    List<search> searchlist = new List<search>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        searchlist.Add(new search
                        {
                            Name = dr["GroupName"].ToString(),
                            Sr_no = dr["GroupId"].ToString()
                        });
                    }
                    dbHandle.con.Close();
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

        //Add Group Members
        public JsonResult AddMember(string userTockenId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand addMember = new SqlCommand("USPGroupAddMember", dbHandle.con))
                {

                    addMember.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    addMember.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@userId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    addMember.Parameters.Add(parm);
                    addMember.Parameters.Add("@FK_GroupId", SqlDbType.Int).Value = Session["groupId"];
                    addMember.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userTockenId;
                    addMember.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    SqlDataAdapter da = new SqlDataAdapter(addMember);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    dbHandle.con.Close();
                    return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
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
        //Remove Group members
        public JsonResult RemoveGrpMember(string userTockenId)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand removemember = new SqlCommand("USPRemoveGroupMember", dbHandle.con))
                {

                    removemember.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    removemember.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@userId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    removemember.Parameters.Add(parm);
                    removemember.Parameters.Add("@FK_GroupId", SqlDbType.Int).Value = Session["groupId"]; ;//Session["userid"];
                    removemember.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userTockenId;
                    SqlDataAdapter da = new SqlDataAdapter(removemember);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    dbHandle.con.Close();
                    return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
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
        //Group Name Validation
        public JsonResult GroupValidation(string groupName)
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand grpvalidation = new SqlCommand("USPCheckGroupName", dbHandle.con))
                {
                    grpvalidation.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    grpvalidation.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@flag", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    grpvalidation.Parameters.Add(parm);
                    grpvalidation.Parameters.AddWithValue("@GroupName", SqlDbType.Int).Value = groupName;
                    SqlDataAdapter da = new SqlDataAdapter(grpvalidation);
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
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        //Admin Validation
        public JsonResult DeleteGroupsValidation()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand deletegroupvalidation = new SqlCommand("USPAdminDeleteGroup", dbHandle.con))
                {

                    deletegroupvalidation.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    deletegroupvalidation.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@flag", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    deletegroupvalidation.Parameters.Add(parm);
                    deletegroupvalidation.Parameters.Add("@FK_UserId", SqlDbType.Int).Value = Session["userid"];
                    deletegroupvalidation.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    SqlDataAdapter da = new SqlDataAdapter(deletegroupvalidation);
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
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }
        //Admin Delete Group
        public JsonResult DeleteGroups()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand deletegroup = new SqlCommand("USPDeleteGroup", dbHandle.con))
                {

                    deletegroup.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    deletegroup.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@userId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    deletegroup.Parameters.Add(parm);
                    deletegroup.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    deletegroup.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    SqlDataAdapter da = new SqlDataAdapter(deletegroup);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    int flag = Convert.ToInt32(parm.Value);
                    dbHandle.con.Close();
                    return Json(new { id = 1 }, JsonRequestBehavior.AllowGet);
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
        // Display Group Name In NewsFeed
        [HttpPost]
        public JsonResult GroupMemberDisplay()
        {   
            try
            {
                DataTable dataTable = new DataTable();
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand grpMemberDisplay = new SqlCommand("USPGroupMemberDisplay", dbHandle.con))
                {
                    grpMemberDisplay.CommandType = CommandType.StoredProcedure;
                    grpMemberDisplay.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    dbHandle.con.Open();
                    SqlDataReader reader = grpMemberDisplay.ExecuteReader();
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
        //Left Group
        public JsonResult RemoveFromGroup()
        {
            int flag = 2;
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand rmvFromgroup = new SqlCommand("USPRemoveFromGroup", dbHandle.con))
                {
                    rmvFromgroup.Connection = dbHandle.con;
                    dbHandle.con.Open();
                    rmvFromgroup.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@userId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    rmvFromgroup.Parameters.Add(parm);
                    rmvFromgroup.Parameters.Add("@GroupId", SqlDbType.Int).Value = Session["groupId"];
                    rmvFromgroup.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    rmvFromgroup.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    SqlDataAdapter da = new SqlDataAdapter(rmvFromgroup);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    flag = Convert.ToInt32(parm.Value);
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
        //Change Group Profile Picture
        [HttpPost]
        public JsonResult ChangeProfilePicture()
        {
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                var pic = System.Web.HttpContext.Current.Request.Files["inputfile"];
                var path = "";
                HttpPostedFile filebase = (HttpPostedFile)(pic);
                var Random = System.DateTime.Now.ToString("ddMMyyhhmmss");
                var fileName = Random + Path.GetFileName(filebase.FileName);
                path = ("../Images/") + fileName;
                filebase.SaveAs(Server.MapPath("../Images/") + fileName);
                using (SqlCommand Imagecmd = new SqlCommand("USPChangeGroupImage", dbHandle.con))
                {
                    Imagecmd.CommandType = CommandType.StoredProcedure;
                    Imagecmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    Imagecmd.Parameters.Add("@ImageUrl", SqlDbType.VarChar).Value = path;
                    Imagecmd.Parameters.Add("@CompanyCode", SqlDbType.Int).Value = Session["companyid"];
                    Imagecmd.Parameters.Add("@GroupId",SqlDbType.Int).Value= Session["groupId"];
                    dbHandle.con.Open();
                    Imagecmd.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(new { id = 1 },JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ViewBag.error = "Error!!";
                ExceptionLog.Log(e, Request.UserHostAddress);
                return Json(new { id = 0 }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                Dispose();
            }
        }


    }
}