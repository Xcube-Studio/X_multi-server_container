using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using X_multi_server_container.Tools;

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
            recentListView.ItemsSource = Data.HistoryList;
        }
        #region 最近       
        private void recentListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ProcessContainer processContainerPage = new ProcessContainer();
                var item = recentListView.SelectedItem as HistoryModel;
                processContainerPage.StPar = JObject.Parse(File.ReadAllText(item.subtitle + "\\" + item.title));
                //    throw new Exception(processContainerPage.StPar.ToString());
                PageManager.AddPage(processContainerPage, "进程启动器");
            }
            catch (Exception err) { Tools.DialogAPI.MessageBoxShow("打开失败！", err.ToString() ); }
        }
        //模型
        #endregion
        private void OpenSlnButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { InitialDirectory = Environment.CurrentDirectory, Filter = "XMSC启动文件(*.xmsc)|*.xmsc", Title = "打开启动方案" };
            if (fileDialog.ShowDialog(Application.Current.MainWindow) == true)
            {
                CreateSolution createPage = new CreateSolution();
                createPage.LoadFromFile(fileDialog.FileName);
                createPage.Title.Text = "已打开启动方案";
                PageManager.ReplacePage((DataContext as PageItemModel).uuid, createPage, Path.GetFileName(fileDialog.FileName));
            }
        }
        private void BDSTemplateButton_Click(object sender, RoutedEventArgs e)
        {
            CreateSolution createPage = new CreateSolution();
            createPage.Title.Text = "BDS启动模板";
            createPage.targetPath.Text = "mc_start.bat";
            PageManager.ReplacePage((DataContext as PageItemModel).uuid, createPage, "新建启动方案(BDS模板)");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string uuid = (sender as Button).Tag.ToString();
            int i = Data.HistoryList.ToList().FindIndex(l => l.uuid == uuid);
            Data.HistoryListRemove(i);
        }
    }
}
