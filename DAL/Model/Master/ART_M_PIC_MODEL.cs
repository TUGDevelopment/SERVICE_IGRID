using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_M_PIC_2 : ART_M_PIC
    {
        public int ID { get; set; }
        public string USER_DISPLAY_TXT { get; set; }
        public string FIRST_NAME { get; set; }
        public string FIRST_NAME_DISPLAY_TXT { get; set; }
        public string LAST_NAME_DISPLAY_TXT { get; set; }

        public int SOLD_TO_ID { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SOLD_TO_EXCLUDE_DISPLAY_TXT { get; set; }
        public int SHIP_TO_ID { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_EXCLUDE_DISPLAY_TXT { get; set; }
        public int COUNTRY_ID { get; set; }
        public string COUNTRY_DISPLAY_TXT { get; set; }

        public int ZONE_ID { get; set; }

        public string LIST_SOLD_TO { get; set; }
        public string LIST_SHIP_TO { get; set; }
        public string LIST_ZONE { get; set; }
        public string LIST_COUNTRY { get; set; }
        public string LIST_PERSON { get; set; }

        public string EXPORT_EXCEL { get; set; }

    }

    public class ART_M_PIC_REQUEST : REQUEST_MODEL
    {
        public ART_M_PIC_2 data { get; set; }
    }

    public class ART_M_PIC_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_PIC_2> data { get; set; }
    }

    public class ART_M_PIC_RESULT : RESULT_MODEL
    {
        public List<ART_M_PIC_2> data { get; set; }
    }
}