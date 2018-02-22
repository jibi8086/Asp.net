using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
namespace NuNetwork.Models
{
    public class ExceptionLog
    {
      
        public static void Log(Exception exceptionMessage, string clientIp)
        {
            string filename;
            StreamWriter sw=null;
            try
            {

                 filename = HostingEnvironment.ApplicationPhysicalPath + "Files\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
              
                sw = new StreamWriter(filename, true);
                sw.WriteLine("\n  Date and time :" + DateTime.Now.ToString() + "\n  ip_address : " + clientIp + "\n  Exception : " + exceptionMessage + " \n \n ");
                sw.Close();
            }
            catch
            {
                 filename = HostingEnvironment.ApplicationPhysicalPath + "Files\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt";
               
                sw = new StreamWriter(filename, true);
                sw.WriteLine("\n  Date and time :" + DateTime.Now.ToString() + "\n  ip_address : " + clientIp + "\n  Exception : " + exceptionMessage + " \n \n ");
               
            }
            finally
            {
                sw.Close();
                sw.Dispose();
            }

        }

        //internal static void Log(Exception e)
        //{
        //    throw new NotImplementedException();
        //}
    }
}