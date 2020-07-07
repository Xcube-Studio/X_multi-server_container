﻿using System;
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
        #region 启动参数
        public JObject StPar = new JObject{
            new JProperty("basicFilePath", "cmd"),
            new JProperty("Encoding", Encoding.Default.ToString()),
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
            switch (e.Key)
            {
                case Key.Up: UPSend(); break;
                case Key.Down: DOWNSend(); break;
                case Key.Enter: if (SendCMDButton.Content.ToString() == "发送") SendCMDButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent)); break;
                default:
                    break;
            }
        }
        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmdHistory[int.Parse(Input.Tag.ToString())] = Input.Text;
            if (Input.Text.Length == 0)
            { SendCMDButton.Content = "︾"; SendCMDButton.FontSize = 21; }
            else
            { SendCMDButton.Content = "发送"; SendCMDButton.FontSize = 14; }
        }
        List<string> cmdHistory = new List<string>() { "" };
        private void SendCMD(string cmd) => p.StandardInput.WriteLine(cmd);
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
            #region 界面/进程
            try
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
                    p.StartInfo.StandardOutputEncoding = GetEncoding(StPar.Value<string>("Encoding"));
                    WriteLine("当前编码:" + p.StartInfo.StandardOutputEncoding);
                    p.Start();
                    p.BeginErrorReadLine();
                    p.ErrorDataReceived += (_s, _e) => WriteLine(_e.Data);
                    p.BeginOutputReadLine();
                    p.OutputDataReceived += (_s, _e) => OnConsoleReadLine(_e.Data);
                }
                Board.Height = 0;
                EchoInputPanel();
                #endregion
                #region WS
                try
                {
                    try { if (webSocketServer == null) webSocketServer = new WebSocketServer(StPar.Value<string>("WebsocketAPI")) { RestartAfterListenError = true }; } catch (Exception) { }
                    if (webSocketServer != null)
                    {
                        webSocketServer.Start(socket =>
                        {
                            //WS建立连接
                            socket.OnOpen = () => { webSocketClients.Add(socket); };
                            //WS断开连接
                            socket.OnClose = () => { webSocketClients.Remove(socket); };
                            //WS收到信息
                            socket.OnMessage = rece => { OnClientMessage(rece); };
                            //WS出错
                            socket.OnError = rece => { WriteLine(rece.ToString()); };
                        });
                        WriteLine("WS服务器端启动成功！");
                        WriteLine(StPar.Value<string>("WebsocketAPI"));
                    }
                }
                catch (Exception err) { WriteLine("WS服务器启动失败!\n" + err.ToString()); }
            }
            catch (Exception err)
            { WriteLine("启动失败!\n" + err.ToString()); }
            #endregion  
        }
        private Encoding GetEncoding(string v)
        {
            switch (v)
            {
                case "System.Text.ASCIIEncoding": return Encoding.ASCII;
                case "System.Text.UTF8Encoding": return Encoding.UTF8;
                case "System.Text.UTF7Encoding": return Encoding.UTF7;
                case "System.Text.UTF32Encoding": return Encoding.UTF32;
                case "System.Text.UnicodeEncoding": return Encoding.Unicode;
                default: return Encoding.Default;
            }
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
            if (a == false)
            {
                EchoInputPanel();
                Input.Focus();
            }
        }
        #region WebsocketAPI
        WebSocketServer webSocketServer = null;
        List<IWebSocketConnection> webSocketClients = new List<IWebSocketConnection>();
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
            WriteLine(rece);
            if (webSocketClients.Count > 0)
            {
                int type = StPar.Value<int>("Type");
                if (type == 0)//BDS
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
                            var match2 = Regex.Match(content, @"^Player\s(?<dis>dis)?connected:\s(?<Player>.*?)\sxuid:\s(?<xuid>\d*)");
#if DEBUG
                            WriteLineDEBUG(match2.Groups["Player"].Value + ":" + match2.Groups["dis"]);
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
                            var match2 = Regex.Match(content, @"^(?!Server|服务器)(?<Player>.*?)(?<!Server|服务器)\s说:\s(?<text>.+)");
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
                else if (type == 1)
                {

                }
                else if (type == 2)
                {

                }
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
                WriteLineDEBUG(err);
#endif
            }
        }
        #endregion
    }
}
