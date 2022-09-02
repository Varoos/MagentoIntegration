using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentoIntegration.Models
{
    public class Token
    {

        public class Gettoken
        {
            public string username { get; set; }
            public string password { get; set; }
        }
            public class CategoryLink
        {
            public int position { get; set; }
            public string category_id { get; set; }
        }

        public class StockItem
        {
            public string qty { get; set; }
            public bool is_in_stock { get; set; }
        }

        public class ExtensionAttributes
        {
            public List<CategoryLink> category_links { get; set; }
            public StockItem stock_item { get; set; }
        }

        public class CustomAttribute
        {
            public string attribute_code { get; set; }
            public string value { get; set; }
        }

        public class Product
        {
            public string sku { get; set; }
            public string name { get; set; }
            public int attribute_set_id { get; set; }
            public double price { get; set; }
            public int status { get; set; }
            public int visibility { get; set; }
            public string type_id { get; set; }
            public string weight { get; set; }
            public int TaxClass { get; set; }
            public ExtensionAttributes extension_attributes { get; set; }
            public List<CustomAttribute> custom_attributes { get; set; }
        }

        public class Products
        {
            public Product product { get; set; }
        }

        public class GetToken
        {
            public string Token { get; set; }
        }






    }
}