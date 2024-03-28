using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ProductCode_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string PRODUCT_CODE { get; set; }
        public string Address { get; set; }
        public string RegisteredNo { get; set; }
        public int Count_RegisteredNo { get; set; }
        public int Count_Address { get; set; }
        public string prd_plant { get; set; }
        public bool FIRST_LOAD { get; set; }
    }

     
    public class ProductCode_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public ProductCode_MODEL data { get; set; }
    }

    public class ProductCode_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ProductCode_MODEL> data { get; set; }
    }

    public class ProductCode_RESULT : RESULT_MODEL
    {
        public List<ProductCode_MODEL> data { get; set; }
    }
}
