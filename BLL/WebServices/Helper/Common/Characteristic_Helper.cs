using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServices.Model;

namespace WebServices.Helper
{
    public static class Characteristic_Helper
    {
        public static void UpdateCharacteristic(CHARACTERISTIC item, int userID, ARTWORKEntities context)
        {
            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
            characteristic.NAME = item.NAME;
            characteristic.VALUE = item.VALUE;
            characteristic.DESCRIPTION = item.DESCRIPTION;
            if (string.Format("{0}", item.Changed_Action) == "Inactive")
            {
                characteristic.VALUE = item.ID;
                characteristic.DESCRIPTION = item.ID;
                var existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();
                if (existItem != null)
                {
                    characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
                    characteristic.DESCRIPTION = string.Format("{0}", item.VALUE);

                    characteristic.IS_ACTIVE = "X";
                    characteristic.CREATE_BY = userID;
                    characteristic.CREATE_DATE = DateTime.Today;
                    characteristic.UPDATE_BY = userID;
                    characteristic.UPDATE_DATE = DateTime.Today;

                    SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);
                }
            }
            else if (string.Format("{0}", item.Changed_Action) == "Re-Active")
            {
                //characteristic.VALUE = string.Format("{0}-XXX Do Not Use XXX", item.Old_ID);
                characteristic.VALUE = string.Format("{0}", item.ID);
                //characteristic.NAME = string.Format("{0}", item.NAME);
                //characteristic.DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", item.ID);
                //var existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();
                //if ((new[] { "ZPKG_SEC_COMPANY_ADR", "ZPKG_SEC_PLANT_REGISTER", "ZPKG_SEC_CONTAINER_TYPE", "ZPKG_SEC_LID_TYPE", "ZPKG_SEC_PRIMARY_SIZE" }).Any(item.NAME.Equals)) { 
                   
                    characteristic.NAME = string.Format("{0}", item.NAME);
                    //characteristic.DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", "%");
                    var m2p = (from b in context.SAP_M_CHARACTERISTIC
                                    where b.NAME == item.NAME && b.VALUE == item.ID
                                    && b.DESCRIPTION.Contains("-XXX Do Not Use XXX")
                                 select b).ToList();
                   var existItem = m2p.FirstOrDefault();
                //} 


                if (existItem != null)
                {
                    characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
                    characteristic.DESCRIPTION = string.Format("{0}", item.DESCRIPTION);
                    //characteristic.VALUE = string.Format("{0}", item.ID);
                    characteristic.IS_ACTIVE = "X";
                    characteristic.CREATE_BY = userID;
                    characteristic.CREATE_DATE = DateTime.Today;
                    characteristic.UPDATE_BY = userID;
                    characteristic.UPDATE_DATE = DateTime.Today;

                    SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);

                }

                //if (item.NAME.Equals("ZPKG_SEC_PACKING"))
                //{  
                //        //SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

                //        characteristic.NAME = string.Format("{0}", item.NAME);
                //        characteristic.VALUE = string.Format("{0}-XXX Do Not Use XXX", item.Old_ID);
                //        characteristic.DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", item.Old_ID);

                //        existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();

                //        if (existItem != null)
                //        {
                //            characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
                //        }
                //        characteristic.VALUE = string.Format("{0}", item.ID);
                //        characteristic.DESCRIPTION = string.Format("{0}", item.VALUE);
                //}
            }
            else
            {

                characteristic.IS_ACTIVE = "X";
                characteristic.CREATE_BY = userID;
                characteristic.CREATE_DATE = DateTime.Today;
                characteristic.UPDATE_BY = userID;
                characteristic.UPDATE_DATE = DateTime.Today;

                SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);
            }
        }

        public static void UpdateBrand(ARTWORKEntities context)
        {
            string name = "ZPKG_SEC_BRAND";
            int userid = -2;
            var sapBrand = (from b in context.SAP_M_CHARACTERISTIC
                            where b.NAME == name
                            select b).ToList();

            if (sapBrand != null && sapBrand.Count > 0)
            {
                SAP_M_BRAND brand = new SAP_M_BRAND();
                foreach (SAP_M_CHARACTERISTIC iBrand in sapBrand)
                {
                    if (!string.IsNullOrEmpty(iBrand.VALUE))
                    {
                        brand = new SAP_M_BRAND();
                        brand.MATERIAL_GROUP = iBrand.VALUE;
                        brand = SAP_M_BRAND_SERVICE.GetByItem(brand, context).FirstOrDefault();

                        if (brand != null)
                        {
                            brand.DESCRIPTION = iBrand.DESCRIPTION;
                            brand.UPDATE_BY = userid;
                            SAP_M_BRAND_SERVICE.SaveOrUpdateNoLog(brand, context);
                        }
                        else
                        {
                            brand = new SAP_M_BRAND();
                            brand.MATERIAL_GROUP = iBrand.VALUE;
                            brand.DESCRIPTION = iBrand.DESCRIPTION;
                            brand.IS_ACTIVE = "X";
                            brand.CREATE_BY = userid;
                            brand.UPDATE_BY = userid;
                            SAP_M_BRAND_SERVICE.SaveOrUpdateNoLog(brand, context);
                        }
                    }

                    SAP_M_CHARACTERISTIC_SERVICE.DeleteByCHARACTERISTIC_ID(iBrand.CHARACTERISTIC_ID, context);
                }
            }
        }
    }
}