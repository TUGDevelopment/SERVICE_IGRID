using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;

namespace PLL.API
{
    public class QCFormController : ApiController
    {
       

        [Route("api/taskform/internal/qc/info")]
        public ART_WF_ARTWORK_PROCESS_QC_RESULT GetQCForm([FromUri]ART_WF_ARTWORK_PROCESS_QC_REQUEST param)
        {
            return QCFormHelper.GetQCForm(param);
        }

     
    }

  
}
