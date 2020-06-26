using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace X_multi_server_container.Pages
{
    /// <summary>
    /// ProcessContainer.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessContainer : Page
    {
        public ProcessContainer()
        {
            InitializeComponent();
        }

        private void FindFileButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".exe";
            ofd.Filter = "exe file|*.exe";
            if (ofd.ShowDialog() == true)
            {
                Console.WriteLine("Selected path:" + ofd.FileName);
                programmarPath.Text = ofd.FileName;
            }
        }

        #region Main   
        private void WL(string value)
        {
            Dispatcher.Invoke(() =>
            {
                Cos.AppendText(value + Environment.NewLine);
                Cos.ScrollToEnd();
            });
        }
        private Process p;
        bool? pState = false;
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string bdspath = programmarPath.Text;
            StartInfoText.Text = "正在启动";
            if (pState != false)
            {
                StartInfoText.Text = "正在终止\n" + p.Id.ToString();
                p.Close();
                 pState = false;
                StartInfoText.Text = "已终止";
                StartButton.Content = "启动";
                 return;
            }
            pState = null;
             StartButton.Content = "终止";
            _ = Task.Run(() =>
             {
                 try
                 {
                     p = new Process();
                     p.StartInfo.FileName = bdspath;//控制台程序的路径
                     p.StartInfo.WorkingDirectory = Path.GetDirectoryName(bdspath);
                     p.StartInfo.UseShellExecute = false;
                     p.StartInfo.RedirectStandardOutput = true;
                     p.StartInfo.RedirectStandardInput = true;
                     p.StartInfo.RedirectStandardError = true;
                     p.StartInfo.CreateNoWindow = true;
                     p.EnableRaisingEvents = true;
                     p.Start();
                     p.BeginErrorReadLine();
                     p.ErrorDataReceived += (_s, _e) => WL(_e.Data);
                     p.BeginOutputReadLine();
                     p.OutputDataReceived += (_s, _e) => WL(_e.Data);
                     p.Exited += (_s, _e) =>
                    {
                        WL("---Exited---");
                        Dispatcher.Invoke(() => StartInfoText.Text = "已退出");
                        pState = false;
                    };
                     Dispatcher.Invoke(() => StartInfoText.Text = "已启动" + p.ProcessName + "\n进程ID:" + p.Id);
                     pState = true;
                 }
                 catch (Exception err)
                 { Dispatcher.Invoke(() => StartInfoText.Text = "启动失败\n" + err.Message); pState = false; }
             });
        }

        List<string> cmdHistory = new List<string>() { "" };
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string cmd = Input.Text;
            Input.Clear();
            Input.Tag = cmdHistory.Count;
            cmdHistory.Add("");
            try
            {
                p.StandardInput.WriteLine(cmd);
            }
            catch (Exception) { }
        }
        #endregion


        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: UPSend(); break;
                case Key.Down: DOWNSend(); break;
                default: cmdHistory[int.Parse(Input.Tag.ToString())] = Input.Text; break;
            }
        }
        private void UP_Button_Click(object sender, RoutedEventArgs e) => UPSend();
        private void DOWN_Button_Click(object sender, RoutedEventArgs e) => DOWNSend();
        private void UPSend()
        {
            int index = Math.Max(0, int.Parse(Input.Tag.ToString()) - 1);
            Input.Tag = index;
            Input.Text = cmdHistory[index];

        }
        private void DOWNSend()
        {
            int index = Math.Min(cmdHistory.Count - 1, int.Parse(Input.Tag.ToString()) + 1);
            Input.Tag = index;
            Input.Text = cmdHistory[index];
        }
    }
}
