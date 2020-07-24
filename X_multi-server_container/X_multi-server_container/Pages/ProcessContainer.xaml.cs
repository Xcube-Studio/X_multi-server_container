using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Newtonsoft.Json.Linq;
using Fleck;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Media;
using X_multi_server_container.Tools;
using System.Threading;
using Timer = System.Timers.Timer;

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
            #region DEBUG
#if DEBUG
            var debugbutton = new Button()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = "DEBUG",
                Margin = new Thickness(0, 0, 20, 0)
            };
            var debugtext = new TextBox()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Text = "DEBUG",
                Margin = new Thickness(0, 30, 20, 0)
            };
            debugbutton.Click += (sender, e) =>
            {
                try
                {
                    debugtext.Text += p.ProcessName + "\n";
                    debugtext.Text += p.SessionId + "\n";
                    debugtext.Text += p.Id + "\n";
                    debugtext.Text += p.HandleCount + "\n";
                    debugtext.Text += p.HasExited + "\n";
                    p.Kill();
                    debugtext.Text += p.HasExited + "\n";
                    debugtext.Text += p.HandleCount + "\n";
                }
                catch (Exception err) { debugtext.Text += "\n" + err.ToString(); }
            };
            ((Grid)this.Content).Children.Add(debugbutton);
            ((Grid)this.Content).Children.Add(debugtext);
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(5000);
                WriteLine("当前参数\n" + StPar.ToString());
            });
#endif 
            #endregion
        }
        #region 启动参数
        public JObject StPar = new JObject{
            new JProperty("basicFilePath", "cmd"),
            new JProperty("OutPutEncoding", Encoding.Default.ToString()),
            new JProperty("Type",-1),
            new JProperty("WebsocketAPI",false)
        };
        #endregion
        #region Main   
        private Process p;
#if DEBUG
        public void WriteLineDEBUG(object content)
        {
            if (content == null) return;
            Dispatcher.Invoke((() =>
            {
                Cos.AppendText("[DEBUG]=>" + content.ToString().TrimEnd(' ', '\n', '\r') + Environment.NewLine);
                CosViewer.ScrollToEnd();
            }));
        }
