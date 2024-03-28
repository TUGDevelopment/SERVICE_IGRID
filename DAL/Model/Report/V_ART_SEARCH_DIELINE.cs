using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class V_ART_SEARCH_DIELINE_2 : V_ART_SEARCH_DIELINE
    {
        public int PACK_SIZE_ID { get; set; }
        public bool FIRST_LOAD { get; set; }
        public string FINAL_INFO_GROUP_DISPLAY_TXT { get; set; }
    }

    public class V_ART_SEARCH_DIELINE_REQUEST : REQUEST_MODEL
    {
        public V_ART_SEARCH_DIELINE_2 data { get; set; }
    }

    public class V_ART_SEARCH_DIELINE_RESULT : RESULT_MODEL
    {
        public List<V_ART_SEARCH_DIELINE_2> data { get; set; }
    }
}
