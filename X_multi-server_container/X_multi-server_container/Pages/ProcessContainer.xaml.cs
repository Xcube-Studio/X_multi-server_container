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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SourceChord.FluentWPF;

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
        #region Main   
        private Process p;
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            /*
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
             */
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Button.Click += Button_Click;
            if (p == null)
            {
                p = new Process();
                p.StartInfo.FileName = "cmd";
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(p.StartInfo.FileName);
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
            }

            Storyboard sb = new Storyboard();//首先实例化一个故事板
            DoubleAnimation yd5 = new DoubleAnimation(Board.ActualWidth, 0, new Duration(TimeSpan.FromSeconds(0.35)));//浮点动画定义了开始值和起始值
            Board.RenderTransform = new TranslateTransform();//在二维x-y坐标系统内平移(移动)对象
            yd5.AutoReverse = false;//设置可以进行反转
            Storyboard.SetTarget(yd5, Board);//绑定动画为这个按钮执行的浮点动画
            Storyboard.SetTargetProperty(yd5, new PropertyPath("RenderTransform.X"));//依赖的属性
            sb.Children.Add(yd5);//向故事板中加入此浮点动画
            sb.Begin();//播放此动画
            a = true;
        }
        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Cos.AppendText(e.Data + Environment.NewLine);
                Cos.ScrollToEnd();
            });
        }
        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Cos.AppendText(e.Data + Environment.NewLine);
                Cos.ScrollToEnd();
            });
        }
        bool a;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (a)
            {
                Storyboard sb = new Storyboard();//首先实例化一个故事板
                DoubleAnimation yd5 = new DoubleAnimation(0, Board.ActualWidth, new Duration(TimeSpan.FromSeconds(0.35)));//浮点动画定义了开始值和起始值
                Board.RenderTransform = new TranslateTransform();//在二维x-y坐标系统内平移(移动)对象
                yd5.AutoReverse = false;//设置可以进行反转
                Storyboard.SetTarget(yd5, Board);//绑定动画为这个按钮执行的浮点动画
                Storyboard.SetTargetProperty(yd5, new PropertyPath("RenderTransform.X"));//依赖的属性
                sb.Children.Add(yd5);//向故事板中加入此浮点动画
                sb.Begin();//播放此动画
                a = true; a = false;
            }
            else
            {
                Storyboard sb = new Storyboard();//首先实例化一个故事板
                DoubleAnimation yd5 = new DoubleAnimation(Board.ActualWidth, 0, new Duration(TimeSpan.FromSeconds(0.35)));//浮点动画定义了开始值和起始值
                Board.RenderTransform = new TranslateTransform();//在二维x-y坐标系统内平移(移动)对象
                yd5.AutoReverse = false;//设置可以进行反转
                Storyboard.SetTarget(yd5, Board);//绑定动画为这个按钮执行的浮点动画
                Storyboard.SetTargetProperty(yd5, new PropertyPath("RenderTransform.X"));//依赖的属性
                sb.Children.Add(yd5);//向故事板中加入此浮点动画
                sb.Begin();//播放此动画
                a = true;
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Button.Click -= Button_Click;
        }
    }
}
