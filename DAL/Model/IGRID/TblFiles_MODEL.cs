using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DAL.Model
{
    public class TblFiles_MODEL
    {
        public int id { get; set; }
        public string name { get; set; }
        public string contenttype { get; set; }
        public int matdoc { get; set; }
        public byte[] data { get; set; }
        public string activeby { get; set; }


     
        public long size { get; set; }
        public bool canDelete { get; set; }
        public bool canDownload { get; set; }
        public string create_by_display_txt { get; set; }
        public string create_date_display_txt { get; set; }
        public string create_by_desc_txt { get; set; }
        public string extension { get; set; }
        public string msg { get; set; }
        public string error { get; set; }
        public string NODE_ID_TXT { get; set; }
        public long nodeid { get; set; }
    }

    public class TblFiles_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public TblFiles_MODEL data { get; set; }
    }
    public class TblFiles_REQUEST_LIST : REQUEST_MODEL
    {
        public List<TblFiles_MODEL> data { get; set; }
    }
    public class TblFiles_RESULT : RESULT_MODEL
    {
        public List<TblFiles_MODEL> data { get; set; }
    }
}
