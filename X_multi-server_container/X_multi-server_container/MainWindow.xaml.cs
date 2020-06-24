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
            PageContainer.Navigate(PageManager.AddPage(new Pages.Setup()));
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Button button = new Button();
            button.Tag = (ListView_Page.Items.Count + 1).ToString();
            ListView_Page.Items.Add(button);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {

        }
    }
  
}
