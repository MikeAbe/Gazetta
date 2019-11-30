using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gazzetta.Models;
using PagedList;

namespace Gazzetta.ViewModels
{
    public class SearchResultsViewModel
    {
//        public List<Book> Books { get; set; }
//        public List<Magazine> Magazines { get; set; }

        public IPagedList<Book> Books { get; set; }
        public IPagedList<Magazine> Magazines { get; set; }
    }
}