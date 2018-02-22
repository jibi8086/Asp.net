using NuNetwork.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Configuration;
using System.Configuration;
using NuNetwork.Hubs;
using Newtonsoft.Json;
using System.IO;

namespace NuNetwork.Controllers
{
    public class CreatePostController : Controller
    {
       
        // GET: CreatePost
        public ActionResult CreatePost()
        {
                try
                {

                   if (Session["companyid"] != null && Session["userid"] != null)
                   {
                    DbConnection dbHandle = new DbConnection();
                    dbHandle.Connection();
                    using (SqlCommand showallpost = new SqlCommand("USPShowAllPost", dbHandle.con))
                    {
                        showallpost.Connection = dbHandle.con;
                        showallpost.CommandType = CommandType.StoredProcedure;
                        showallpost.Parameters.AddWithValue("@CompanyId", Session["companyid"]);
                        dbHandle.con.Open();
                        using (SqlDataReader comList = showallpost.ExecuteReader())
                        {
                            DataTable comTable = new DataTable();
                            comTable.Load(comList);
                            return View(comTable);
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
                }
                finally
                {
                    Dispose();

                }
            
             ViewBag.name = Session["name"];
            return View();
        }
        [HttpPost]
        public JsonResult InsertPost()
        {
            string Imagepath = "";
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
             
                string PostMessages = System.Web.HttpContext.Current.Request.Form["postmessage"];
                if (Request.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase filebase = Request.Files[i];
                        string randomNo = System.DateTime.Now.ToString("ddMMyyhhmmss");
                        string fileName = randomNo + Path.GetFileName(filebase.FileName);
                        Imagepath = Imagepath + "../Images/" + fileName + ',';
                        filebase.SaveAs(Server.MapPath("../Images/") + fileName);


                    }
                }
             

                using (SqlCommand createpost = new SqlCommand("USPInsertPost", dbHandle.con))
                {

                    createpost.CommandType = CommandType.StoredProcedure;
                    SqlParameter parm = new SqlParameter("@PostId", SqlDbType.Int);
                    parm.Direction = ParameterDirection.ReturnValue;
                    createpost.Parameters.Add(parm);
                    createpost.Parameters.Add("@PostMessage", SqlDbType.VarChar).Value = PostMessages;
                    createpost.Parameters.Add("@FK_UserId", SqlDbType.Int).Value = Session["userid"];
                    createpost.Parameters.Add("@PostUrl", SqlDbType.VarChar).Value = Imagepath;
                    if (Imagepath == "")
                        createpost.Parameters.Add("@FK_PostTypeId", SqlDbType.Int).Value = 3;
                    else
                        createpost.Parameters.Add("@FK_PostTypeId", SqlDbType.Int).Value = 1;
                    createpost.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    dbHandle.con.Open();
                    createpost.ExecuteNonQuery();
                    int flag = Convert.ToInt32(parm.Value);
                    Session["postid"] = flag;
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
        public JsonResult InsertComment(string CommentText, string PostId)
        {   
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand commentpost = new SqlCommand("USPCommentPosting", dbHandle.con))
                {
                    commentpost.CommandType = CommandType.StoredProcedure;
                    commentpost.Parameters.Add("@PostId", SqlDbType.Int).Value = Convert.ToInt32(PostId);
                    commentpost.Parameters.Add("@Comment", SqlDbType.VarChar).Value = CommentText;
                    commentpost.Parameters.Add("@CommentedUserId", SqlDbType.Int).Value = Session["userid"];
                    commentpost.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    dbHandle.con.Open();
                    commentpost.ExecuteNonQuery();
                    dbHandle.con.Close();
                    return Json(new { id = 1 },JsonRequestBehavior.AllowGet);
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
        public JsonResult InsertReplay(string CommentId, string CommentText)
        {
           
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand commentpost = new SqlCommand("USPReplies", dbHandle.con))
                {
                    dbHandle.con.Open();
                    commentpost.CommandType = CommandType.StoredProcedure;
                    commentpost.Parameters.Add("@CommentedId", SqlDbType.Int).Value = Convert.ToInt32(CommentId);
                    commentpost.Parameters.Add("@Replay", SqlDbType.VarChar).Value = CommentText;
                    commentpost.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    commentpost.Parameters.Add("@CompanyCode", SqlDbType.Int).Value = Session["companyid"];
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
        public JsonResult ShowComment(string showPost)
        {
           
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable showCommentdataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand viewcmt = new SqlCommand("USPShowComment", dbHandle.con))
                {
                    viewcmt.CommandType = CommandType.StoredProcedure;
                    viewcmt.Parameters.Add("@PostId", SqlDbType.Int).Value = Convert.ToInt32(showPost);
                    dbHandle.con.Open();

                    SqlDataReader reader = viewcmt.ExecuteReader();
                    showCommentdataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(showCommentdataTable), JsonRequestBehavior.AllowGet);
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

        public JsonResult validateControls(string mailidId)
        {
            return Json(new { });
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

        [HttpPost]
        public JsonResult ProfileDetails()
        {
            
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable profileDetailsdataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand profilepost = new SqlCommand("USPProfilePost", dbHandle.con))
                {
                    profilepost.CommandType = CommandType.StoredProcedure;

                    profilepost.Parameters.AddWithValue("@UserId", Session["userid"]);
                    profilepost.Parameters.AddWithValue("@CompanyId", Session["companyid"]);

                    dbHandle.con.Open();

                    SqlDataReader reader = profilepost.ExecuteReader();
                    profileDetailsdataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(profileDetailsdataTable), JsonRequestBehavior.AllowGet);
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
                using (SqlCommand insertlike = new SqlCommand("USPLike", dbHandle.con))
                {
                    insertlike.CommandType = CommandType.StoredProcedure;
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
        public JsonResult DeletePost(string PostId)
        {
            
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand deletePost = new SqlCommand("USPUpdatePost", dbHandle.con))
                {
                    deletePost.CommandType = CommandType.StoredProcedure;
                    deletePost.Parameters.Add("@PostId", SqlDbType.Int).Value = Convert.ToInt32(PostId);
                    deletePost.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["userid"];
                    dbHandle.con.Open();
                    deletePost.ExecuteNonQuery();
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
        [HttpPost]
        public JsonResult Insertdislike(string PostId)
        {
           
            try
            {
                DbConnection dbHandle = new DbConnection();
                dbHandle.Connection();
                using (SqlCommand insertdislike = new SqlCommand("USPLike", dbHandle.con))
                {
                    insertdislike.CommandType = CommandType.StoredProcedure;
                    insertdislike.Parameters.Add("@FK_PostId", SqlDbType.Int).Value = Convert.ToInt32(PostId);
                    insertdislike.Parameters.Add("@FK_UserId", SqlDbType.Int).Value = Session["userid"];
                    insertdislike.Parameters.Add("@CompanyId", SqlDbType.Int).Value = Session["companyid"];
                    insertdislike.Parameters.Add("@operation", SqlDbType.Int).Value = 2;
                    dbHandle.con.Open();
                    insertdislike.ExecuteNonQuery();
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
        public JsonResult ShowProfileReplays(string commentid)
        {  
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable showProfileReplydataTable = new DataTable();
                dbHandle.Connection();
                using (SqlCommand userpostcomment = new SqlCommand("USPShowReplay", dbHandle.con))
                {
                    userpostcomment.CommandType = CommandType.StoredProcedure;
                    userpostcomment.Parameters.Add("@CommentId", SqlDbType.Int).Value = commentid;
                    userpostcomment.Parameters.AddWithValue("@userid", Session["userid"]);
                    dbHandle.con.Open();
                    SqlDataReader reader = userpostcomment.ExecuteReader();
                    showProfileReplydataTable.Load(reader);
                    dbHandle.con.Close();
                    return Json(JsonConvert.SerializeObject(showProfileReplydataTable), JsonRequestBehavior.AllowGet);
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
        public JsonResult showAllPost()
        {  
            try
            {
                DbConnection dbHandle = new DbConnection();
                DataTable postTable, postImageTable;
                dbHandle.Connection();
                using (SqlCommand shwpostcmt = new SqlCommand("USPShowAllPost", dbHandle.con))
                {
                    postTable = new DataTable();
                    shwpostcmt.CommandType = CommandType.StoredProcedure;
                    shwpostcmt.Parameters.AddWithValue("@CompanyId", Session["companyid"]);
                    dbHandle.con.Open();
                    SqlDataReader readerPost = shwpostcmt.ExecuteReader();
                    postTable.Load(readerPost);
                    dbHandle.con.Close();
                    
                }
                using (SqlCommand shwpostImgcmt = new SqlCommand("USPPostImageView", dbHandle.con))
                {
                    postImageTable = new DataTable();
                    shwpostImgcmt.CommandType = CommandType.StoredProcedure;
                    shwpostImgcmt.Parameters.AddWithValue("@CompanyId", Session["companyid"]);
                    dbHandle.con.Open();
                    SqlDataReader readerPostImg = shwpostImgcmt.ExecuteReader();
                    postImageTable.Load(readerPostImg);
                    dbHandle.con.Close();

                }
                return Json(new { postList = JsonConvert.SerializeObject(postTable), postImages = JsonConvert.SerializeObject(postImageTable) }, JsonRequestBehavior.AllowGet);

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

    }
}