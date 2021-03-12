using System;
using System.Collections.Generic;
using System.Linq;
using aspCart.Core.Domain.Catalog;

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

        public static IEnumerable<Category> OrderByPosition(this IEnumerable<Category> source)
        {
            var categories = new List<Category>();

            var et = source.Where(c => c.Name == "Et Ürünleri").SingleOrDefault();
            categories.Add(et);

            var peynir = source.Where(c => c.Name == "Peynirler").SingleOrDefault();
            categories.Add(peynir);

            var bal = source.Where(c => c.Name == "Bal").SingleOrDefault();
            categories.Add(bal);

            var yag = source.Where(c => c.Name == "Yağlar").SingleOrDefault();
            categories.Add(yag);

            var zeytin = source.Where(c => c.Name == "Zeytinler").SingleOrDefault();
            categories.Add(zeytin);

            var salca = source.Where(c => c.Name == "Salçalar").SingleOrDefault();
            categories.Add(salca);

            var pekmez = source.Where(c => c.Name == "Pekmezler").SingleOrDefault();
            categories.Add(pekmez);

            var baharat = source.Where(c => c.Name == "Baharatlar").SingleOrDefault();
            categories.Add(baharat);

            var kuruMeyve = source.Where(c => c.Name == "Kuru Meyve").SingleOrDefault();
            categories.Add(kuruMeyve);

            var yoreselUrunler = source.Where(c => c.Name == "Yöresel Ürünler").SingleOrDefault();
            categories.Add(yoreselUrunler);

            return categories;
        }
    }
}