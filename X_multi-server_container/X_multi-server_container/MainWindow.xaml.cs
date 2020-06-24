using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SourceChord.FluentWPF;

namespace X_multi_server_container
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AcrylicWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            ListView_Page.ItemsSource = PageManager.PageItems;
            PageContainer.Navigate(PageManager.AddPage(new Pages.Setup(), "Welcome"));
            ListView_Page.SelectedIndex = 0;
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            PageContainer.Navigate(PageManager.AddPage(new Pages.Home() { DataContext = new test() { testtxt = PageManager.UUIDTemp } }, "Page#" + PageManager.UUIDTemp));
            ListView_Page.SelectedIndex = ListView_Page.Items.Count - 1;
            try { PageContainer.RemoveBackEntry(); } catch { }
        }
        private void Button_ClosePage_Click(object sender, RoutedEventArgs e)
        {
            if (ListView_Page.Items.Count == 1)
                App.Current.Shutdown();
            try
            {
                int a = ListView_Page.SelectedIndex;
                PageManager.ClosePage((sender as Button).Tag as string);
                ListView_Page.SelectedIndex = a - 1;
            }
            catch (Exception) { }
        }
        private int nextPage = 0;
        private void ListView_Page_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ListView_Page.SelectedIndex >= 0)
                {
                    nextPage = ListView_Page.SelectedIndex;
                    _ = PageContainer.Navigate(PageManager.GetPage((ListView_Page.SelectedItem as PageItemModel).uuid));
                }
            }
            catch (Exception) { ListView_Page.SelectedIndex = nextPage; }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }
    public class test
    {
        public string _testtxt = Guid.NewGuid().ToString();
        public string testtxt
        {
            get { return _testtxt; }
            set { _testtxt = value; }
        }
    }
}
