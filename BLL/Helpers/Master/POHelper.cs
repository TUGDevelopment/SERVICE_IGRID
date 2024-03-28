using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class POHelper
    {
        public static SAP_M_PO_IDOC_RESULT GetPurchasingOrg(SAP_M_PO_IDOC_REQUEST param)
        {
            SAP_M_PO_IDOC_RESULT Results = new SAP_M_PO_IDOC_RESULT();
            List<SAP_M_PO_IDOC_2> listPO_2 = new List<SAP_M_PO_IDOC_2>();
            SAP_M_PO_IDOC_2 po_2 = new SAP_M_PO_IDOC_2();
            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var poIDOC = context.SAP_M_PO_IDOC.GroupBy(po => po.PURCHASING_ORG).Select(po => po.Key).ToList();
                        foreach (var i in poIDOC)
                        {
                            po_2 = new SAP_M_PO_IDOC_2
                            {
                                ID = i,
                                DISPLAY_TXT = i
                            };

                            listPO_2.Add(po_2);
                        }

                        Results.data = listPO_2.OrderBy(so => so.DISPLAY_TXT).ToList();
                        if (param != null && param.data != null)
                        {
                            if (param.data.DISPLAY_TXT != null && param.data.DISPLAY_TXT != "")
                            {
                                var filteredUserList = listPO_2.Where(s => (s.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))).OrderBy(so => so.DISPLAY_TXT).ToList();
                                Results.data = filteredUserList;
                            }
                        }
                    }
                }

                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }
    }
}
