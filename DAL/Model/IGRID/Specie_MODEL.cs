using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Specie_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string Inactive { get; set; }

    }
    public class Specie_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Specie_MODEL data { get; set; }
    }
    public class Specie_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Specie_MODEL> data { get; set; }
    }
    public class Specie_RESULT : RESULT_MODEL
    {
        public List<Specie_MODEL> data { get; set; }
    }
}
