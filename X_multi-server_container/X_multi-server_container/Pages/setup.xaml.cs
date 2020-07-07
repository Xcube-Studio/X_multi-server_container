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
    /// Interaction logic for setup.xaml
    /// </summary>
    public partial class Setup : Page
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void OpenSlnButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BDSTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSolution createPage = new CreateSolution();
            createPage.Title.Text = "BDS启动模板";
            createPage.targetPath.Text = "mc_start.bat";
            PageManager.ReplacePage((DataContext as PageItemModel).uuid, createPage, "新建启动方案(BDS模板)");
        }
    }
}
