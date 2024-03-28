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
    public class PAByMKHelper
    {
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT SavePAByMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var mkData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            mkData.ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID;

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.SaveOrUpdate(mkData, context);

                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                        }

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>();
                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2();
                        List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>();

                        item.ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID = mkData.ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        if (param.data.ACTION_CODE == "APPROVE")
                        {
                            var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var ID_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = stepId }, context).FirstOrDefault();

                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, ID_PA.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                        }

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001",context);
                    }
                }
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
