using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace X_multi_server_container
{
    public static class PageManager
    {
        public static List<Page> pages = new List<Page>();
        public static Page AddPage(Page page)
        {
            page.Tag = pages.Count;
            pages.Add(page);
            return page;
        }
        public static void ClosePage(Page page)
        {
            page = null;
            pages.Remove(page);
            GC.Collect();
            //Dispatcher.CurrentDispatcher.Invoke
        }
    }
}
