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
    public class DiStickerController : ApiController
    {

        [Route("api/lov/distick")]
        public SAP_M_CHARACTERISTIC_RESULT GetDiStick([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return ItemDiStickerHelper.GetDiStick(param);
        }


    }
}
