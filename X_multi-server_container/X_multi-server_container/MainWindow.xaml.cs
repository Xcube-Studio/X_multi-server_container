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
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
             PageContainer.Navigate(PageManager.AddPage(new Page(), "Page#" +PageManager.UUIDTemp));
        }

        private void Button_ClosePage_Click(object sender, RoutedEventArgs e)
        {
            //Button button = new Button();
            //button.Click += (object _sender, RoutedEventArgs _e) => { };
            try
            {
                PageManager.ClosePage((sender as Button).Tag as string);
            }
            catch (Exception)
            { } 
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
    }

}
