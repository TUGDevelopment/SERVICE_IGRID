using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class ReferenceHelper
    {
        //public static XECM_M_REFERENCE_RESULT GetReference(XECM_M_REFERENCE_REQUEST_LIST param)
        //{
        //    XECM_M_REFERENCE_RESULT Results = new XECM_M_REFERENCE_RESULT();
        //    List<XECM_M_REFERENCE_2> _listRef2 = new List<XECM_M_REFERENCE_2>();
        //    XECM_M_REFERENCE_2 _ref2 = new XECM_M_REFERENCE_2();
            
        //    try
        //    {
        //        if (param != null && param.data != null && param.data.Count > 0)
        //        {
        //            foreach (XECM_M_REFERENCE itemRef in param.data)
        //            {
        //                _listRef2.AddRange(MapperServices.XECM_M_REFERENCE(XECM_M_REFERENCE_SERVICE.GetByItem(MapperServices.XECM_M_REFERENCE(itemRef))));
        //            }
        //        }

        //        Results.data = _listRef2;
        //        Results.status = "S";
        //    }
        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }
        //    return Results;
        //}
    }
}
