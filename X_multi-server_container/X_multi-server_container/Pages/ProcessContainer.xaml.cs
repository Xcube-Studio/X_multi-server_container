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
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string bdspath = programmarPath.Text;
            Task.Run(() =>
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
                p.BeginErrorReadLine(); p.BeginOutputReadLine();
                p.OutputDataReceived += P_OutputDataReceived;
                p.ErrorDataReceived += P_ErrorDataReceived;
                p.Exited += P_Exited;
            });
        }

        private void P_Exited(object sender, EventArgs e)
        {
            
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Cos.AppendText(e.Data + "\r\n");
                Cos.ScrollToEnd();
            }));
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Cos.AppendText(e.Data + "\r\n");
                Cos.ScrollToEnd();
            }));
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
