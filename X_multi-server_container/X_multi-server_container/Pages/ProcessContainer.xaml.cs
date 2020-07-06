using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Newtonsoft.Json.Linq;
using SourceChord.FluentWPF;

namespace X_multi_server_container.Pages
{
    /// <summary>
    /// ProcessContainer.xaml 的交互逻辑
    /// </summary>
    public partial class ProcessContainer : Page
    {
        #region 启动参数
        public JObject StPar = new JObject() {
            new JProperty("basicFilePath", "cmd")
        };
        #endregion
        public ProcessContainer()
        {
            InitializeComponent();
        }
        #region Main   
        private Process p;
        public void WriteLine(object content)
        {
            Cos.AppendText(content.ToString().TrimEnd(' ', '\n', '\r') + Environment.NewLine);
            CosViewer.ScrollToEnd();
        }

        //private void StartButton_Click(object sender, RoutedEventArgs e)
        //{
        //    /*
        //    string bdspath = programmarPath.Text;
        //    StartInfoText.Text = "正在启动";
        //    if (pState != false)
        //    {
        //        StartInfoText.Text = "正在终止\n" + p.Id.ToString();
        //        p.Close();
        //         pState = false;
        //        StartInfoText.Text = "已终止";
        //        StartButton.Content = "启动";
        //         return;
        //    }
        //    pState = null;
        //     StartButton.Content = "终止";
        //    _ = Task.Run(() =>
        //     {
        //         try
        //         {
        //             p = new Process();
        //             p.StartInfo.FileName = bdspath;//控制台程序的路径
        //             p.StartInfo.WorkingDirectory = Path.GetDirectoryName(bdspath);
        //             p.StartInfo.UseShellExecute = false;
        //             p.StartInfo.RedirectStandardOutput = true;
        //             p.StartInfo.RedirectStandardInput = true;
        //             p.StartInfo.RedirectStandardError = true;
        //             p.StartInfo.CreateNoWindow = true;
        //             p.EnableRaisingEvents = true;
        //             p.Start();
        //             p.BeginErrorReadLine();
        //             p.ErrorDataReceived += (_s, _e) => WL(_e.Data);
        //             p.BeginOutputReadLine();
        //             p.OutputDataReceived += (_s, _e) => WL(_e.Data);
        //             p.Exited += (_s, _e) =>
        //            {
        //                WL("---Exited---");
        //                Dispatcher.Invoke(() => StartInfoText.Text = "已退出");
        //                pState = false;
        //            };
        //             Dispatcher.Invoke(() => StartInfoText.Text = "已启动" + p.ProcessName + "\n进程ID:" + p.Id);
        //             pState = true;
        //         }
        //         catch (Exception err)
        //         { Dispatcher.Invoke(() => StartInfoText.Text = "启动失败\n" + err.Message); pState = false; }
        //     });
        //     */
        //}
        #endregion
        #region 输入逻辑
        List<string> cmdHistory = new List<string>() { "" };
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (SendCMDButton.Content.ToString() == "发送")
            {
                string cmd = Input.Text;
#if DEBUG //输入回显
            WriteLine(">" + cmd);
#endif
                int index = int.Parse(Input.Tag.ToString());
                if (index < cmdHistory.Count - 1)
                {
                    string tmpHis = cmdHistory[index];
                    for (int i = index + 1; i < cmdHistory.Count; i++)
                        cmdHistory[i - 1] = cmdHistory[i];
                    cmdHistory[cmdHistory.Count - 1] = tmpHis;
                }
                Input.Clear();
                Input.Tag = cmdHistory.Count;
                cmdHistory.Add("");
                try
                {
                    p.StandardInput.WriteLine(cmd);
                }
                catch (Exception) { }
            }
            else
            {
                HideInputPanel();
            }
        }
        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: UPSend(); break;
                case Key.Down: DOWNSend(); break;
                case Key.Enter: SendCMDButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); ; break;
                default:
                    cmdHistory[int.Parse(Input.Tag.ToString())] = Input.Text;
                    if (Input.Text.Length == 0)
                    {
                        SendCMDButton.Content = "︾";
                        SendCMDButton.FontSize = 21;
                    }
                    else
                    {
                        SendCMDButton.Content = "发送";
                        SendCMDButton.FontSize = 14;
                    }
                    break;
            }
        }
        private void UP_Button_Click(object sender, RoutedEventArgs e) => UPSend();
        private void DOWN_Button_Click(object sender, RoutedEventArgs e) => DOWNSend();
        private void UPSend()
        {
            int index = Math.Max(0, int.Parse(Input.Tag.ToString()) - 1);
            Input.Tag = index;
            Input.Text = cmdHistory[index];
            if (index + 2 == cmdHistory.Count && string.IsNullOrEmpty(cmdHistory.Last()))
                cmdHistory.RemoveAt(index + 1);
        }
        private void DOWNSend()
        {
            int index = int.Parse(Input.Tag.ToString()) + 1;
            if (index == cmdHistory.Count && !string.IsNullOrEmpty(cmdHistory.Last()))
                cmdHistory.Add("");
            index = Math.Min(cmdHistory.Count - 1, index);
            Input.Tag = index;
            Input.Text = cmdHistory[index];
        }
        #endregion
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Button.Click += Button_Click;
            if (p == null)
            {
                p = new Process();
                p.StartInfo.FileName = StPar.Value<string>("basicFilePath");
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(p.StartInfo.FileName);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.EnableRaisingEvents = true;
                p.Start();
                p.BeginErrorReadLine();
                p.ErrorDataReceived += (_s, _e) => Dispatcher.Invoke((() => { WriteLine(_e.Data); }));
                p.BeginOutputReadLine();
                p.OutputDataReceived += (_s, _e) => Dispatcher.Invoke((() => { WriteLine(_e.Data); }));
            }
            Board.Height = 0;
            EchoInputPanel();
        }
        #region 面板动画
        bool a;
        private void EchoInputPanel()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimationUsingKeyFrames keyFramesAnimation = new DoubleAnimationUsingKeyFrames();
            keyFramesAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(33, TimeSpan.FromSeconds(0.35)));
            Storyboard.SetTarget(keyFramesAnimation, Board);
            Storyboard.SetTargetProperty(keyFramesAnimation, new PropertyPath("(FrameworkElement.Height)"));
            storyboard.Children.Add(keyFramesAnimation);
            storyboard.Begin();
            a = true;
        }
        private void HideInputPanel()
        {
            #region 旧的参考
            //Storyboard sb = new Storyboard();//首先实例化一个故事板
            //DoubleAnimation yd5 = new DoubleAnimation(Board.ActualHeight, 0, new Duration(TimeSpan.FromSeconds(0.35)));//浮点动画定义了开始值和起始值
            //Board.RenderTransform = new TranslateTransform();//在二维x-y坐标系统内平移(移动)对象
            //yd5.AutoReverse = false;//设置可以进行反转
            //Storyboard.SetTarget(yd5, Board);//绑定动画为这个按钮执行的浮点动画
            //Storyboard.SetTargetProperty(yd5, new PropertyPath("RenderTransform.Y"));//依赖的属性
            //sb.Children.Add(yd5);//向故事板中加入此浮点动画
            //sb.Begin();//播放此动画
            #endregion
            Storyboard storyboard = new Storyboard();
            DoubleAnimationUsingKeyFrames keyFramesAnimation = new DoubleAnimationUsingKeyFrames();
            keyFramesAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromSeconds(0.35)));
            Storyboard.SetTarget(keyFramesAnimation, Board);
            Storyboard.SetTargetProperty(keyFramesAnimation, new PropertyPath("(FrameworkElement.Height)"));
            storyboard.Children.Add(keyFramesAnimation);
            storyboard.Begin();
            a = false;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (a) HideInputPanel(); else EchoInputPanel();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.Button.Click -= Button_Click;
        }
        #endregion

        private void Cos_KeyUp(object sender, KeyEventArgs e)
        {
            if (a==false)
            {
            EchoInputPanel();
            Input.Focus();
             }
        }
    }
}
