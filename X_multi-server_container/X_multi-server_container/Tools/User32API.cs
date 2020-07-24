using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace X_multi_server_container.Tools
{
    class User32API
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
        private static extern bool ShowWindow(IntPtr hWnd, int type);

        //public TestClass()
        //{
        //    Console.Title = "MyConsoleApp";
        //    IntPtr ParenthWnd = new IntPtr(0);
        //    IntPtr et = new IntPtr(0);
        //    ParenthWnd = FindWindow(null, "MyConsoleApp");

        //    ShowWindow(ParenthWnd, 2);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化

        //    //作自己的事情
        //    Thread.Sleep(3000);

        //    Console.Write("Exit");

        //}
    }
}
