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
    public static class MessageHelper
    {
        public static ART_M_MESSAGE_RESULT GetMessage(ART_M_MESSAGE_REQUEST param)
        {
            ART_M_MESSAGE_RESULT Results = new ART_M_MESSAGE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        ART_M_MESSAGE_REQUEST paramTmp = new ART_M_MESSAGE_REQUEST();
                        ART_M_MESSAGE_2 message_2 = new ART_M_MESSAGE_2();

                        if (param == null || param.data == null)
                        {
                            message_2.IS_ACTIVE = "X";
                            paramTmp.data = message_2;
                            Results.data = MapperServices.ART_M_MESSAGE(ART_M_MESSAGE_SERVICE.GetByItem(MapperServices.ART_M_MESSAGE(paramTmp.data), context));
                        }
                        else
                        {
                            message_2 = param.data;
                            message_2.IS_ACTIVE = "X";
                            paramTmp.data = message_2;
                            Results.data = MapperServices.ART_M_MESSAGE(ART_M_MESSAGE_SERVICE.GetByItemContain(MapperServices.ART_M_MESSAGE(param.data), context));
                        }

                        Results.status = "S";
                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].DISPLAY_TXT = Results.data[i].MSG_CODE + " : " + Results.data[i].MSG_DESCRIPTION;
                            }
                        }
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

        public static ART_M_MESSAGE_RESULT GetMessage(ART_M_MESSAGE_REQUEST param,ARTWORKEntities context)
        {
            ART_M_MESSAGE_RESULT Results = new ART_M_MESSAGE_RESULT();

            try
            {
                ART_M_MESSAGE_REQUEST paramTmp = new ART_M_MESSAGE_REQUEST();
                ART_M_MESSAGE_2 message_2 = new ART_M_MESSAGE_2();

                if (param == null || param.data == null)
                {
                    message_2.IS_ACTIVE = "X";
                    paramTmp.data = message_2;
                    Results.data = MapperServices.ART_M_MESSAGE(ART_M_MESSAGE_SERVICE.GetByItem(MapperServices.ART_M_MESSAGE(paramTmp.data), context));
                }
                else
                {
                    message_2 = param.data;
                    message_2.IS_ACTIVE = "X";
                    paramTmp.data = message_2;
                    Results.data = MapperServices.ART_M_MESSAGE(ART_M_MESSAGE_SERVICE.GetByItemContain(MapperServices.ART_M_MESSAGE(param.data), context));
                }

                Results.status = "S";
                if (Results.data.Count > 0)
                {
                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        Results.data[i].DISPLAY_TXT = Results.data[i].MSG_CODE + " : " + Results.data[i].MSG_DESCRIPTION;
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

        public static string GetMessage(string code)
        {
            string msg = "";
            using (var context = new ARTWORKEntities())
            {
                msg = GetMessage(code, context);
            }

            return msg;
        }

        public static string GetMessage(string code, ARTWORKEntities context)
        {
            string msg = "";
            ART_M_MESSAGE_REQUEST param = new ART_M_MESSAGE_REQUEST();
            ART_M_MESSAGE_RESULT result = new ART_M_MESSAGE_RESULT();
            ART_M_MESSAGE_2 tempMsg = new ART_M_MESSAGE_2();
            tempMsg.MSG_CODE = code;
            param.data = tempMsg;

            result = GetMessage(param, context);

            if (result != null && result.data != null && result.data.Count > 0)
            {
                msg = result.data[0].MSG_DESCRIPTION.ToString();
            }

            return msg;
        }
    }
}
