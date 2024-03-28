using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public static class MaterialIGridHelper
    {
        public static int? GetPACharacteristicID(List<IGRID_M_OUTBOUND_ITEM> _items, string CHARACTERISTIC_NAME,ARTWORKEntities context)
        {
            var secGroup = _items.Where(i => i.CHARACTERISTIC_NAME == CHARACTERISTIC_NAME).FirstOrDefault();
            if (secGroup != null)
            {
                SAP_M_CHARACTERISTIC charac = new SAP_M_CHARACTERISTIC();
                charac.VALUE = secGroup.CHARACTERISTIC_VALUE;
                charac.NAME = CHARACTERISTIC_NAME;
                charac = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(charac,context).FirstOrDefault();

                if (charac != null)
                {
                    return charac.CHARACTERISTIC_ID;
                }
            }

            return null;
        }

        public static SAP_M_CHARACTERISTIC GetPACharacteristicData(List<IGRID_M_OUTBOUND_ITEM> _items, string CHARACTERISTIC_NAME,ARTWORKEntities context)
        {
            var secGroup = _items.Where(i => i.CHARACTERISTIC_NAME == CHARACTERISTIC_NAME).FirstOrDefault();
            if (secGroup != null)
            {
                SAP_M_CHARACTERISTIC charac = new SAP_M_CHARACTERISTIC();
                charac.VALUE = secGroup.CHARACTERISTIC_VALUE;
                charac.NAME = CHARACTERISTIC_NAME;
                charac = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(charac,context).FirstOrDefault();

                if (charac != null)
                {
                    return charac;
                }
            }

            return null;
        }

        public static IGRID_M_OUTBOUND_ITEM GetIgridItem(List<IGRID_M_OUTBOUND_ITEM> _items, string CHARACTERISTIC_NAME, ARTWORKEntities context)
        {
            var secGroup = _items.Where(i => i.CHARACTERISTIC_NAME == CHARACTERISTIC_NAME).FirstOrDefault();
            if (secGroup != null)
            {
                return secGroup;
            }

            return null;
        }

      
        public static string ConvertYesNoValue(string param)
        {
            if (String.IsNullOrEmpty(param))
            { return ""; }

            string value = "";

            if (param == "Yes")
            {
                value = "1";
            }
            else if (param == "No")
            {
                value = "0";
            }
            else
            {
                value = "";
            }

            return value;
        }

    }
}
