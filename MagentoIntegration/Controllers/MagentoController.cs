using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using MagentoIntegration.Models;
using Newtonsoft.Json;
using System.Data;

namespace MagentoIntegration.Controllers
{
    public class MagentoController : Controller
    {
        string Baseurl = WebConfigurationManager.AppSettings["BaseURL"];
        //string grant_type = WebConfigurationManager.AppSettings["granttype"];
        string uname = WebConfigurationManager.AppSettings["UserName"];
        string password = WebConfigurationManager.AppSettings["password"];
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ItemData(int companyId, string name, string code, string TaxCategory)
        {
            bool postflg = false;
            string postmsg = "";
            Clsdata.LogFile("MagentoProduct", "Product creation method starting");
            try
            {
                string token = GetToken();
                Clsdata.LogFile("MagentoProduct", "Token:" + token);
                string strsql = "select sname ,scode,'9' attributeset,'20' price,1 status,1 visibility,'Simple' type_id,0.5 weight from vmcore_product where scode='" + code + "' ";
                Clsdata.LogFile("MagentoProduct", "strsql:" + strsql);
                DataSet ds = Clsdata.GetData(strsql, companyId);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Token.Product prd = new Token.Product();
                    prd.name = Convert.ToString(ds.Tables[0].Rows[0]["sname"]);
                    prd.sku = Convert.ToString(ds.Tables[0].Rows[0]["scode"]);
                    prd.attribute_set_id = Convert.ToInt32(ds.Tables[0].Rows[0]["attributeset"]);
                    prd.price = Convert.ToDouble(ds.Tables[0].Rows[0]["price"]);
                    prd.type_id = Convert.ToString(ds.Tables[0].Rows[0]["type_id"]);
                    prd.status = Convert.ToInt32(ds.Tables[0].Rows[0]["status"]);
                    prd.visibility = Convert.ToInt32(ds.Tables[0].Rows[0]["visibility"]);
                    prd.weight = Convert.ToString(ds.Tables[0].Rows[0]["weight"]);
                    prd.TaxClass = Convert.ToInt32(TaxCategory);
                    Token.ExtensionAttributes Extat = new Token.ExtensionAttributes();

                    Token.StockItem stock = new Token.StockItem();
                    stock.qty = "1";
                    stock.is_in_stock = true;
                    Extat.stock_item = stock;

                    Token.CategoryLink catlink = new Token.CategoryLink();
                    List<Token.CategoryLink> lstcatlink = new List<Token.CategoryLink>();
                    Extat.category_links = lstcatlink;

                    Token.CustomAttribute cust = new Token.CustomAttribute();
                    List<Token.CustomAttribute> lstcust = new List<Token.CustomAttribute>();

                    prd.extension_attributes = Extat;
                    prd.custom_attributes = lstcust;

                    Token.Products prds = new Token.Products();
                    prds.product = prd;

                    string sContent = JsonConvert.SerializeObject(prds);
                    Clsdata.LogFile("MagentoProduct", "sContent:" + sContent);
                    token = token.ToString().Trim();

                    var str = JsonConvert.DeserializeObject<string>(token);
                    //var str = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(token).ToString());
                    string authtoken = "Bearer " + str;

                    var client = new RestClient("https://www.dme-medical.com/rest/V1/products");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("Authorization", authtoken);
                    request.AddHeader("Content-Type", "application/json");
                    //request.AddHeader("Cookie", "PHPSESSID=ca03034b419ad51132de6a52801125a3");
                    request.AddParameter("application/json", sContent, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                    //    var client = new RestClient(Baseurl + "/rest/V1/products");
                    //    client.Timeout = -1;
                    //    var request = new RestRequest(Method.POST);                   

                    //    request.AddHeader("Content-Type", "application/json");
                    //    request.AddHeader("Authorization", "Bearer "+ token +"");
                    //    request.AddParameter("application/json", sContent, ParameterType.RequestBody);
                    //    IRestResponse response = client.Execute(request);

                    var responseData = JsonConvert.DeserializeObject<ProductResponse.Root>(response.Content);
                    if (responseData.id > 0)
                    {
                        //Clsdata.GetExecute("update  muCore_Product set id='" + responseData.id + "' where imasterid in (select imasterid from  mcore_product where scode='" + code + "' and istatus<>5) ", companyId);
                        postflg = true;
                        postmsg = "Product Master Posted Successfully";
                    }
                    //}
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { status = postflg, message = postmsg });
        }
        public string GetToken()
        {
            string Token = "";
            var client = new RestClient(Baseurl + "/rest/V1/integration/admin/token");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            Token.Gettoken cls = new Token.Gettoken();
            cls.username = uname;
            cls.password = password;
            string sContent = JsonConvert.SerializeObject(cls);

            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", sContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Token = response.Content;


            return Token;
        }
    }
}