using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    [MetadataType(typeof(ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2_METADATA))]
    public partial class ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2 : ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT
    {
        public string PLANT_DISPLAY_TXT { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2_METADATA
    {

    }

    //public class ART_WF_MOCKUP_CHECK_LIST_REQUEST : REQUEST_MODEL
    //{
    //    public ART_WF_MOCKUP_CHECK_LIST_2 data { get; set; }
    //}

    //public class ART_WF_MOCKUP_CHECK_LIST_RESULT : RESULT_MODEL
    //{
    //    public List<ART_WF_MOCKUP_CHECK_LIST_2> data { get; set; }
    //}

}
