﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
#if DEBUG
            recentListView.Items.Add(new HistoryModel("相对路径快速启动BDS", "[DEBUG]运行当前目录下的mc_start.bat", new JObject() {
                new JProperty("basicFilePath",  "mc_start.bat"),
                new JProperty("Encoding", Encoding.UTF8.ToString()),
                new JProperty("WebsocketAPI",   "ws://0.0.0.0:29132/xsbasurXXXgxh"),
                new JProperty("Type",  0)
            }));
#endif
        }
        #region 最近       
        private void recentListView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ProcessContainer processContainerPage = new ProcessContainer();
                processContainerPage.StPar = (recentListView.SelectedItem as HistoryModel).StartINFO;
                PageManager.AddPage(processContainerPage, "进程启动器");
            }
            catch (Exception) { }
        }
        //模型
        public class HistoryModel : INotifyPropertyChanged
        {
            public HistoryModel(string title_, string subtitile_, JObject StartINFO_)
            {
                _title = title_;
                _subtitle = subtitile_;
                _StartINFO = StartINFO_;
            }
            private string _title;
            public string title
            {
                get { return _title; }
                set { _title = value; FirePropertyChanged("title"); }
            }
            private string _subtitle;
            public string subtitle
            {
                get { return _subtitle; }
                set { _subtitle = value; FirePropertyChanged("subtitle"); }
            }
            private JObject _StartINFO;
            public JObject StartINFO
            {
                get { return _StartINFO; }
                set { _StartINFO = value; FirePropertyChanged("StartINFO"); }
            }
            public virtual event PropertyChangedEventHandler PropertyChanged;
            public virtual void FirePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
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
