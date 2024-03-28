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
    public class CompanyHelper
    {
        public static SAP_M_COMPANY_RESULT GetCompany(SAP_M_COMPANY_REQUEST param)
        {
            SAP_M_COMPANY_RESULT Results = new SAP_M_COMPANY_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            param = new SAP_M_COMPANY_REQUEST();
                            param.data = new SAP_M_COMPANY_2();
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COMPANY(SAP_M_COMPANY_SERVICE.GetByItem(MapperServices.SAP_M_COMPANY(param.data), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COMPANY(SAP_M_COMPANY_SERVICE.GetByItem(MapperServices.SAP_M_COMPANY(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].COMPANY_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].COMPANY_CODE + ":" + Results.data[i].DESCRIPTION;
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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


        public static SAP_M_COMPANY_RESULT GetCompany_COM_CODE(SAP_M_COMPANY_REQUEST param)
        {
            SAP_M_COMPANY_RESULT Results = new SAP_M_COMPANY_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            param = new SAP_M_COMPANY_REQUEST();
                            param.data = new SAP_M_COMPANY_2();
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COMPANY(SAP_M_COMPANY_SERVICE.GetByItem(MapperServices.SAP_M_COMPANY(param.data), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COMPANY(SAP_M_COMPANY_SERVICE.GetByItem(MapperServices.SAP_M_COMPANY(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = CNService.ToInt(Results.data[i].COM_CODE);
                                Results.data[i].DISPLAY_TXT = Results.data[i].COM_CODE + ":" + Results.data[i].DESCRIPTION;
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
                           
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
