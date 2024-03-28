using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Condition_MODEL
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string ROLE { get; set; }
        public string user_name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int au_id { get; set; }
    }
    public class Condition_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Condition_MODEL data { get; set; }
    }
    public class Condition_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Condition_MODEL> data { get; set; }
    }
    public class Condition_RESULT : RESULT_MODEL
    {
        public List<Condition_MODEL> data { get; set; }
    }
}
