using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Attachment_MODEL : IGRID_AUTHROLIZE_CHANGE
    {        
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public int MatDoc { get; set; }
        public string ActiveBy { get; set; }

    }
    public class Attachment_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Attachment_MODEL data { get; set; }
    }
    public class Attachment_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Attachment_MODEL> data { get; set; }
    }
    public class Attachment_RESULT : RESULT_MODEL
    {
        public List<Attachment_MODEL> data { get; set; }
        public string haveAuthrolizeEditMaster { get; set; }
    }
}