#endif
        public void WriteLine(object content)
        {
            if (content == null) return;
            Dispatcher.Invoke((() =>
            {
                Cos.AppendText(content.ToString().TrimEnd(' ', '\n', '\r') + Environment.NewLine);
                CosViewer.ScrollToEnd();
            }));
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
        private void Input_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.Key)
                {
                    case Key.Up: UPSend(); break;
                    case Key.Down: DOWNSend(); break;
                    case Key.Enter: if (SendCMDButton.Content.ToString() == "发送") SendCMDButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); break;
                    default:
                        cmdHistory[int.Parse(Input.Tag.ToString())] = Input.Text;
                        break;
                }
            }
            catch (Exception
#if DEBUG
            err
#endif
            )
            {
#if DEBUG
                WriteLineDEBUG("[ERROR]输入逻辑Input_PreviewKeyDown\n" + err);
#endif
            }

        }
        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Input.Text)) cmdHistory[int.Parse(Input.Tag.ToString())] = Input.Text;
                if (Input.Text.Length == 0)
                { SendCMDButton.Content = "︾"; SendCMDButton.FontSize = 21; }
                else
                { SendCMDButton.Content = "发送"; SendCMDButton.FontSize = 14; }
            }
            catch (Exception
#if DEBUG
            err
#endif
            )
            {
#if DEBUG
                WriteLineDEBUG("[ERROR]输入逻辑Input_TextChanged\n" + err);
#endif
            }
        }
        List<string> cmdHistory = new List<string>() { "" };
        private void SendCMD(string cmd)
        {
            if (StPar.ContainsKey("InPutEncoding"))
            {
                p.StandardInput.WriteLine(GetEncoding(StPar["InPutEncoding"].Value<string>("To")).GetString(GetEncoding(StPar["InPutEncoding"].Value<string>("From")).GetBytes(cmd)));
                //cmd = GetEncoding(StPar["InPutEncoding"].Value<string>("To")).GetString(Encoding.GetEncoding(65001).GetBytes(cmd));
            }
            else
                p.StandardInput.WriteLine(cmd);
        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (SendCMDButton.Content.ToString() == "发送")
            {
                string cmd = Input.Text;
#if DEBUG //输入回显
                WriteLineDEBUG("输入命令 >" + cmd);
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
                    SendCMD(cmd);
                }
                catch (Exception) { }
            }
            else
            {
                HideInputPanel();
            }
        }
        private void UP_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UPSend();
            }
            catch (Exception
#if DEBUG
            err
#endif
            )
            {
#if DEBUG
                WriteLineDEBUG("[ERROR]输入逻辑UP_Button_Click\n" + err);
#endif
            }
        }
        private void DOWN_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            { DOWNSend(); }
            catch (Exception
#if DEBUG
            err
#endif
            )
            {
#if DEBUG
                WriteLineDEBUG("[ERROR]输入逻辑DOWN_Button_Click\n" + err);
#endif
            }

        }
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
            #region 界面/进程
            try
            {
                if (!ClearTimer.Enabled)
                {
                    ClearTimer.Elapsed += ClearTimer_Elapsed;
                    ClearTimer.Start();
                }
                MainWindow.Button.Click += Button_Click;
                if (p == null)
                {
                    StartProcess();
                    #region WS
                    try
                    {
                        try { if (webSocketServer == null) webSocketServer = new WebSocketServer(StPar.Value<string>("WebsocketAPI")) { RestartAfterListenError = true }; } catch (Exception) { }
                        if (webSocketServer != null)
                        {
                            webSocketServer.Start(socket =>
                            {
                                try
                                {
                                    //WS建立连接
                                    socket.OnOpen = () => { try { webSocketClients.Add(socket); WriteLine("[websocket]连接已建立"); } catch (Exception) { } };
                                    //WS断开连接
                                    socket.OnClose = () => { try { webSocketClients.Remove(socket); WriteLine("[websocket]连接已断开"); } catch (Exception) { } };
                                    //WS收到信息
                                    socket.OnMessage = rece => { OnClientMessage(rece); };
                                    //WS出错
                                    socket.OnError = rece => { WriteLine(rece.ToString()); };
                                }
                                catch (Exception
#if DEBUG
            err
#endif
            )
                                {
#if DEBUG
                                    WriteLineDEBUG("[ERROR]ws底层\n" + err);
#endif
                                }
                            });
                            WriteLine("WS服务器端启动成功！by gxh");
                            WriteLine(StPar.Value<string>("WebsocketAPI"));
                        }
                    }
                    catch (Exception err) { WriteLine("WS服务器启动失败!\n" + err.ToString()); }
                    #endregion
                    if (StPar.ContainsKey("ShowRebootButton"))
                    {
                        if (StPar.Value<bool>("ShowRebootButton"))
                        {
                            RebootButtonBorder.Visibility = Visibility.Visible;
                            Button rebootButton = new Button() { Content = "重启" };
                            rebootButton.Click += (sender_, e_) =>
                            {
                                p.Exited += (s, b) => { };
                                StopProcess();
                                rebootButton.Content = "正在终止...";
                                rebootButton.IsEnabled = false;
                                Task.Run(() =>
                                {
                                    Thread.Sleep(500);
                                    int state = 0;
                                    for (int i = 0; i < 200; i++)
                                    {
                                        p.OutputDataReceived += (s, b) => state = 0;
                                        //WriteLine(p.HasExited);
                                        if (p.HasExited) { state++; }
                                        p.WaitForExit(1000);
                                        try
                                        {
                                            p.StandardInput.WriteLine(StPar["ExitCMD"].ToString());
                                        }
                                        catch (Exception) { }
                                        Thread.Sleep(100);
                                        if (state > 50)
                                        {
                                            break;
                                        }
                                    }
                                    Thread.Sleep(1000);
                                    Dispatcher.Invoke(() =>
                                 {
                                     rebootButton.Content = "正在启动进程...";
                                     rebootButton.IsEnabled = false;
                                     StartProcess();
                                 });
                                    Thread.Sleep(1000);
                                    Dispatcher.Invoke(() =>
                                    {
                                        rebootButton.Content = "重启";
                                        rebootButton.IsEnabled = true;
                                    });
                                });
                            };
                            RebootButtonBorder.Child = rebootButton;
                        }
                    }

                }
                BottomDP.Height = 0;
                EchoInputPanel();
                #endregion
            }
            catch (Exception err)
            {
                WriteLine("启动失败!\n" + err.ToString());
            }

        }
        public Encoding GetEncoding(string v)
        {
            if (v.ToLower() == "default") { return Encoding.Default; }
            else
            {
                try
                { return Encoding.GetEncoding(int.Parse(v)); }
                catch (Exception)
                {
                    try
                    { return Encoding.GetEncoding(v); }
                    catch (Exception) { return Encoding.Default; }
                }
            }
        }
        #region 面板动画
        bool a;
        private void EchoInputPanel()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimationUsingKeyFrames keyFramesAnimation = new DoubleAnimationUsingKeyFrames();
            keyFramesAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(33, TimeSpan.FromSeconds(0.35)));
            Storyboard.SetTarget(keyFramesAnimation, BottomDP);
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
            Storyboard.SetTarget(keyFramesAnimation, BottomDP);
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
            if (a == false)
            {
                EchoInputPanel();
                Input.Focus();
            }
        }
        #region WebsocketAPI
        WebSocketServer webSocketServer = null;
        List<IWebSocketConnection> webSocketClients = new List<IWebSocketConnection>();
        #region 关闭页面释放资源
        public void DisposePage()
        {
            try
            {
                webSocketClients.ForEach(ws => ws.Close());
                webSocketClients.Clear();
                webSocketServer.Dispose();
            }
            catch (Exception) { }
            StopProcess();
        }
        private void StopProcess()
        {
            try
            {
                try { SendCMD(StPar["ExitCMD"].ToString()); } catch (Exception) { p.Kill(); }
                try
                {
                    if (p.MainWindowHandle != IntPtr.Zero)
                    {
                        User32API.ShowWindow(p.MainWindowHandle, User32API.SW_SHOWNA);
                    }
                }
                catch (Exception) { }
                //p.StartInfo.RedirectStandardOutput = false;
                //p.StartInfo.RedirectStandardInput = false;  

            }
            catch (Exception) { }
        }
        private void StartProcess()
        {
            if (p != null)
            {
                try
                {
                    p.Kill();
                    p.Dispose();
                }
                catch (Exception)
                { }
            }
            p = new Process();
            p.StartInfo.FileName = StPar.Value<string>("basicFilePath");
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(p.StartInfo.FileName);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            if (StPar.ContainsKey("showWindow"))
            {
                if (!StPar.Value<bool>("showWindow"))
                {
#if DEBUG
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
#else
                            p.StartInfo.CreateNoWindow = true;
#endif
                }
            }
            p.EnableRaisingEvents = true;
            var encoding = GetEncoding(StPar.Value<string>("OutPutEncoding"));
            p.StartInfo.StandardOutputEncoding = encoding;
            p.StartInfo.StandardErrorEncoding = encoding;
            WriteLine("当前编码:" + p.StartInfo.StandardOutputEncoding);
            p.Start();
            p.BeginErrorReadLine();
            p.ErrorDataReceived += (_s, _e) => WriteLine(_e.Data);
            p.BeginOutputReadLine();
            p.OutputDataReceived += (_s, _e) => OnConsoleReadLine(_e.Data);
            p.Exited += (_s, _e) =>
            {
                WriteLine("---Main Process Exited---");
            };
            try
            {
                Task.Run(() =>
                {
                    for (int i = 0; i < 2000; i++)
                    {
                        Thread.Sleep(10);
                        if (p.MainWindowHandle != IntPtr.Zero)
                        {
                            User32API.ShowWindow(p.MainWindowHandle, User32API.SW_HIDE);
                            break;
                        }
                        if (p.HasExited) break;
                    }
                    //WriteLine(p.HandleCount);
                    //WriteLine(p.MainWindowHandle);
                    User32API.ShowWindow(p.Handle, User32API.SW_MINIMIZE);
                    //User32API.ShowWindow(User32API.GetPWindowHandle(p.Id), User32API.SW_MINIMIZE);
                });
            }
            catch (Exception) { }
        }
        #endregion
        private void SendToAll(object intext)
        {
            if (intext == null) return;
            string text = intext.ToString();
            webSocketClients.ForEach(ws =>
            {
                try {/* Task.Run(() =>*/ ws.Send(text)/*)*/; }
                catch (Exception) { }
            });
        }
        private void OnConsoleReadLine(string rece)
        {
            try
            {
                WriteLine(rece);
                if (webSocketClients.Count > 0)
                {
                    switch (StPar.Value<int>("Type"))
                    {
                        case 0://BDS
                        case 2://MG
                            try
                            {
                                var match1 = Regex.Match(rece, @"{?\[\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\s(?<type>\w*?)\]\s?(?<Content>.*)");
                                string msgtype = match1.Groups["type"].Value;
                                if (!string.IsNullOrEmpty(msgtype))
                                {
                                    string content = match1.Groups["Content"].Value;
#if DEBUG
                                    WriteLineDEBUG(msgtype + ":" + content);
#endif
                                    if (msgtype == "INFO")
                                    {
                                        var match2 = Regex.Match(content, @"^Player\s(?<dis>dis)?connected:\s(?<Player>.*?),\sxuid:\s(?<xuid>\d*)");
#if DEBUG
                                        WriteLineDEBUG(match2.Groups["Player"].Value + ":" + match2.Groups["xuid"]);
#endif
                                        if (match2.Groups["Player"].Success)
                                        {
                                            if (match2.Groups["dis"].Success)
                                            {
                                                SendToAll(new JObject(){
                                    new JProperty("operate","onleft"),
                                    new JProperty( "target",match2.Groups["Player"].Value ),
                                    new JProperty( "text",match2.Groups["xuid"].Value )
                                 });
                                            }
                                            else
                                            {
                                                SendToAll(new JObject(){
                                    new JProperty("operate","onjoin"),
                                    new JProperty( "target",match2.Groups["Player"].Value ),
                                    new JProperty( "text",match2.Groups["xuid"].Value )
                                 });
                                            }
                                        }
                                    }
                                    else if (msgtype == "Chat")
                                    {
                                        var match2 = Regex.Match(content, @"^(玩家\s)?(?!(玩家\s)?(Server|服务器))(?<Player>.*?)(?<!Server|服务器|\s悄悄地对.*)\s说:\s?(?<text>.+)");
                                        if (match2.Groups["Player"].Success)
                                        {
                                            SendToAll(new JObject()
                            {
                                new JProperty("operate","onmsg"),
                                new JProperty( "target",match2.Groups["Player"].Value ),
                                new JProperty( "text",match2.Groups["text"].Value )
                             });
                                        }
                                    }
                                }
                            }


                            catch (Exception
#if DEBUG
            err
#endif
            )
                            {
#if DEBUG
                                WriteLineDEBUG("[ERROR]OnConsoleReadLine\n" + err);
#endif
                            }
                            break;
                        case 1://EZ
                            try
                            {
                                var match1 = Regex.Match(rece, @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\s\w\s\[(?<type>\w+?)\]\s\(.+?\)\s(?<Content>.*)");
                                string msgtype = match1.Groups["type"].Value;
                                if (!string.IsNullOrEmpty(msgtype))
                                {
                                    string content = match1.Groups["Content"].Value;
#if DEBUG
                                    WriteLineDEBUG(msgtype + ":" + content);
#endif
                                    if (msgtype == "SERVER")
                                    {
                                        var match2 = Regex.Match(content, @"^Player\s(?<dis>dis)?connected:\s(?<Player>.*?),\sxuid:\s(?<xuid>\d*)");
#if DEBUG
                                        WriteLineDEBUG(match2.Groups["Player"].Value + ":" + match2.Groups["xuid"]);
#endif
                                        if (match2.Groups["Player"].Success)
                                        {
                                            if (match2.Groups["dis"].Success)
                                            {
                                                SendToAll(new JObject(){
                                    new JProperty("operate","onleft"),
                                    new JProperty( "target",match2.Groups["Player"].Value ),
                                    new JProperty( "text",match2.Groups["xuid"].Value )
                                 });
                                            }
                                            else
                                            {
                                                SendToAll(new JObject(){
                                    new JProperty("operate","onjoin"),
                                    new JProperty( "target",match2.Groups["Player"].Value ),
                                    new JProperty( "text",match2.Groups["xuid"].Value )
                                 });
                                            }
                                        }
                                    }
                                    else if (msgtype == "CHAT")
                                    {
                                        var match2 = Regex.Match(content, @"\[(?<Player>.+?)\]\s(?<text>.+)");
                                        if (match2.Groups["Player"].Success)
                                        {
                                            SendToAll(new JObject()
                            {
                                new JProperty("operate","onmsg"),
                                new JProperty( "target",match2.Groups["Player"].Value ),
                                new JProperty( "text",match2.Groups["text"].Value )
                             });
                                        }
                                    }
                                }
                            }


                            catch (Exception
#if DEBUG
            err
#endif
            )
                            {
#if DEBUG
                                WriteLineDEBUG("[ERROR]OnConsoleReadLine\n" + err);
#endif
                            }
                            break;
                        default:
                            break;

                    }
                }
            }
            catch (Exception
#if DEBUG
            err
#endif
            )
            {
#if DEBUG
                WriteLineDEBUG("[ERROR]输入逻辑Input_TextChanged\n" + err);
#endif
            }
        }
        private void OnClientMessage(string rece)
        {
            try
            {
                var receive = JObject.Parse(rece);
                // {"operate":"runcmd","passwd":"CD92DDCEBFB8D3FB1913073783FAC0A1","cmd":"in_game command here"}
                if (receive.Value<string>("operate") == "runcmd")
                {
                    SendCMD(receive.Value<string>("cmd"));
                }
                //new JProperty("cmd", cmd),
                //new JProperty("msgid", Operation.RandomUUID),
                //new JProperty("passwd", "")
                //Dispatcher.Invoke(() =>
                //{    
                //}); 
            }
            catch (Exception
#if DEBUG
            err
#endif
            )
            {
#if DEBUG
                WriteLineDEBUG("[ERROR]ws接收消息\n" + err);
#endif
            }
        }
        #endregion
        #region 定时清屏
        private Timer ClearTimer = new Timer(1800000) { AutoReset = true, Enabled = false };/**/
        private int ClearText = 0;
        private void ClearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (ClearText < Cos.Text.Length)
                    {
                        Cos.Text = Cos.Text.Substring(ClearText);
                        Cos.ScrollToEnd();
                    }
                    ClearText = Cos.Text.Length;
                });
            }
            catch (Exception) { }
        }
        #endregion
    }
}
