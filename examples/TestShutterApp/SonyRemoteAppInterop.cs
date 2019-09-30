using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestShutterApp
{
    public class SonyRemoteAppInterop
    {
        #region pInvoke definitions
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

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr GetClassName(IntPtr hWnd, StringBuilder text, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr parent, uint cmd);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const int WM_LBUTTONDOWN = 0x201;
        const int WM_LBUTTONUP = 0x202;




        #endregion

        const string MinISO = "AUTO";
        const string MaxISO = "25600";

        const string MinShutterSpeed = "1/8000";
        const string MaxShutterSpeed = "BULB";

        private IntPtr _shutterButtonHandle;
        private IntPtr _isoLabelHandle;
        private IntPtr _isoIncreaseButtonHandle;
        private IntPtr _isoDecreaseButtonHandle;
        private IntPtr _shutterSpeedLabelHandle;
        private IntPtr _shutterSpeedIncreaseButtonHandle;
        private IntPtr _shutterSpeedDecreaseButtonHandle;

        private TreeNode<IntPtr> _hWindowTree;

        public bool IsConnected { get;  private set; }

        public SonyRemoteAppInterop()
        {

        }

        public void Connect()
        {
            var hRemoteAppWindow = (IntPtr)FindWindow(null, "Remote");

            if (hRemoteAppWindow == IntPtr.Zero)
            {
                throw new Exception("Unable to connect to Sony Remote app. Ensure app is running and camera is connected.");
            }

            _hWindowTree = BuildWindowHandlesTree(hRemoteAppWindow);

            //populate important UI elements window handlers

            _shutterButtonHandle = _hWindowTree.Children[3].Children[0].Children[0].Value;
            _shutterSpeedLabelHandle = _hWindowTree.Children[3].Children[2].Children[7].Value;
            _isoLabelHandle = _hWindowTree.Children[3].Children[2].Children[9].Value;

            _shutterSpeedIncreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[14].Value;
            _shutterSpeedDecreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[15].Value;

            _isoIncreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[18].Value;
            _isoDecreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[19].Value;

            IsConnected = true;
        }

        public void TakeExposure(int iso, int lengthMilliseconds)
        {

            if (IsConnected == false)
            {
                throw new InvalidOperationException("Unable to take exposure when camera is not connected.");
            }

            PostMessage(_shutterButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(_shutterButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            Thread.Sleep(lengthMilliseconds);

            PostMessage(_shutterButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(_shutterButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        public string GetCurrentISO()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            return GetWindowText(_isoLabelHandle);
        }

        public void IncreaseISO()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            if (GetCurrentISO() == MaxISO)
            {
                return;
            }

            PostMessage(_isoIncreaseButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(_isoIncreaseButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            //TODO: properly wait sony remote app to update ISO
            Thread.Sleep(1000);

        }

        public void DecreaseISO()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            if (GetCurrentISO() == MinISO)
            {
                return;
            }

            PostMessage(_isoDecreaseButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(_isoDecreaseButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);

            //TODO: properly wait sony remote app to update ISO
            Thread.Sleep(1000);
        }

        public void IncreaseShutterSpeed()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            if (GetCurrentShutterSpeed() == MaxShutterSpeed)
            {
                return;
            }

            PostMessage(_shutterSpeedIncreaseButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(_shutterSpeedIncreaseButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            

            //TODO: properly wait sony remote app to update Shutter Speed
            Thread.Sleep(1000);
        }

        public void DecreaseShutterSpeed()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            if (GetCurrentShutterSpeed() == MinShutterSpeed)
            {
                return;
            }

            PostMessage(_shutterSpeedDecreaseButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            PostMessage(_shutterSpeedDecreaseButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            
            //TODO: properly wait sony remote app to update Shutter Speed
            Thread.Sleep(1000);
        }

        public string GetCurrentShutterSpeed()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            return GetWindowText(_shutterSpeedLabelHandle);
        }

        private static TreeNode<IntPtr> BuildWindowHandlesTree(IntPtr window)
        {
            TreeNode<IntPtr> handlesTree = new TreeNode<IntPtr>(window);

            GCHandle gcHandlesTree = GCHandle.Alloc(handlesTree);
            IntPtr pointerHandlesTree = GCHandle.ToIntPtr(gcHandlesTree);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindowTree);
                EnumChildWindows(window, childProc, pointerHandlesTree);
            }
            finally
            {
                gcHandlesTree.Free();
            }
            return handlesTree;

        }

        private static bool EnumWindowTree(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gcHandlesTree = GCHandle.FromIntPtr(lParam);

            if (gcHandlesTree == null || gcHandlesTree.Target == null)
            {
                return false;
            }

            TreeNode<IntPtr> handlesTree = gcHandlesTree.Target as TreeNode<IntPtr>;
            var parentHandle = GetParent(hWnd);

            if (parentHandle != IntPtr.Zero)
            {
                handlesTree.Traverse((handle) => {

                    if (handle.Value == parentHandle)
                    {
                        var child = handle.Children.FirstOrDefault(tn => tn.Value == hWnd);

                        if (child == null)
                        {
                            handle.AddChild(hWnd);
                        }
                    }
                });
            }

            return true;
        }
        
        private static string GetWindowFullName(IntPtr hWnd)
        {
            var text = new StringBuilder(1024);
            var className = new StringBuilder(1024);

            GetWindowText(hWnd, text, 1024);
            GetClassName(hWnd, className, 1024);

            string fullName = string.Format("{0:X8}:{1}", (int)hWnd, className);

            var parent = GetParent(hWnd);

            if (parent != IntPtr.Zero)
            {
                string parentName = GetWindowFullName(parent);

                return string.Format("{0} > {1}", parentName, fullName);
            }

            return fullName;
        }

        private static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder();
            GetWindowText(hWnd, sb, 1024);
            return sb.ToString();
        }
    }
}
