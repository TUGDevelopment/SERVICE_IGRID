using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;
using BLL.Helpers.Master;

namespace PLL.API
{
    public class PICController : ApiController
    {
        [Route("api/master/pic")]
        public ART_M_PIC_RESULT GetPICData([FromUri]ART_M_PIC_REQUEST param)
        {
            return PICHelper.GetPIC(param);
        }

        [Route("api/master/pic/edit")]
        public ART_M_PIC_RESULT GetPICDataEdit([FromUri]ART_M_PIC_REQUEST param)
        {
            return PICHelper.GetPICEdit(param);
        }

        [Route("api/master/pic/edit")]
        public ART_M_PIC_RESULT PostPICDataEdit(ART_M_PIC_REQUEST param)
        {
            return PICHelper.SavePICEdit(param);
        }

        [Route("api/master/pic/delete")]
        public ART_M_PIC_RESULT DeletePICDataEdit(ART_M_PIC_REQUEST param)
        {
            return PICHelper.DeletePICEdit(param);
        }

        [Route("api/master/pic/deletelist")]
        public ART_M_PIC_RESULT DeletePICDataList(ART_M_PIC_REQUEST_LIST param)
        {
            return PICHelper.DeletePICList(param);
        }
    }
}
