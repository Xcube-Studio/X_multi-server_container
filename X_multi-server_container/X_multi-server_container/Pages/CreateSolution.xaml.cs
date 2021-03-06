﻿using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using X_multi_server_container.Tools;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace X_multi_server_container.Pages
{
    /// <summary>
    /// CreateSolution.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSolution : Page
    {
        public ObservableCollection<LogFilterModel> logFilters = new ObservableCollection<LogFilterModel>();
        public CreateSolution()
        {
            InitializeComponent();
            pubblishedTemplate.SelectionChanged += pubblishedTemplate_SelectionChanged;
            LogFilderList.ItemsSource = logFilters;
            LogSettingRoot.Height = 0;
        }
        #region Main
        public void LoadFromFile(string path)
        {
            SubTitle.Visibility = Visibility.Visible;
            SubTitle.Text = "位于:" + path;
            try
            {
                LoadFromConfig(JObject.Parse(File.ReadAllText(path)));
                var addModel = new HistoryModel(Path.GetFileName(path), Path.GetDirectoryName(path));
                int find_i = Data.HistoryList.ToList().FindIndex(l => l.title == addModel.title && l.subtitle == addModel.subtitle);/*&&l.StartINFO.ToString() == addModel.StartINFO.ToString()*/
                if (find_i != -1)
                {
                    Data.HistoryListRemove(find_i);
                }
                Data.HistoryListAdd(addModel);
            }
            catch (Exception err)
            { Tools.DialogAPI.MessageBoxShow("打开启动方案失败", err.ToString()); }
        }
        public void LoadFromConfig(JObject config)
        {
            targetPath.Text = config["basicFilePath"].ToString();
            outputEncodingCB.Text = config["OutPutEncoding"].ToString();
            exitCMD.Text = config["ExitCMD"].ToString();
            pubblishedTemplate.SelectedIndex = config.Value<int>("Type");
            if (config["WebsocketAPI"].Type == JTokenType.Null)
            {
                WSAPIToggle.IsChecked = false;
            }
            else
            {
                Match wsInfo = Regex.Match(config["WebsocketAPI"].ToString(), @"ws://0\.0\.0\.0:?(?<port>\d{1,5})/?(?<ep>.*)");
                portTB.Text = wsInfo.Groups["port"].Success ? wsInfo.Groups["port"].ToString() : "";
                endpointTB.Text = wsInfo.Groups["ep"].Success ? wsInfo.Groups["ep"].ToString() : "";
                wsPwd.Text = config["WebsocketPassword"].ToString();
            }
            if (config.ContainsKey("InPutEncoding"))
            {
                InPutEncodingConverntCB.IsChecked = true;
                inputEncodingCBF.Text = config["InPutEncoding"].Value<string>("From");
                inputEncodingCBT.Text = config["InPutEncoding"].Value<string>("To");
            }
            if (config.ContainsKey("LogAPI"))
            {
                LogFileName.Text = Path.GetFileName(config["LogAPI"].Value<string>("Path"));
                LogDirPath.Text = Path.GetDirectoryName(config["LogAPI"].Value<string>("Path"));
                if (config["LogAPI"].Value<bool>("Enable"))
                {
                    foreach (var filter in ((JArray)config["LogAPI"]["Filters"]))
                        logFilters.Add(new LogFilterModel(filter.Value<int>("Type"), filter.Value<string>("Value")));
                    void LogAPIToggle_Loaded(object sender, RoutedEventArgs e)
                    {
                        LogAPIToggle.IsChecked = true;
                        LogAPIToggle.Loaded -= LogAPIToggle_Loaded;
                    }
                    LogAPIToggle.Loaded += LogAPIToggle_Loaded;
                }
            }
        }


        private void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (Directory.Exists(targetPath.Text)) fileDialog.InitialDirectory = targetPath.Text;
            if (File.Exists(targetPath.Text)) { fileDialog.InitialDirectory = Path.GetDirectoryName(targetPath.Text); fileDialog.FileName = targetPath.Text; }
            fileDialog.Title = "请选择运行的程序";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == true)
            {
                targetPath.Text = fileDialog.FileName;
            }
        }
        private void LaunchInNewTabButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessContainer processContainerPage = new ProcessContainer();
            processContainerPage.config = GetConfigJson();

            PageManager.AddPage(processContainerPage, "进程启动器");
        }
        private void SaveSlnButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "将启动方案保存至",
                Filter = "XMSC启动文件(*.xmsc)|*.xmsc",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string saveFileName = saveFileDialog.FileName;
                    if (!Regex.IsMatch(Path.GetExtension(saveFileName), @"\.xmsc$"))
                    {
                        saveFileName += ".xmsc";
                    }
                    using (TaskDialog dialog = new TaskDialog())
                    {
                        dialog.WindowTitle = "确认保存";
                        dialog.MainInstruction = "确认保存";
                        dialog.Content = "目标路径：\n" + saveFileName;
                        dialog.ExpandedInformation = "将文件保存为启动方案后，可以通过启动页的\"打开启动方案\"来加载！";
                        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
                        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Cancel));
                        if (dialog.ShowDialog(Application.Current.MainWindow).ButtonType == ButtonType.Ok)
                        {
                            try
                            {
                                File.WriteAllText(saveFileName, GetConfigJson().ToString());
                                Tools.DialogAPI.MessageBoxShow("保存成功", "文件已保存至" + saveFileName);
                                Data.HistoryListAdd(new HistoryModel(Path.GetFileName(saveFileName), Path.GetDirectoryName(saveFileName)));
                            }
                            catch (Exception err)
                            {
                                Tools.DialogAPI.MessageBoxShow("保存失败", "错误信息\n" + err.ToString());
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }
        private JObject GetConfigJson()
        {
            JObject config = new JObject() {
                new JProperty("basicFilePath", targetPath.Text),
                new JProperty("OutPutEncoding",   outputEncodingCB.Text),
                new JProperty("Type",   pubblishedTemplate.SelectedIndex),
                new JProperty("WebsocketAPI",   WSAPIToggle.IsChecked==true?"ws://0.0.0.0" + (portTB.Text.Length > 0 ? ":" : "") + portTB.Text + "/" + endpointTB.Text:null),
                new JProperty("WebsocketPassword", WSAPIToggle.IsChecked == true ? wsPwd.Text : ""),
                new JProperty("ExitCMD", exitCMD.Text),
                new JProperty("ShowRebootButton",rbtshow.IsChecked==true),
                new JProperty("LogAPI",new JObject (){
                    new JProperty("Enable", LogAPIToggle.IsChecked == true),
                    new JProperty("Path", LogDirPath.Text+( LogDirPath.Text.EndsWith("\\")?"":"\\")+LogFileName.Text),
                    new JProperty("Filters",new JArray()),
                }),
            };
            if (InPutEncodingConverntCB.IsChecked == true)
            {
                config.Add(new JProperty("InPutEncoding", new JObject {
                    new JProperty("From", inputEncodingCBF.Text   ),
                    new JProperty("To", inputEncodingCBT.Text  )
                }));
            }
            if (LogAPIToggle.IsChecked == true)
            {
                foreach (var filter in logFilters)
                {
                    ((JArray)config["LogAPI"]["Filters"]).Add(new JObject { new JProperty("Type", filter.Type), new JProperty("Value", filter.Value) });
                }
            }
            return config;
        }
        #endregion
        #region WSAPI
        private void pubblishedTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pubblishedTemplate.SelectedIndex)
            {
                case 0:
                    InPutEncodingConverntCB.IsChecked = false;
                    outputEncodingCB.Text = "UTF-8";
                    inputEncodingCBF.Text = "UTF-8";
                    inputEncodingCBT.Text = "GBK";
                    break;
                case 1:
                case 2:
                    InPutEncodingConverntCB.IsChecked = true;
                    outputEncodingCB.Text = "UTF-8";
                    inputEncodingCBF.Text = "UTF-8";
                    inputEncodingCBT.Text = "GBK";
                    break;
                default:
                    break;
            }
        }
        private void portTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Match match = Regex.Match(portTB.Text, "[^\\d]");
                if (match.Success)
                    portTB.Text = Regex.Replace(portTB.Text, "[^\\d]", "");
                int port = int.Parse(portTB.Text);
                if (port < 1 || port > 65535)
                {
                    int cIndex = portTB.SelectionStart;
                    portTB.Text = (Math.Max(Math.Min(port, 65535), 1)).ToString();
                    portTB.SelectionStart = cIndex - 1;
                }
            }
            catch (Exception) { }
            RefreshPreview();
        }
        private void endpointTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            //    Match match = Regex.Match(endpointTB.Text, "[^\\d]");
            //    if (match.Success)
            //        endpointTB.Text = Regex.Replace(endpointTB.Text, "[^\\d]", ""); 
            RefreshPreview();
        }
        private void RefreshPreview()
        {
            try
            {
                PreviewAddressTB.Text = "ws://127.0.0.1" + (portTB.Text.Length > 0 ? ":" : "") + portTB.Text + "/" + endpointTB.Text;
            }
            catch (Exception) { }
        }
        #endregion

        private void AddLogFilterButton_Click(object sender, RoutedEventArgs e)
        {
            logFilters.Add(new LogFilterModel());
            WebsocketAPIRootStoryBoard();
        }
        private void WebsocketAPIRootStoryBoard()
        {
            Task.Run(() =>
            {
                Thread.Sleep(50);
                Dispatcher.Invoke(() =>
                {
                    Storyboard storyboard = new Storyboard();
                    DoubleAnimationUsingKeyFrames keyFramesAnimation = new DoubleAnimationUsingKeyFrames();
                    keyFramesAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(LogSettingPanel.ActualHeight, TimeSpan.FromSeconds(0.3)));
                    Storyboard.SetTarget(keyFramesAnimation, LogSettingRoot);
                    Storyboard.SetTargetProperty(keyFramesAnimation, new PropertyPath("(FrameworkElement.Height)"));
                    storyboard.Children.Add(keyFramesAnimation);
                    storyboard.Begin();
                });
            });
        }
        private void DelLogFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logFilters.Remove(logFilters.First(l => l.Uuid == ((Button)sender).Tag.ToString()));
                WebsocketAPIRootStoryBoard();
            }
            catch (Exception) { }
        }

        private void SelectLogDirButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            try
            {
                dialog.SelectedPath = Path.GetFullPath(Regex.Replace(LogDirPath.Text , @"^~\\?","") );
            }
            catch (Exception) { } 
            dialog.Description = "选择保存日志的文件夹";
            dialog.UseDescriptionForTitle = true;
            if ((bool)dialog.ShowDialog(Application.Current.MainWindow))
            {
                LogDirPath.Text = dialog.SelectedPath;
            }
        }
    }
}
