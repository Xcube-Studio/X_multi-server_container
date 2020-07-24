using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;


namespace X_multi_server_container.Pages
{
    /// <summary>
    /// CreateSolution.xaml 的交互逻辑
    /// </summary>
    public partial class CreateSolution : Page
    {
        public CreateSolution()
        {
            InitializeComponent();
            pubblishedTemplate.SelectionChanged += pubblishedTemplate_SelectionChanged;
        }
        public void LoadFromFile(string path)
        {
            SubTitle.Visibility = Visibility.Visible;
            SubTitle.Text = "位于:" + path;
            try
            {
                LoadFromConfig(JObject.Parse(File.ReadAllText(path)));
            }
            catch (Exception err)
            { MessageBoxShow("打开启动方案失败", err.ToString()); }
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
            processContainerPage.StPar = GetConfigJson();

            PageManager.AddPage(processContainerPage, "进程启动器");
        }
        private void MessageBoxShow(string title, string content)
        {
            using (TaskDialog dialog = new TaskDialog())
            {
                dialog.WindowTitle = title;
                dialog.MainInstruction = title;
                dialog.Content = content;
                dialog.Buttons.Add(new TaskDialogButton(ButtonType.Close));
                dialog.ShowDialog(Application.Current.MainWindow);
            }
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
                                MessageBoxShow("保存成功", "文件已保存至" + saveFileName);
                            }
                            catch (Exception err)
                            {
                                MessageBoxShow("保存失败", "错误信息\n" + err.ToString());
                            }
                        }
                    }
                }
                catch (Exception) { }
            }
        }
        private void Toggle_Checked(object sender, RoutedEventArgs e) => ((TextBlock)((CheckBox)sender).Content).Text = "开";
        private void Toggle_Unchecked(object sender, RoutedEventArgs e) => ((TextBlock)((CheckBox)sender).Content).Text = "关";
        private JObject GetConfigJson()
        {
            JObject config = new JObject() {
                new JProperty("basicFilePath", targetPath.Text),
                new JProperty("OutPutEncoding",   outputEncodingCB.Text),
                new JProperty("WebsocketAPI",   WSAPIToggle.IsChecked==true?"ws://0.0.0.0" + (portTB.Text.Length > 0 ? ":" : "") + portTB.Text + "/" + endpointTB.Text:null),
              new JProperty("WebsocketPassword", WSAPIToggle.IsChecked == true ? wsPwd.Text : ""),
                new JProperty("ExitCMD", exitCMD.Text),
                new JProperty("ShowRebootButton",rbtshow.IsChecked==true),
                new JProperty("Type",   pubblishedTemplate.SelectedIndex)
            };

            if (InPutEncodingConverntCB.IsChecked == true)
            {
                config.Add(new JProperty("InPutEncoding", new JObject {
                    new JProperty("From", inputEncodingCBF.Text   ),
                    new JProperty("To", inputEncodingCBT.Text  )
                }));
            }
            return config;
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

        private void pubblishedTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pubblishedTemplate.SelectedIndex)
            {
                case 0:
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
    }
}
