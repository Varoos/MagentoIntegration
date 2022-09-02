using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagentoIntegrationService
{
    public partial class MagentoGetOrders : Form
    {

        int second = 0;
        int interval = 2;
        string sessionId = "";
        

        string ccode = ClsGeneral.CompanyCode;

      
        public MagentoGetOrders()
        {
            InitializeComponent();
        }

        private void MagentoGetOrders_Load(object sender, EventArgs e)
        {
            Process p = Process.GetCurrentProcess();
            Process[] pname = Process.GetProcessesByName(p.ProcessName);
            ClsGeneral.SetLog("[" + System.DateTime.Now + "]  : Process Name : " + p.ProcessName);
            ClsGeneral.SetLog("[" + System.DateTime.Now + "]  : Process Count : " + pname.Length);
            if (pname.Length > 1)
            {
                MessageBox.Show("MagentokGetOrders Service already running");
                timer1.Stop();
                timer1.Dispose();

                Process[] workers = Process.GetProcessesByName(p.ProcessName);
                foreach (Process worker in workers)
                {
                    worker.Kill();
                    worker.WaitForExit();
                    worker.Dispose();
                    break;
                }

                this.Close();
                return;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            int SleepTime = 0;
            second = second + 1;
            DateTime currentTime = DateTime.Now;
            DateTime x30MinsLater = currentTime.AddMinutes(Convert.ToInt32(ClsGeneral.Interval));

            if (Convert.ToInt32(ClsGeneral.Interval) >= 10)
            { SleepTime = Convert.ToInt32(ClsGeneral.Interval); }
            else
            { SleepTime = 10; }

            if (second >= interval)
            {
                second = 0;

                GetOrders();

                Thread.Sleep(SleepTime * 60 * 1000);

            }
            else
            {
                this.Hide();
            }
        }

        private void GetOrders()
        {
            string token = GetToken();
            Clsdata.LogFile("MagentoOrderService", DateTime.Now + " Get order service started Toke:" + token);
            var str = JsonConvert.DeserializeObject<string>(token);
            //var str = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(token).ToString());
            string authtoken = "Bearer " + str;

            var client = new RestClient("https://www.dme-medical.com/rest/V1/orders?searchCriteria=0");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", authtoken);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Execute(request);
            
            var responseData = JsonConvert.DeserializeObject<ClsorderResponse.Root>(response.Content);

            Clsdata.LogFile("MagentoOrderService", DateTime.Now + " Get order service data:" + responseData.items.Count);
            if (responseData.items.Count > 0)
            {
                postsalesorder(responseData);
            }
        }
        public void postsalesorder(ClsorderResponse.Root objdata)
        {
            string ordid = "";
            int ivouchertype = 0;
            try
            {
                Clsdata cls = new Clsdata();
                int compid = cls.GetCompanyId(ccode);
                DataSet ds1 = Clsdata.GetData("select * from ccore_vouchers_0 where sname = '" + ClsGeneral.Focus_screenName + "'",compid);
                if (ds1.Tables[0].Rows.Count>0)
                {
                    ivouchertype = Convert.ToInt32(ds1.Tables[0].Rows[0]["ivouchertype"]);
                }

                
               
                


                for (int k = 0; k < objdata.items.Count; k++)
                {
                    if (objdata.items[k].status.ToUpper()!="PENDING" )
                    {
                        goto lst;
                    }
                     ds1 = Clsdata.GetData("select sNarration from tcore_headerdata" + ivouchertype + "_0 where sNarration='" + objdata.items[k].items[0].order_id + "'", compid);
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        goto lst;
                    }

                    double Qty = 0;
                    double rate = 0;
                    double gross = 0;
                    int sactid = 0;
                    int prodid = 0;

                    DataSet ds = Clsdata.GetData("select sname,imasterid from mcore_Account where sname='" + objdata.items[k].customer_firstname + "' and istatus<>5 ", compid);
                    if (ds.Tables[0].Rows.Count <= 0)
                    {
                        sactid = createAccount(objdata.items[k].customer_firstname, 3);
                    }
                    else
                    {
                        sactid = Convert.ToInt32(ds.Tables[0].Rows[0]["imasterid"]);
                    }
                   

                    string createdate = objdata.items[k].created_at.Substring(0, 10);
                    string[] str = createdate.Split('-');

                    // int docdate =0;

                    DateTime dt = new DateTime(Convert.ToInt32(str[0]), Convert.ToInt32(str[1]), Convert.ToInt32(str[2]));
                    int docdate = cls.GetDateToInt(dt);
                    ordid =  Convert.ToString(objdata.items[k].order_id);
                    Hashtable header = new Hashtable
                {                    
                    { "DocNo",objdata.items[k].order_id },
                    { "Date",docdate  },
                    {"CustomerAC__Id",sactid},
                    {"Currency__Code",objdata.items[k].order_currency_code},
                    {"ExchangeRate",objdata.items[k].store_to_order_rate },
                    {"sNarration",  objdata.items[k].items[0].order_id },


                };
                    List<Hashtable> body = new List<Hashtable>();
                    Hashtable row = new Hashtable { };
                    for (int j = 0; j < objdata.items[k].items.Count; j++)
                    {
                        ds = Clsdata.GetData("select sname,imasterid from mcore_product where scode='" + objdata.items[k].items[j].sku + "' and istatus<>5 ", compid);
                        if (ds.Tables[0].Rows.Count <= 0)
                        {
                            prodid = createProduct(objdata.items[k].items[j].sku);
                        }
                        else
                        {
                            prodid = Convert.ToInt32(ds.Tables[0].Rows[0]["imasterid"]);
                        }

                        Qty = Convert.ToDouble(objdata.items[k].items[j].qty_ordered);
                        rate = Convert.ToDouble(objdata.items[k].items[j].price);
                        gross = Qty * rate;
                        row = new Hashtable
                    {
                        {"Item__Id",prodid },
                       { "Quantity", Qty },
                        { "Rate", rate },
                         { "Gross", gross },

                    };
                        body.Add(row);

                    }
                    var postingData = new ClsProperties.PostingData();
                    postingData.data.Add(new Hashtable { { "Header", header }, { "Body", body } });

                    string sContent = JsonConvert.SerializeObject(postingData);
                    Clsdata.LogFile("MagentoOrderService", DateTime.Now + " invoice JSon:" + sContent);
                    string err = "";
                    sessionId = getsessionid(ClsGeneral.FocusUser_Name, ClsGeneral.FocusPassword, ClsGeneral.CompanyCode);
                    string url = "http://" + ClsGeneral.FocusAPIServer_IP + "/Focus8API/Transactions/Vouchers/"+ClsGeneral.Focus_screenName;
                    string error = "";
                    var response = Post(url, sContent, sessionId, ref error);
                    //using (WebClient client = new WebClient())
                    //{
                    //    client.Headers.Add("Content-Type", "application/json");
                    //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    //    //client.Timeout = 10 * 60 * 1000;
                    //    var arrResponse = client.UploadString(url, sContent);

                    //    Clsdata.LogFile("RightAngleInvoice", DateTime.Now + "Focus invoice Response:" + response);
                    //}
                    if (response != null)
                    {
                        var responseData = JsonConvert.DeserializeObject<ClsProperties.PostResponse>(response);

                        Clsdata.LogFile("MagentoOrderService", DateTime.Now + " invoice JSon response:" + response);
                        if (responseData.result != -1)
                        {
                            Clsdata.LogFile("MagentoOrderService", DateTime.Now + " sales order posted success fully order No:" + responseData.data[0]["VoucherNo"]);

                            bool flg = logout(sessionId, ClsGeneral.FocusAPIServer_IP);
                        }
                        else
                        {
                            Clsdata.LogFile("MagentoOrderService", DateTime.Now + " sales order posting failed:" + responseData.message);
                            bool flg = logout(sessionId, ClsGeneral.FocusAPIServer_IP);
                        }
                    }


                    lst:;

                }
            }
            catch(Exception ex)
            {
                Clsdata.LogFile("MagentoOrderService", DateTime.Now + " sales order posting exception:" + ex.Message +" order id :"+ordid);
            }
        }

        public static string Post(string url, string data, string sessionId, ref string err)
        {
            try
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("fSessionId", sessionId);
                    client.Headers.Add("Content-Type", "application/json");
                    var response = client.UploadString(url, data);
                    return response;
                }
            }
            catch (Exception e)
            {
                err = e.Message;
                return null;
            }

        }

        public int createAccount(string name, int type)
        {
            int id = 0;
            Hashtable master = new Hashtable();
            master = new Hashtable
                        {

                            { "sName", name},
                            { "sCode",name },
                            { "iAccountType", type}
                        };
            ClsProperties.PostingData2 objHashMaster = new ClsProperties.PostingData2();
            objHashMaster.data.Add(master);
            ClsProperties.HashData objHashResponse = new ClsProperties.HashData();

            string sContent = JsonConvert.SerializeObject(objHashMaster);
            sessionId = getsessionid(ClsGeneral.FocusUser_Name, ClsGeneral.FocusPassword, ClsGeneral.CompanyCode);
            using (WebClient client = new WebClient())

            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add("fSessionId", sessionId);
                client.Headers.Add("Content-Type", "application/json");
                //client.Timeout = 600 * 60 * 1000;

                var response = client.UploadString("http://" + ClsGeneral.FocusAPIServer_IP + "/Focus8API/Masters/Core__Account", sContent);
                objHashResponse = JsonConvert.DeserializeObject<ClsProperties.HashData>(response);
                Clsdata.LogFile("MagentoOrderService", DateTime.Now + "Focus Account Creation:" + objHashResponse);

                if (objHashResponse.result != -1)
                {
                    id = Convert.ToInt32(objHashResponse.data[0]["MasterId"]);
                    bool flg = logout(sessionId, ClsGeneral.FocusAPIServer_IP);
                }
                else
                {
                    bool flg = logout(sessionId, ClsGeneral.FocusAPIServer_IP);
                    //Clsdata.LogFile("RightAngleInvoice", DateTime.Now + "Focus invoice Response:" + response);
                }
            }

            return id;
        }
        public bool logout(string sessionid, string serverip)
        {
            bool flg = false;
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("fsessionid", sessionid);
                //client.Timeout = 10 * 60 * 1000;
                client.Headers.Add("Content-Type", "application/json");
                var arrResponse = client.DownloadString("http://" + ClsGeneral.FocusAPIServer_IP + "/focus8API/Logout");
                flg = true;
            }
            return flg;
        }
        public int createProduct(string name)
        {
            int id = 0;
            Hashtable master = new Hashtable();
            master = new Hashtable
                        {

                            { "sName", name},
                            { "sCode",name },
                            { "iProductType", 4}
                        };

            ClsProperties.PostingData2 objHashMaster = new ClsProperties.PostingData2();
            objHashMaster.data.Add(master);
            ClsProperties.HashData objHashResponse = new ClsProperties.HashData();
            sessionId = getsessionid(ClsGeneral.FocusUser_Name, ClsGeneral.FocusPassword, ClsGeneral.CompanyCode);
            string sContent = JsonConvert.SerializeObject(objHashMaster);
            using (WebClient client = new WebClient())

            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add("fSessionId", sessionId);
                client.Headers.Add("Content-Type", "application/json");
                //client.Timeout = 600 * 60 * 1000;

                var response = client.UploadString("http://" + ClsGeneral.FocusAPIServer_IP + "/Focus8API/Masters/Core__Product", sContent);
                objHashResponse = JsonConvert.DeserializeObject<ClsProperties.HashData>(response);
                Clsdata.LogFile("MagentoOrderService", DateTime.Now + "Focus Product Creation:" + objHashResponse);

                if (objHashResponse.result != -1)
                {
                    id = Convert.ToInt32(objHashResponse.data[0]["MasterId"]);
                    bool flg = logout(sessionId, ClsGeneral.FocusAPIServer_IP);
                }
                else
                {
                    bool flg = logout(sessionId, ClsGeneral.FocusAPIServer_IP);
                    //Clsdata.LogFile("RightAngleInvoice", DateTime.Now + "Focus invoice Response:" + response);
                }
            }

            return id;
        }

        public string getsessionid(string usrename, string password, string companycode)
        {
            string sid = "";
            ClsProperties.Datum datanum = new ClsProperties.Datum();
            datanum.CompanyCode = companycode;
            datanum.Username = usrename;
            datanum.password = password;
            List<ClsProperties.Datum> lstd = new List<ClsProperties.Datum>();
            lstd.Add(datanum);
            ClsProperties.Lolgin lngdata = new ClsProperties.Lolgin();
            lngdata.data = lstd;
            string sContent = JsonConvert.SerializeObject(lngdata);
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                //client.Timeout = 10 * 60 * 1000;
                var arrResponse = client.UploadString("http://" + ClsGeneral.FocusAPIServer_IP + "/focus8API/Login", sContent);
                //returnObject = new clsDeserialize().Deserialize<RootObject>(arrResponse);
                ClsProperties.Resultlogin lng = JsonConvert.DeserializeObject<ClsProperties.Resultlogin>(arrResponse);

                sid = lng.data[0].fSessionId;


            }

            return sid;
        }
        public string GetToken()
        {


            string Token = "";

            var client = new RestClient(ClsGeneral.Magento_BaseURL + "/rest/V1/integration/admin/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            ClsProperties.Gettoken cls = new ClsProperties.Gettoken();
            cls.username = ClsGeneral.Magento_Username;
            cls.password = ClsGeneral.Magento_Password;
            string sContent = JsonConvert.SerializeObject(cls);

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", sContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Token = response.Content;


            return Token;
        }


    }
}
