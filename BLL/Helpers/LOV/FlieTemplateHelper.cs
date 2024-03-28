using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL.Model;

namespace BLL.Helpers
{
    public class FileTemplateHelper
    {
        public static CN_LOV_MODEL_RESULT GetFileTemplateMockup(CN_LOV_MODEL_REQUEST param)
        {
            CN_LOV_MODEL_RESULT Results = new CN_LOV_MODEL_RESULT();

            try
            {
                var parentId = Convert.ToInt64(ConfigurationManager.AppSettings["MockupFileTemplateNodeID"]);
                var token = CWSService.getAuthToken();
                var allFile = CWSService.getAllNodeInFolder(parentId, token);
                Results.data = new List<CN_LOV_MODEL>();

                foreach (var file in allFile)
                {
                    CN_LOV_MODEL item = new CN_LOV_MODEL();
                    item.ID = Convert.ToInt32(file.ID);
                    item.DISPLAY_TXT = file.Name;
                    Results.data.Add(item);
                }

                if (Results.data.Count > 0)
                {
                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        Results.data = (from u1 in Results.data
                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                        select u1).ToList();
                    }

                    Results.data = Results.data.OrderBy(m => m.DISPLAY_TXT).ToList();
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

        public static CN_LOV_MODEL_RESULT GetFileTemplateArtwork(CN_LOV_MODEL_REQUEST param)
        {
            CN_LOV_MODEL_RESULT Results = new CN_LOV_MODEL_RESULT();

            try
            {
                var parentId = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkFileTemplateNodeID"]);
                var token = CWSService.getAuthToken();
                var allFile = CWSService.getAllNodeInFolder(parentId, token);
                Results.data = new List<CN_LOV_MODEL>();

                foreach (var file in allFile)
                {
                    CN_LOV_MODEL item = new CN_LOV_MODEL();
                    item.ID = Convert.ToInt32(file.ID);
                    item.DISPLAY_TXT = file.Name;
                    Results.data.Add(item);
                }

                if (Results.data.Count > 0)
                {
                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        Results.data = (from u1 in Results.data
                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                        select u1).ToList();
                    }

                    Results.data = Results.data.OrderBy(m => m.DISPLAY_TXT).ToList();
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
