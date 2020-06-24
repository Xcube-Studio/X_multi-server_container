using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace X_multi_server_container
{
    public static class PageManager
    {
        public static Dictionary<string, Page> pages = new Dictionary<string, Page>();
        public static string UUIDTemp = Guid.NewGuid().ToString();
        public static string CreateNewUUID => Guid.NewGuid().ToString();
        public static Page AddPage(Page page, string title)
        {
            //page.Tag = pages.Count;
            UUIDTemp = CreateNewUUID;
            pages.Add(UUIDTemp, page);
            PageItems.Add(new PageItem() { PageTitle = title, PageSource = page, uuid = UUIDTemp });
            return page;
        }
        public static void ClosePage(string uuid)
        {
            pages[uuid] = null;
            pages.Remove(uuid);
            PageItems.Remove(PageItems.First(l => l.uuid == uuid));
            GC.Collect();
            //Dispatcher.CurrentDispatcher.Invoke
        }
        public static ObservableCollection<PageItem> PageItems = new ObservableCollection<PageItem>();
    }
    public class PageItem : INotifyPropertyChanged
    {

        private string _PageTitle = "Page";
        public string PageTitle
        {
            get { return _PageTitle; }
            set { _PageTitle = value; FirePropertyChanged("PageTitle"); }
        }
        private Page _PageSource = new Page();
        public Page PageSource
        {
            get { return _PageSource; }
            set { _PageSource = value; FirePropertyChanged("PageSource"); }
        }
        //Tag用的标记序号
        private string _uuid;
        public string uuid
        {
            get { return _uuid; }
            set { _uuid = value; FirePropertyChanged("uuid"); }
        }
        //private Action<object, RoutedEventArgs> _CloseClick;
        //public Action<object, RoutedEventArgs> CloseClick
        //{
        //    get { return _CloseClick; }
        //    set { _CloseClick = value; FirePropertyChanged("CloseClick"); }
        //}
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public virtual void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
