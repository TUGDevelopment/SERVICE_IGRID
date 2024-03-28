﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_PO_COMPLETE_SO_HEADER_2 : SAP_M_PO_COMPLETE_SO_HEADER
    {
        public string GENERAL_TEXT { get; set; }
        public List<SAP_M_PO_COMPLETE_SO_ITEM_2> ITEMS { get; set; }
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_HEADER_2_METADATA
    {
      
    }

    public class SAP_M_PO_COMPLETE_SO_HEADER_REQUEST : REQUEST_MODEL
    {
        public SAP_M_PO_COMPLETE_SO_HEADER_2 data { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_HEADER_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SAP_M_PO_COMPLETE_SO_HEADER_2> data { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_HEADER_RESULT : RESULT_MODEL
    {
        public List<SAP_M_PO_COMPLETE_SO_HEADER_2> data { get; set; }
    }
}
