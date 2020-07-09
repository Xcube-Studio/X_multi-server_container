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
        /// <summary>
        /// UUID缓存
        /// </summary>
        public static string UUIDTemp = Guid.NewGuid().ToString();
        /// <summary>
        /// 随机生成新的UUID
        /// </summary>
        private static string CreateNewUUID => Guid.NewGuid().ToString();
        /// <summary>
        /// 添加新的页面
        /// </summary>
        /// <param name="page">页面源，使用new Pages.xxx()初始化</param>
        /// <param name="title">页面标题</param>
        /// <returns>返回初始化后的Page</returns>
        public static Page AddPage(Page page, string title)
        {
            pages.Add(UUIDTemp, page);
            var PageInfo = new PageItemModel() { PageTitle = title, PageSource = page, uuid = UUIDTemp };
            UUIDTemp = CreateNewUUID;
            page.DataContext = PageInfo;
            PageItems.Add(PageInfo);
            Dispatcher.CurrentDispatcher.Invoke(() => (Application.Current.MainWindow as MainWindow).ListView_Page.SelectedIndex = PageItems.Count - 1);
            return page;
        }
        /// <summary>
        /// 获取Page
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <returns>返回通过uuid找到的Page</returns>
        public static Page GetPage(string uuid) => PageItems.First(l => l.uuid == uuid).PageSource;
        /// <summary>
        /// 关闭Page
        /// </summary>
        /// <param name="uuid">需要关闭的Page的UUID</param>
        /// <returns>返回关闭的Page的位置</returns>
        public static int ClosePage(string uuid)
        {
            if (pages[uuid] is Pages.ProcessContainer)
                ((Pages.ProcessContainer)pages[uuid]).DisposePage();//如果是进程容器则对进程和ws进行关闭
            pages[uuid].Content = null;
            pages.Remove(uuid);
            int index = PageItems.IndexOf(PageItems.First(l => l.uuid == uuid));
            PageItems.RemoveAt(index);
            GC.Collect();
            return index;
        }
        /// <summary>
        /// 替换Page
        /// </summary>
        /// <param name="uuid">目标Page的UUID</param>
        /// <param name="page">替换成的Page，使用new Pages.xxx()初始化</param>
        /// <param name="title">替换后的Page标题</param>
        /// <returns>返回替换后的Page的位置</returns>
        public static int ReplacePage(string uuid, Page page, string title)
        {
            //清除旧Page
            pages[uuid].Content = null;
            //创建新Page的信息
            var PageInfo = new PageItemModel() { PageTitle = title, PageSource = page, uuid = uuid };
            page.DataContext = PageInfo;
            int index = PageItems.IndexOf(PageItems.First(l => l.uuid == uuid));
            PageItems[index] = PageInfo;
            Dispatcher.CurrentDispatcher.Invoke(() => (Application.Current.MainWindow as MainWindow).ListView_Page.SelectedIndex = index);
            GC.Collect();
            //返回Page的Index
            return index;
        }
        /// <summary>
        /// PageItemModel示例组,绑定了界面的Source/DataContnnt
        /// </summary>
        public static ObservableCollection<PageItemModel> PageItems = new ObservableCollection<PageItemModel>();
        /// <summary>
        /// 使用Dictionary排列的Page组
        /// 通过PageItems获取的副本
        /// </summary>
        public static Dictionary<string, Page> pages
        {
            get
            {
                return PageItems.ToDictionary(l => l.uuid, l => l.PageSource);
            }
        }
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
