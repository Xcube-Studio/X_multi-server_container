using System;
using System.Collections.Generic;
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

namespace X_multi_server_container.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void CreateProcessContainer_Button_Click(object sender, RoutedEventArgs e)
        {
            PageManager.ReplacePage((DataContext as PageItemModel).uuid, new ProcessContainer(), "进程容器");
        }

        private void GotoSetupPageButton_Click(object sender, RoutedEventArgs e)
        {
            //PageManager.ReplacePage(
            //  ((Application.Current.MainWindow as MainWindow).ListView_Page.SelectedItem as PageItemModel).uuid,
            //   new Pages.Setup(), "启动页");     
            PageManager.ReplacePage((DataContext as PageItemModel).uuid, new Setup(), "启动页");
        }
    }
}
