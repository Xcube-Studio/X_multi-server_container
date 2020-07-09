using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System.Text;
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
        }

        private void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
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
            processContainerPage.StPar = new JObject() {
                new JProperty("basicFilePath", targetPath.Text),
                new JProperty("Encoding", Encoding.UTF8.ToString()),
                new JProperty("WebsocketAPI",   WSAPIToggle.IsChecked==true?"ws://0.0.0.0:29132/xsbasurXXXgxh":null),
                new JProperty("Type",   pubblishedTemplate.SelectedIndex)
            };
            PageManager.AddPage(processContainerPage, "进程启动器");
        }
        /*
         {
            "basicFilePath":"C://xxx",
            "basicModel":0,
            "WebsocketAPI":true
         } 
         */
        private void SaveSlnButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "将启动方案保存至";
            saveFileDialog.Filter = "所有文件(*.mcsln)|*.mcsln";
            if (saveFileDialog.ShowDialog() == true)
            {

            }
        } 
        private void WSAPIToggle_Checked(object sender, RoutedEventArgs e) => ((TextBlock)WSAPIToggle.Content).Text = "开";
        private void WSAPIToggle_Unchecked(object sender, RoutedEventArgs e) => ((TextBlock)WSAPIToggle.Content).Text = "关";
    }
}
