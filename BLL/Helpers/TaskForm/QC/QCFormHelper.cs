using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class QCFormHelper
    {
        public static ART_WF_ARTWORK_PROCESS_QC_RESULT GetQCForm(ART_WF_ARTWORK_PROCESS_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_QC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            return Results;
                        }
                        else
                        {
                            ART_WF_ARTWORK_PROCESS_QC_2 qc2 = new ART_WF_ARTWORK_PROCESS_QC_2();
                            List<ART_WF_ARTWORK_PROCESS_QC_2> listQC2 = new List<ART_WF_ARTWORK_PROCESS_QC_2>();

                            List<int> listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);
                            var QCProcessSubID = 0;

                            var QCStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_QC").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                            var QCProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                               where listSubID.Contains(p.ARTWORK_SUB_ID)
                                               && p.CURRENT_STEP_ID == QCStepID
                                               && p.REMARK_KILLPROCESS == null
                                               select p).ToList();
                            if (QCProcesses != null && QCProcesses.Count > 0)
                            {
                                var QCProcess = QCProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                qc2.ARTWORK_SUB_ID = QCProcess.ARTWORK_SUB_ID;
                                QCProcessSubID = QCProcess.ARTWORK_SUB_ID;
                            }

                            listQC2 = MapperServices.ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(qc2, context).ToList());

                            Results.data = listQC2;
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
