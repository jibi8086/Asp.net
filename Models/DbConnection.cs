using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace NuNetwork.Models
{
    public class DbConnection
    {
        public SqlConnection con;

        public void Connection()
        {
                   
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["ConNew"].ConnectionString;
                con = new SqlConnection(constr);
               
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
          
        }
    }
}