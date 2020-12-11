using System;
using System.Collections.Generic;
using System.Linq;

namespace aspCart.Web.Extensions
{
    public static class LinqExtensions
    {
        public static List<T> Parents<T>(this IList<T> list, T current, Func<T, Guid> getId, Func<T, Guid> getPid)
        {
            List<T> returnlist = new List<T>();

            T temp = list.FirstOrDefault(x => getPid(current) == getId(x));
            while (temp != null)
            {
                returnlist.Add(temp);
                current = temp;
                temp = list.FirstOrDefault(x => getPid(current) == getId(x));
            }

            return returnlist;
        }
    }
}