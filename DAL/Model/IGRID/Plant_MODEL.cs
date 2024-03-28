using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Plant_MODEL
    {
        public string Id { get; set; }
        public string Description { get; set; }

    }
    public class Plant_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Plant_MODEL data { get; set; }
    }
    public class Plant_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Plant_MODEL> data { get; set; }
    }
    public class Plant_RESULT : RESULT_MODEL
    {
        public List<Plant_MODEL> data { get; set; }
    }
}
