using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace DAL.Model
{
    public class REQUEST_MODEL
    {
        public int MOCKUP_SUB_ID_CHECK { get; set; }
        public int ARTWORK_SUB_ID_CHECK { get; set; }

        public bool validate { get; set; }
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<COLUMN_MODEL> columns { get; set; }
        public List<ORDER_MODEL> order { get; set; }
        public SEARCH_MODEL search { get; set; }

        public REQUEST_MODEL()
        {
            validate = true;
            draw = 1;
            start = 0;
            length = 10;
            //search = new SEARCH_MODEL();
            //columns = new List<COLUMN_MODEL>();
            //order = new List<ORDER_MODEL>();
        }
    }

    public class RESULT_MODEL
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public string status { get; set; }
        public string msg { get; set; }
        public RESULT_MODEL()
        {
            msg = "";
            recordsTotal = 0;
            recordsFiltered = 0;
        }
    }

    public class RESULT_ENCRYPTION_MODEL
    {
        public string str { get; set; }
        public RESULT_ENCRYPTION_MODEL()
        {
            str = "";
        }
    }
    public class COLUMN_MODEL
    {
        public SEARCH_MODEL search { get; set; }
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
    }

    public class SEARCH_MODEL
    {
        public string value { get; set; }
        public string regex { get; set; }
    }

    public class ORDER_MODEL
    {
        public int column { get; set; }
        public string dir { get; set; }
    }

    public static class extensionmethods
    {
        public static IQueryable<T> OrderByField<T>(this IQueryable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }
    }
}
