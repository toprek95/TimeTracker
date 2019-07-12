using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTracker.Models
{
    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Page { get; set; } //Trenutna stranica
        public int PageSize { get; set; } //Velicina stranice
        public int TotalCount { get; set; } //Ukupan broj elemenata u bazi za prikazivanje
        public int TotalPages => (int) Math.Ceiling(TotalCount / (decimal) PageSize);

    }
}
