using DAL;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL.Model;
namespace PLL.zProgram
{
    public partial class zEmailOverdue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        void Main(int artwork_item_id)
        {
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    //MapperServices.Initialize();
                    using (CNService.IsolationLevel(context))
                    {

                        var stepMO = (from m in context.ART_M_STEP_MOCKUP
                                      select new ART_M_STEP_MOCKUP_2
                                      {
                                          STEP_MOCKUP_ID = m.STEP_MOCKUP_ID,
                                          DURATION = m.DURATION,
                                          DURATION_EXTEND = m.DURATION_EXTEND
                                      }).ToList();

                        var stepAW = (from m in context.ART_M_STEP_ARTWORK
                                      select new ART_M_STEP_ARTWORK_2
                                      {
                                          STEP_ARTWORK_ID = m.STEP_ARTWORK_ID,
                                          DURATION = m.DURATION,
                                          DURATION_EXTEND = m.DURATION_EXTEND
                                      }).ToList();

                        //#region "Mockup"
                        //var allProcessMockup = (from m in context.ART_WF_MOCKUP_PROCESS
                        //                        where string.IsNullOrEmpty(m.IS_END)
                        //                        select new ART_WF_MOCKUP_PROCESS_2
                        //                        {
                        //                            MOCKUP_ID = m.MOCKUP_ID,
                        //                            MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                        //                            CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        //                            CREATE_DATE = m.CREATE_DATE,
                        //                            IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                        //                            CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                        //                            CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID
                        //                        }).ToList();

                        //foreach (var item in allProcessMockup)
                        //{
                        //    try
                        //    {
                        //        var durationValue = stepMO.Where(s => s.STEP_MOCKUP_ID == item.CURRENT_STEP_ID).Select(s => s.DURATION).FirstOrDefault();
                        //        if (durationValue == null) durationValue = 0;
                        //        var durationExtendValue = stepMO.Where(s => s.STEP_MOCKUP_ID == item.CURRENT_STEP_ID).Select(s => s.DURATION_EXTEND).FirstOrDefault();
                        //        if (durationExtendValue == null) durationExtendValue = 0;
                        //        var durationStep = (int)Math.Ceiling(durationValue.Value);
                        //        var durationStepExtend = (int)Math.Ceiling(durationExtendValue.Value);

                        //        var createDate = item.CREATE_DATE;
                        //        var finishStepDate = CNService.AddBusinessDays(createDate, durationStep);
                        //        var finishStepDateExtend = CNService.AddBusinessDays(createDate, durationStepExtend);

                        //        DateTime finishDate = new DateTime();

                        //        finishDate = finishStepDate;

                        //        if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND == "X")
                        //        {
                        //            finishDate = finishStepDateExtend;
                        //        }

                        //        if (DateTime.Now.Date.AddDays(1).Date == finishDate.Date)
                        //        {
                        //            if (item.CURRENT_VENDOR_ID > 0)
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE_VN1", context);
                        //            else if (item.CURRENT_CUSTOMER_ID > 0)
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE_CUS1", context);
                        //            else
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE1", context);
                        //        }
                        //        else if (DateTime.Now.Date == finishDate.Date)
                        //        {
                        //            if (item.CURRENT_VENDOR_ID > 0)
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE_VN2", context);
                        //            else if (item.CURRENT_CUSTOMER_ID > 0)
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE_CUS2", context);
                        //            else
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE2", context);
                        //        }
                        //        else if (DateTime.Now.Date > finishDate.Date)
                        //        {
                        //            if (item.CURRENT_VENDOR_ID > 0)
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE_VN3", context);
                        //            else if (item.CURRENT_CUSTOMER_ID > 0)
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE_CUS3", context);
                        //            else
                        //                BLL.Services.EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_OVERDUE3", context);
                        //        }
                        //    }
                        //    catch (Exception ex) { CNService.GetErrorMessage(ex); }
                        //}
                        //#endregion

                        #region "Artwork"
                        var allProcessArtwork = (from m in context.ART_WF_ARTWORK_PROCESS
                                                 where string.IsNullOrEmpty(m.IS_END) && m.ARTWORK_ITEM_ID == artwork_item_id
                                                 select new ART_WF_ARTWORK_PROCESS_2
                                                 {
                                                     ARTWORK_REQUEST_ID = m.ARTWORK_REQUEST_ID,
                                                     ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                                                     CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                                     CREATE_DATE = m.CREATE_DATE,
                                                     IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                                                     CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                                                     CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID
                                                 }).ToList();


                        foreach (var item in allProcessArtwork)
                        {
                            try
                            {
                                var durationValue = stepAW.Where(s => s.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).Select(s => s.DURATION).FirstOrDefault();
                                if (durationValue == null) durationValue = 0;
                                var durationExtendValue = stepAW.Where(s => s.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).Select(s => s.DURATION_EXTEND).FirstOrDefault();
                                if (durationExtendValue == null) durationExtendValue = 0;
                                var durationStep = (int)Math.Ceiling(durationValue.Value);
                                var durationStepExtend = (int)Math.Ceiling(durationExtendValue.Value);
                                var createDate = item.CREATE_DATE;
                                var finishStepDate = CNService.AddBusinessDays(createDate, durationStep);
                                var finishStepDateExtend = CNService.AddBusinessDays(createDate, durationStepExtend);

                                DateTime finishDate = new DateTime();

                                finishDate = finishStepDate;

                                if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND == "X")
                                {
                                    finishDate = finishStepDateExtend;
                                }

                                if (DateTime.Now.Date.AddDays(1).Date == finishDate.Date)
                                {
                                    if (item.CURRENT_VENDOR_ID > 0)
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE_VN1", context);
                                    else if (item.CURRENT_CUSTOMER_ID > 0)
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE_CUS1", context);
                                    else
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE1", context);
                                }
                                else if (DateTime.Now.Date == finishDate.Date)
                                {
                                    if (item.CURRENT_VENDOR_ID > 0)
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE_VN2", context);
                                    else if (item.CURRENT_CUSTOMER_ID > 0)
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE_CUS2", context);
                                    else
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE2", context);
                                }
                                else if (DateTime.Now.Date > finishDate.Date)
                                {
                                    if (item.CURRENT_VENDOR_ID > 0)
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE_VN3", context);
                                    else if (item.CURRENT_CUSTOMER_ID > 0)
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE_CUS3", context);
                                    else
                                        BLL.Services.EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_OVERDUE3", context);
                                }
                            }
                            catch (Exception ex) { CNService.GetErrorMessage(ex); }
                        }
                        #endregion
                    }
                }

                Console.WriteLine("Completed");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        protected void btnDecrypt_Click(object sender, EventArgs e)
        {
            int artwork_item_id = Convert.ToInt32( txtartwork_item_id.Text);
            Main(artwork_item_id);
        }
    }
}