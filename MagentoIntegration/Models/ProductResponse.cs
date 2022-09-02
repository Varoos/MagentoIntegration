using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagentoIntegration.Models
{
    public class ProductResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class StockItem
        {
            public int item_id { get; set; }
            public int product_id { get; set; }
            public int stock_id { get; set; }
            public int qty { get; set; }
            public bool is_in_stock { get; set; }
            public bool is_qty_decimal { get; set; }
            public bool show_default_notification_message { get; set; }
            public bool use_config_min_qty { get; set; }
            public int min_qty { get; set; }
            public int use_config_min_sale_qty { get; set; }
            public int min_sale_qty { get; set; }
            public bool use_config_max_sale_qty { get; set; }
            public int max_sale_qty { get; set; }
            public bool use_config_backorders { get; set; }
            public int backorders { get; set; }
            public bool use_config_notify_stock_qty { get; set; }
            public int notify_stock_qty { get; set; }
            public bool use_config_qty_increments { get; set; }
            public int qty_increments { get; set; }
            public bool use_config_enable_qty_inc { get; set; }
            public bool enable_qty_increments { get; set; }
            public bool use_config_manage_stock { get; set; }
            public bool manage_stock { get; set; }
            public object low_stock_date { get; set; }
            public bool is_decimal_divided { get; set; }
            public int stock_status_changed_auto { get; set; }
        }

        public class ExtensionAttributes
        {
            public List<int> website_ids { get; set; }
            public StockItem stock_item { get; set; }
        }

        public class CustomAttribute
        {
            public string attribute_code { get; set; }
            public object value { get; set; }
        }

        public class Root
        {
            public int id { get; set; }
            public string sku { get; set; }
            public string name { get; set; }
            public int attribute_set_id { get; set; }
            public int price { get; set; }
            public int status { get; set; }
            public int visibility { get; set; }
            public string type_id { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public double weight { get; set; }
            public ExtensionAttributes extension_attributes { get; set; }
            public List<object> product_links { get; set; }
            public List<object> options { get; set; }
            public List<object> media_gallery_entries { get; set; }
            public List<object> tier_prices { get; set; }
            public List<CustomAttribute> custom_attributes { get; set; }
        }


    }
}