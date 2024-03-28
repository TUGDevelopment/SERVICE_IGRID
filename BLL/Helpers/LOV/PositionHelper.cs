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
    public class PositionHelper
    {
        public static ART_M_POSITION_RESULT getPosition(ART_M_POSITION_REQUEST param)
        {
            ART_M_POSITION_RESULT Results = new ART_M_POSITION_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        ART_M_POSITION filter = new ART_M_POSITION();
                        Results.data = MapperServices.ART_M_POSITION(ART_M_POSITION_SERVICE.GetAll(context).ToList());

                        if (Results.data.Count > 0)
                        {
                            if (Results.data.Count > 0)
                            {
                                for (int i = 0; i < Results.data.Count; i++)
                                {
                                    Results.data[i].ID = Results.data[i].ART_M_POSITION_ID;
                                    Results.data[i].DISPLAY_TXT = Results.data[i].ART_M_POSITION_NAME;
                                }

                                if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                                {
                                    Results.data = (from u1 in Results.data
                                                    where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                    select u1).ToList();
                                }
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
