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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        #region Main   
        private void WL(string value)
        {
            Dispatcher.Invoke(() =>
            {
                Cos.AppendText(value + "\n");
                Cos.ScrollToEnd();
            });
        }
        private Process p;
        int pid = 0;
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string bdspath = programmarPath.Text;
            _ = Task.Run(() =>
            {
                p = new Process();
                p.StartInfo.FileName = bdspath;//控制台程序的路径
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(bdspath);

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                pid = p.Id;
                int pidn = p.Id;
                while (pid == pidn)
                {
                    try
                    {
                        p.StandardInput.Flush();
                        var readtask = p.StandardOutput.ReadLineAsync();
                        readtask.Wait(1000);
                        string result = Encoding.UTF8.GetString(Encoding.Default.GetBytes(readtask.Result));
                        WL(result);
                     }
                    catch (Exception) { }
                }
                WL("》EXIT《");
                p.Close();
                p.Kill();
            });
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                p.StandardInput.WriteLine(Input.Text);
            }
            catch (Exception) { }
        }
        #endregion

    }
}
