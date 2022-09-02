using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagentoIntegrationService
{
    public class ClsGeneral
    {

       
        public static string Interval
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["Interval"]; }
        }

        public static string FocusUser_Name
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["FocusUser_Name"]; }
        }

        public static string FocusPassword
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["FocusPassword"]; }
        }
        public static string FocusAPIServer_IP
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["FocusAPIServer_IP"]; }
        }
        public static string CompanyCode
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["CompanyCode"]; }
        }

        public static string Magento_Username
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["UserName"]; }
        }

        public static string Magento_Password
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["password"]; }
        }

        public static string Magento_BaseURL
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["BaseURL"]; }
        }
        public static string Magento_Order
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["OrderUrl"]; }
        }
        //screenName

        public static string Focus_screenName
        {
            get { return System.Configuration.ConfigurationSettings.AppSettings["screenName"]; }
        }
        public static void SetLog(string content)
        {
            //set up a filestream
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory.ToString() + @"MagentoGetordersLog.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //set up a streamwriter for adding text
            StreamWriter sw = new StreamWriter(fs);
            //find the end of the underlying filestream
            sw.BaseStream.Seek(0, SeekOrigin.End);
            //add the text
            //sw.NewLine="s";
            sw.WriteLine(DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString() + " - " + content);
            //add the text to the underlying filestream
            sw.Flush();
            //close the writer
            sw.Close();
        }
    }
}
