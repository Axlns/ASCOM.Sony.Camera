using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestShutterApp
{
    class Program
    {

        /// <summary>
        /// The FindWindow API
        /// </summary>
        /// <param name="lpClassName">the class name for the window to search for</param>
        /// <param name="lpWindowName">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        /// <summary>
        /// The SendMessage API
        /// </summary>
        /// <param name="hWnd">handle to the required window</param>
        /// <param name="msg">the system/Custom message to send</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, System.Text.StringBuilder text);

        /// <summary>
        /// The FindWindowEx API
        /// </summary>
        /// <param name="parentHandle">a handle to the parent window </param>
        /// <param name="childAfter">a handle to the child window to start search after</param>
        /// <param name="className">the class name for the window to search for</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className);

        /// <summary>
        /// The FindWindowEx API
        /// </summary>
        /// <param name="parentHandle">a handle to the parent window </param>
        /// <param name="childAfter">a handle to the child window to start search after</param>
        /// <param name="className">the class name for the window to search for</param>
        /// <param name="windowTitle">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);


        [DllImport("user32.dll")]
        static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr GetClassName(IntPtr hWnd, StringBuilder text, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr parent, uint cmd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        const int WM_LBUTTONDOWN = 0x201;
        const int WM_LBUTTONUP = 0x202;

        public struct Rect
        {
            public uint Left;
            public uint Top;
            public uint Right;
            public uint Bottom;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hWnd, out Rect lpRect);


        private static List<IntPtr> GetAllChildHandles(IntPtr window)
        {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(window, childProc, pointerChildHandlesList);
            }
            finally
            {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
            {
                return false;
            }

            List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);

            return true;
        }

       

        public IEnumerable<IntPtr> TraverseHierarchy(IntPtr parent, IntPtr child)
        {
            yield return parent;

            foreach (var hWnd in TraverseChildern(parent))
            {
                yield return hWnd;
            }

            foreach (var hWnd in TraversePeers(parent, child))
            {
                yield return hWnd;
            }
            

        }

        public IEnumerable<IntPtr> TraverseChildern(IntPtr handle)
        {
            // First traverse child windows
            const uint GW_CHILD = 0x05;
            var child = GetWindow(handle, GW_CHILD);
            if (IntPtr.Zero != child)
            {
                foreach (var hWnd in TraverseHierarchy(child, IntPtr.Zero))
                {
                    yield return hWnd;
                }
            }
        }

        public IEnumerable<IntPtr> TraversePeers(IntPtr parent, IntPtr start)
        {
            // Next traverse peers
            var peer = FindWindowEx(parent, start, "");
            if (IntPtr.Zero != peer)
            {

                foreach (var hWnd in TraverseHierarchy(parent, peer))
                {
                    yield return hWnd;
                }
                
            }

        }

        private static string GetWindowFullName(IntPtr hWnd)
        {
            var text = new StringBuilder(1024);
            var className = new StringBuilder(1024);

            GetWindowText(hWnd, text, 1024);
            GetClassName(hWnd, className, 1024);

            string fullName = string.Format("#{2}:{0}.{1}", className, text,hWnd);

            var parent = GetParent(hWnd);

            if (parent != IntPtr.Zero)
            {
                string parentName = GetWindowFullName(parent);

                return string.Format("{0} > {1}", parentName, fullName);
            }

            return fullName;
        }

        static void Main(string[] args)
        {
            const int WM_GETTEXT = 0x0D;
            const int WM_GETTEXTLENGTH = 0x0E;

            var windowHandler = (IntPtr)FindWindow(null, "Viewer");

            if (windowHandler == IntPtr.Zero)
            {
                Console.WriteLine("Viewer application not found");
            }

            var hWnd = FindWindowEx(windowHandler, IntPtr.Zero, "#32770", "");
            if (hWnd != IntPtr.Zero)
            {
                hWnd = FindWindowEx(hWnd, IntPtr.Zero, "Button", "-");

                if (hWnd != IntPtr.Zero)
                {
                    Console.WriteLine(GetWindowFullName(hWnd));
                }

                PostMessage(hWnd, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                PostMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            }

            //Console.WriteLine(GetWindowFullName(windowHandler));

            //foreach (IntPtr childHandle in GetAllChildHandles(windowHandler))
            //{


            //    Console.WriteLine(GetWindowFullName(childHandle));
            //}

            //var buttonHandle = FindWindowEx(windowHandler, IntPtr.Zero, "Button", "+");

            //Console.WriteLine(buttonHandle);
        }
    }
}
