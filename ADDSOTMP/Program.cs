using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WebServices.Model;
using BLL.Services;
using System.Configuration;

namespace ADDSOTMP
{
    class Program
    {
   
        static string jobtype = ConfigurationManager.AppSettings["jobtype"];
        static void Main(string[] args)
        {
            try
            {
                //non fix order
                CNService.buildinterface(string.Format("{0}", jobtype));
                Console.WriteLine("Completed");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
    }

    public class Serializer
    {
        public T Deserialize<T>(string input) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(input))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public string Serialize<T>(T ObjectToSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, ObjectToSerialize);
                return textWriter.ToString();
            }
        }
    }
}
