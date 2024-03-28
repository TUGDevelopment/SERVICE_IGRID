﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 : ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP
    {
        public ART_WF_ARTWORK_PROCESS_2 PROCESS { get; set; }
        public bool ENDTASKFORM { get; set; }
        public int VENDOR_ID { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2> data { get; set; }
    }
}
