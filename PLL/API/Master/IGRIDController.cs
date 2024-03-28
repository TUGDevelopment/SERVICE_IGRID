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
using System.Data;

namespace PLL.API
{
    public class IGRIDController : ApiController
    {
        //[Route("api/master/masterigrid")]
        //public DataSet buildresult([FromUri] MASTER_REQUEST param)
        //{
        //    //DataSet ds = CNService.buildmasterigrid(param.Type);
        //    DataSet ds = new DataSet();
        //    //var model = new List<dynamic>(table.Rows.Count);
        //    return ds;
        //    //    return CNService.buildmasterigrid(string.Format("{0}", param.Type));
        //    //if (table.Rows.Count > 0)
        //    //{
        //    //    foreach (DataRow row in table.Rows)
        //    //    {
        //    //        var obj = (IDictionary<string, object>)new ExpandoObject();
        //    //        foreach (DataColumn col in table.Columns)
        //    //        {
        //    //            obj.Add(col.ColumnName, row[col.ColumnName]);
        //    //        }
        //    //        model.Add(obj);
        //    //    }
        //    //}
        //}

        //[Route("api/master/getrequiredfield")]
        //public MaterialClass_RESULT GetRequiredField([FromUri] MaterialClass_REQUEST param)
        //{
        //    return IGRIDHelper.GetRequiredField(param);
        //}

        [Route("api/master/getigriduser")]
        public ulogin_RESULT GetUser([FromUri] ulogin_REQUEST param)
        {
            return IGRIDHelper.GetUser(param);
        }


    }
}
