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
    public class ListOfArtworkHelper
    {
        public static ART_WF_ARTWORK_REQUEST_LISTOF_RESULT GetListOfArtworkHelper(ART_WF_ARTWORK_REQUEST_LISTOF_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST_LISTOF_RESULT Results_2 = new ART_WF_ARTWORK_REQUEST_LISTOF_RESULT();

            try
            {
                string displayTXT = "";

                if (param != null)
                {
                    if (param.data != null)
                    {
                        if (!String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            displayTXT = param.data.DISPLAY_TXT;
                        }
                    }
                }

                Results_2.status = "S";
                Results.data = MapperServices.ART_WF_ARTWORK_REQUEST(GetLov(displayTXT));

                if (Results.data.Count > 0)
                {
                    ART_WF_ARTWORK_REQUEST_LISTOF lovArtwork = new ART_WF_ARTWORK_REQUEST_LISTOF();
                    Results_2.data = new List<ART_WF_ARTWORK_REQUEST_LISTOF>();

                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        lovArtwork = new ART_WF_ARTWORK_REQUEST_LISTOF();
                        lovArtwork.ID = Results.data[i].ARTWORK_REQUEST_NO.ToString();
                        lovArtwork.DISPLAY_TXT = Results.data[i].ARTWORK_REQUEST_NO;
                        Results_2.data.Add(lovArtwork);
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results_2;
        }

        private static List<ART_WF_ARTWORK_REQUEST> GetLov(string displayTXT)
        {
            List<ART_WF_ARTWORK_REQUEST> listArtworks = new List<ART_WF_ARTWORK_REQUEST>();

            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!String.IsNullOrEmpty(displayTXT))
                    {
                        listArtworks = (from p in context.ART_WF_ARTWORK_REQUEST
                                        where p.ARTWORK_REQUEST_NO.Contains(displayTXT)
                                        select p).ToList();
                    }
                    else
                    {
                        listArtworks = (from p in context.ART_WF_ARTWORK_REQUEST
                                        where p.ARTWORK_REQUEST_NO != null && p.ARTWORK_REQUEST_NO != ""
                                        select p).ToList();
                    }
                }
            }

            return listArtworks;
        }
    }
}
