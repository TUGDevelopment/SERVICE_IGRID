using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class ProductGroup_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int Id { get; set; }
        public string Product_Group { get; set; }
        public string Product_GroupDesc { get; set; }
        public string PRD_Plant { get; set; }
        public string Inactive { get; set; }

    }
    public class ProductGroup_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public ProductGroup_MODEL data { get; set; }
    }
    public class ProductGroup_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ProductGroup_MODEL> data { get; set; }
    }
    public class ProductGroup_RESULT : RESULT_MODEL
    {
        public List<ProductGroup_MODEL> data { get; set; }
    }
}
