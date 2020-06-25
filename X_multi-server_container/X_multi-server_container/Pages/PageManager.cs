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
            pages.Add(UUIDTemp, page);
            var PageInfo = new PageItemModel() { PageTitle = title, PageSource = page, uuid = UUIDTemp };
            UUIDTemp = CreateNewUUID;
            page.DataContext = PageInfo;
            PageItems.Add(PageInfo);
            return page;
        }
        public static Page GetPage(string uuid) => PageItems.First(l => l.uuid == uuid).PageSource;
        public static int ClosePage(string uuid)
        {
            pages[uuid].Content = null;
            pages.Remove(uuid);
            int index = PageItems.IndexOf(PageItems.First(l => l.uuid == uuid));
            PageItems.RemoveAt(index);
            GC.Collect();
            return index;
        }
        public static int ReplacePage(string uuid, Page page, string title)
        {
            //清除旧Page
            pages[uuid].Content = null;
            //创建新Page的信息
            var PageInfo = new PageItemModel() { PageTitle = title, PageSource = page, uuid = uuid };
            page.DataContext = PageInfo;
            int index = PageItems.IndexOf(PageItems.First(l => l.uuid == uuid));
            PageItems[index] = PageInfo;
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                (Application.Current.MainWindow as MainWindow).PageContainer.Navigate(page);
                try { (Application.Current.MainWindow as MainWindow).PageContainer.RemoveBackEntry(); } catch { }
                (Application.Current.MainWindow as MainWindow).ListView_Page.SelectedIndex = index;
            });
            GC.Collect();
            //返回Page的Index
            return index;
        }

        public static ObservableCollection<PageItemModel> PageItems = new ObservableCollection<PageItemModel>();
    }
    public class PageItemModel : INotifyPropertyChanged
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
        private string _uuid;
        public string uuid
        {
            get { return _uuid; }
            set { _uuid = value; FirePropertyChanged("uuid"); }
        }
        public virtual event PropertyChangedEventHandler PropertyChanged;
        public virtual void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
