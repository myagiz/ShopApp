using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopApp.WebUI.Models.Home
{
    public class Item
    {
        public tblUrunler Product { get; set; }
        public int Quantity { get; set; }
        public tblMusteriler Customer { get; set; }
    }
}