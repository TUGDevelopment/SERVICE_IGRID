using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Web;

namespace WebServices.Model
{
    public class SERVICE_REQUEST_MODEL
    {
        public bool validate { get; set; }
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
    }

    [DataContract]
    public class SERVICE_RESULT_MODEL
    {
        [DataMember]
        public string status { get; set; }

        [DataMember]
        public string msg { get; set; }

        [DataMember]
        public string start { get; set; }

        [DataMember]
        public string finish { get; set; }

        [DataMember]
        public int cnt { get; set; }
    }
}
