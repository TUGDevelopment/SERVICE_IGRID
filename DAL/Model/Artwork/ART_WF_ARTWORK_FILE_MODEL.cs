using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    //ART_WF_ARTWORK_FILE
    public partial class ART_WF_ARTWORK_FILE_2 : ART_WF_ARTWORK_REQUEST
    {

        public string SaveEmpty { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_RECIPIENT> RECIPIENT_EMAIL { get; set; }
    }

    public class ART_WF_ARTWORK_FILE_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_FILE_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_FILE_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_2> data { get; set; }
    }
}
