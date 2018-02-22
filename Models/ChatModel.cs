using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;

using NuNetwork.Models;
using System.Configuration;

namespace NuNetwork.Models
{

    public class ChatModel
    {
        public class ChatMessage
        {

            public string UserName { get; set; }

            public string Message { get; set; }

        }
        public static void PersonalChatDb(int MsgFrom,int MsgTo,string message,int CompanyId)
        {
            SqlConnection con = null;
            try
            {
               
                string constr = ConfigurationManager.ConnectionStrings["ConNew"].ConnectionString;
                con = new SqlConnection(constr);

                using (SqlCommand cmd = new SqlCommand("USPPersonalChat", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = CompanyId;
                    cmd.Parameters.Add("@Message", SqlDbType.VarChar).Value = message;
                    cmd.Parameters.Add("@UserIdFrom", SqlDbType.Int).Value = MsgFrom;
                    cmd.Parameters.Add("@UserIdTo", SqlDbType.Int).Value = MsgTo;

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, "no Ip : error in personal chat model");
            }
            finally
            {
               
                con.Dispose();
            }

        }
        public static void GroupChatDb(int UserId, int GroupId, string message, int CompanyId)
        {
            SqlConnection con = null;
            try
            {
                //DbConnection dbHandle = new DbConnection();
                string constr = ConfigurationManager.ConnectionStrings["ConNew"].ConnectionString;

                con = new SqlConnection(constr);
                using (SqlCommand cmd = new SqlCommand("USPGroupChatMessage", con))
                {
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@companyId", SqlDbType.Int).Value = CompanyId;
                    cmd.Parameters.Add("@message", SqlDbType.VarChar).Value = message;
                    cmd.Parameters.Add("@userid", SqlDbType.Int).Value = UserId;
                    cmd.Parameters.Add("@groupid", SqlDbType.Int).Value = GroupId;

                    cmd.ExecuteNonQuery();
                    con.Close();

                }
             
            }
            catch (Exception e)
            {
                ExceptionLog.Log(e, "no Ip : error in Group chat model");
            }
            finally
            {
               
                con.Dispose();
            }

        }

    }

}