using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace X_multi_server_container.Tools
{
    public class User32API
    {
        #region ShowWindow 方法窗体状态的参数枚举
        /// <summary>
        /// 隐藏窗口并激活其他窗口
        /// </summary>
        public const int SW_HIDE = 0;

        /// <summary>
        /// 激活并显示一个窗口。如果窗口被最小化或最大化，系统将其恢复到原来的尺寸和大小。应用程序在第一次显示窗口的时候应该指定此标志
        /// </summary>
        public const int SW_SHOWNORMAL = 1;

        /// <summary>
        /// 激活窗口并将其最小化
        /// </summary>
        public const int SW_SHOWMINIMIZED = 2;
        /// <summary>
        /// 激活窗口并将其最大化
        /// </summary>
        public const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// 以窗口最近一次的大小和状态显示窗口。此值与SW_SHOWNORMAL相似，只是窗口没有被激活
        /// </summary>
        public const int SW_SHOWNOACTIVATE = 4;

        /// <summary>
        /// 在窗口原来的位置以原来的尺寸激活和显示窗口
        /// </summary>
        public const int SW_SHOW = 5;

        /// <summary>
        /// 最小化指定的窗口并且激活在Z序中的下一个顶层窗口
        /// </summary>
        public const int SW_MINIMIZE = 6;

        /// <summary>
        /// 最小化的方式显示窗口，此值与SW_SHOWMINIMIZED相似，只是窗口没有被激活
        /// </summary>
        public const int SW_SHOWMINNOACTIVE = 7;

        /// <summary>
        /// 以窗口原来的状态显示窗口。此值与SW_SHOW相似，只是窗口没有被激活
        /// </summary>
        public const int SW_SHOWNA = 8;

        /// <summary>
        /// 激活并显示窗口。如果窗口最小化或最大化，则系统将窗口恢复到原来的尺寸和位置。在恢复最小化窗口时，应用程序应该指定这个标志
        /// </summary>
        public const int SW_RESTORE = 9;

        /// <summary>
        /// 依据在STARTUPINFO结构中指定的SW_FLAG标志设定显示状态，STARTUPINFO 结构是由启动应用程序的程序传递给CreateProcess函数的
        /// </summary>
        public const int SW_SHOWDEFAULT = 10;

        /// <summary>
        /// 最小化窗口，即使拥有窗口的线程被挂起也会最小化。在从其他线程最小化窗口时才使用这个参数
        /// </summary>
        public const int SW_FORCEMINIMIZE = 11;
        #endregion

        //[DllImport("user32.dll", EntryPoint = "ShowWindow")]
        //public static extern bool ShowWindow(int hwnd, int nCmdShow);

        [DllImport("User32.dll", EntryPoint = "ShowWindow")]
        public static extern bool ShowWindow(IntPtr hWnd, int type);

        //#region 获取自身

        //private static Hashtable processWnd = null;

        //public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        //static User32API()
        //{
        //    if (processWnd == null)
        //    {
        //        processWnd = new Hashtable();
        //    }
        //}

        //[DllImport("user32.dll", EntryPoint = "EnumWindows", SetLastError = true)]
        //public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        //[DllImport("user32.dll", EntryPoint = "GetParent", SetLastError = true)]
        //public static extern IntPtr GetParent(IntPtr hWnd);

        //[DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId")]
        //public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        //[DllImport("user32.dll", EntryPoint = "IsWindow")]
        //public static extern bool IsWindow(IntPtr hWnd);

        //[DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        //public static extern void SetLastError(uint dwErrCode);

        //public static IntPtr GetCurrentWindowHandle()
        //{
        //    IntPtr ptrWnd = IntPtr.Zero;
        //    uint uiPid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID
        //    object objWnd = processWnd[uiPid];
        //     if (objWnd != null)
        //    {
        //        ptrWnd = (IntPtr)objWnd;
        //        if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd))  // 从缓存中获取句柄
        //        {
        //            return ptrWnd;
        //        }
        //        else
        //        {
        //            ptrWnd = IntPtr.Zero;
        //        }
        //    }

        //    bool bResult = EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
        //    // 枚举窗口返回 false 并且没有错误号时表明获取成功
        //    if (!bResult && Marshal.GetLastWin32Error() == 0)
        //    {
        //        objWnd = processWnd[uiPid];
        //        if (objWnd != null)
        //        {
        //            ptrWnd = (IntPtr)objWnd;
        //        }
        //    }

        //    return ptrWnd;
        //}

        //public static IntPtr GetPWindowHandle(int pid)
        //{
        //    IntPtr ptrWnd = IntPtr.Zero;
        //    uint uiPid =(uint)pid;  // 获取进程 ID
        //    object objWnd = processWnd[uiPid];
        //    if (objWnd != null)
        //    {
        //        ptrWnd = (IntPtr)objWnd;
        //        if (ptrWnd != IntPtr.Zero && IsWindow(ptrWnd))  // 从缓存中获取句柄
        //        {
        //            return ptrWnd;
        //        }
        //        else
        //        {
        //            ptrWnd = IntPtr.Zero;
        //        }
        //    }

        //    bool bResult = EnumWindows(new WNDENUMPROC(EnumWindowsProc), uiPid);
        //    // 枚举窗口返回 false 并且没有错误号时表明获取成功
        //    if (!bResult && Marshal.GetLastWin32Error() == 0)
        //    {
        //        objWnd = processWnd[uiPid];
        //        if (objWnd != null)
        //        {
        //            ptrWnd = (IntPtr)objWnd;
        //        }
        //    }

        //    return ptrWnd;
        //}

        //private static bool EnumWindowsProc(IntPtr hwnd, uint lParam)
        //{
        //    uint uiPid = 0;

        //    if (GetParent(hwnd) == IntPtr.Zero)
        //    {
        //        GetWindowThreadProcessId(hwnd, ref uiPid);
        //        if (uiPid == lParam)    // 找到进程对应的主窗口句柄
        //        {
        //            processWnd[uiPid] = hwnd;   // 把句柄缓存起来
        //            SetLastError(0);    // 设置无错误
        //            return false;   // 返回 false 以终止枚举窗口
        //        }
        //    }

        //    return true;
        //}




        //#endregion














    }

    //public class User32API1
    //{
    //    [DllImport("User32.dll", EntryPoint = "FindWindow")]
    //    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    //    [DllImport("user32.dll", EntryPoint = "FindWindowEx")]   //找子窗体   
    //    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    //    [DllImport("User32.dll", EntryPoint = "SendMessage")]   //用于发送信息给窗体   
    //    private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

    //    [DllImport("User32.dll", EntryPoint = "ShowWindow")]   //
    //    private static extern bool ShowWindow(IntPtr hWnd, int type);

    //    //public TestClass()
    //    //{
    //    //    Console.Title = "MyConsoleApp";
    //    //    IntPtr ParenthWnd = new IntPtr(0);
    //    //    IntPtr et = new IntPtr(0);
    //    //    ParenthWnd = FindWindow(null, "MyConsoleApp");

    //    //    ShowWindow(ParenthWnd, 2);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化

    //    //    //作自己的事情
    //    //    Thread.Sleep(3000);

    //    //    Console.Write("Exit");

    //    //}
    //}
}
