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
    public class RDPersonHelper
    {
        public static ART_M_USER_RESULT GetRDPerson(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            ART_M_USER_2 user = new ART_M_USER_2();
                            user.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(param.data), context));
                        }

                        if (param != null && param.data != null)
                        {
                            if (param.data.COMPANY_ID > 0)
                            {
                                var userCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new DAL.ART_M_USER_COMPANY() { COMPANY_ID = Convert.ToInt32(param.data.COMPANY_ID) }, context).Select(m => m.USER_ID).ToList();
                                Results.data = Results.data.Where(m => userCompany.Contains(m.USER_ID)).ToList();
                            }
                        }

                        if (Results.data.Count > 0)
                        {
                            List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                            ART_M_USER_2 item = new ART_M_USER_2();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var doWork = false;
                                var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new DAL.ART_M_USER_ROLE() { USER_ID = Results.data[i].USER_ID }, context);
                                foreach (var role in listRole)
                                {
                                    var roleId = role.ROLE_ID;
                                    var roleCode = ART_M_ROLE_SERVICE.GetByROLE_ID(roleId, context).ROLE_CODE;
                                    if (roleCode.StartsWith("RD_"))
                                    {
                                        doWork = true;
                                    }
                                }

                                if (doWork)
                                {
                                    item = new ART_M_USER_2();
                                    item.ID = Results.data[i].USER_ID;
                                    item.DISPLAY_TXT = CNService.GetUserName(Results.data[i].USER_ID, context);
                                    item.DISPLAY_TXT = item.DISPLAY_TXT.Trim();
                                    newList.Add(item);
                                }
                            }

                            Results.data = newList;

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

        public static ART_M_USER_RESULT GetRDPersonFFC(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            ART_M_USER_2 user = new ART_M_USER_2();
                            user.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(param.data), context));
                        }

                        if (param != null && param.data != null)
                        {
                            if (param.data.COMPANY_ID > 0)
                            {
                                var userCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new DAL.ART_M_USER_COMPANY() { COMPANY_ID = Convert.ToInt32(param.data.COMPANY_ID) }, context).Select(m => m.USER_ID).ToList();
                                Results.data = Results.data.Where(m => userCompany.Contains(m.USER_ID)).ToList();
                            }
                        }

                        if (Results.data.Count > 0)
                        {
                            var PositionID = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "FFC" }, context).FirstOrDefault().ART_M_POSITION_ID;
                            Results.data = Results.data.Where(m => m.POSITION_ID == PositionID).ToList();

                            List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                            ART_M_USER_2 item = new ART_M_USER_2();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var doWork = false;
                                var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new DAL.ART_M_USER_ROLE() { USER_ID = Results.data[i].USER_ID }, context);
                                foreach (var role in listRole)
                                {
                                    var roleId = role.ROLE_ID;
                                    var roleCode = ART_M_ROLE_SERVICE.GetByROLE_ID(roleId, context).ROLE_CODE;
                                    if (roleCode.StartsWith("RD_"))
                                    {
                                        doWork = true;
                                    }
                                }

                                if (doWork)
                                {
                                    item = new ART_M_USER_2();
                                    item.ID = Results.data[i].USER_ID;
                                    item.DISPLAY_TXT = CNService.GetUserName(Results.data[i].USER_ID, context);
                                    item.DISPLAY_TXT = item.DISPLAY_TXT.Trim();
                                    newList.Add(item);
                                }
                            }

                            Results.data = newList;

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

        public static ART_M_USER_RESULT GetRDPersonTHolding(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            ART_M_USER_2 user = new ART_M_USER_2();
                            user.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(param.data), context));
                        }

                        if (param != null && param.data != null)
                        {
                            if (param.data.COMPANY_ID > 0)
                            {
                                var userCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new DAL.ART_M_USER_COMPANY() { COMPANY_ID = Convert.ToInt32(param.data.COMPANY_ID) }, context).Select(m => m.USER_ID).ToList();
                                Results.data = Results.data.Where(m => userCompany.Contains(m.USER_ID)).ToList();
                            }
                        }

                        if (Results.data.Count > 0)
                        {
                            var PositionID = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "T-HOLDING" }, context).FirstOrDefault().ART_M_POSITION_ID;
                            Results.data = Results.data.Where(m => m.POSITION_ID == PositionID).ToList();

                            List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                            ART_M_USER_2 item = new ART_M_USER_2();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var doWork = false;
                                var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new DAL.ART_M_USER_ROLE() { USER_ID = Results.data[i].USER_ID }, context);
                                foreach (var role in listRole)
                                {
                                    var roleId = role.ROLE_ID;
                                    var roleCode = ART_M_ROLE_SERVICE.GetByROLE_ID(roleId, context).ROLE_CODE;
                                    if (roleCode.StartsWith("RD_"))
                                    {
                                        doWork = true;
                                    }
                                }

                                if (doWork)
                                {
                                    item = new ART_M_USER_2();
                                    item.ID = Results.data[i].USER_ID;
                                    item.DISPLAY_TXT = CNService.GetUserName(Results.data[i].USER_ID, context);
                                    item.DISPLAY_TXT = item.DISPLAY_TXT.Trim();
                                    newList.Add(item);
                                }
                            }

                            Results.data = newList;

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
