using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServices.Model
{
    public class CHARACTERISTIC
    {
        public string Name { get; set; }
        public string Values { get; set; }
        public string Description { get; set; }
    }

    public  class CHARACTERISTICS
    {
        public List<CHARACTERISTIC> ListCharacteristics { get; set; }

        public CHARACTERISTICS()
        {
            ListCharacteristics = new List<CHARACTERISTIC>();
        }
    }
}