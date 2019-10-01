using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.DSLR.Sony
{
    public class ExposureReadyEventArgs : EventArgs
    {
        public Array ImageArray { get; private set; }
        public ExposureReadyEventArgs(Array imageArray)
        {
            ImageArray = imageArray;
        }
    }

    public class ExposureCompletedEventArgs : EventArgs{

    }

    public class ExposureFailedEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public ExposureFailedEventArgs(Exception e)
        {
            Exception = e;
        }
    }

    public class ExposureAbortedEventArgs : EventArgs
    {

    }

    public class ExposureStoppedEventArgs : EventArgs
    {

    }

    internal class ImagingEdgeRemoteInterop
    {
        private ImageDataProcessor _imageDataProcessor = new ImageDataProcessor();

        #region pInvoke definitions
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        /// <summary>
        /// The FindWindow API
        /// </summary>
        /// <param name="lpClassName">the class name for the window to search for</param>
        /// <param name="lpWindowName">the name of the window to search for</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wparam, int lparam);

        const int WM_GETTEXT = 0x000D;
        const int WM_GETTEXTLENGTH = 0x000E;
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
        private IntPtr _folderComboboxHandle;
        private IntPtr _fileFormatButtonHandle;

        private TreeNode<IntPtr> _hWindowTree;

        private CameraModel _cameraModel;

        public bool IsConnected { get; private set; }

        private ImageFormat _imageFormat;
        private bool _autoDeleteImageFile;

        public ImagingEdgeRemoteInterop(CameraModel cameraModel, ImageFormat imageFormat, bool autoDeleteImageFile)
        {
            _cameraModel = cameraModel;
            _imageFormat = imageFormat;
            _autoDeleteImageFile = autoDeleteImageFile;
        }

        private Array ReadCameraImageArray(string rawFileName)
        {
            switch (_imageFormat)
            {
                case ImageFormat.CFA:
                    return _imageDataProcessor.ReadRaw(rawFileName);
                case ImageFormat.Debayered:
                    return _imageDataProcessor.ReadAndDebayerRaw(rawFileName);
                case ImageFormat.JPG:
                    return _imageDataProcessor.ReadJpeg(rawFileName);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FileSystemWatcherOnCreated(object sender, FileSystemEventArgs e)
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                string filePath = e.FullPath;
                //ensure file is completely saved to hard disk

                while (CanAccessFile(filePath) == false)
                {
                    //wait
                }

                Thread.Sleep(1000);//for some reason we need to wait here for file lock to be released on image file

                Array imageArray = ReadCameraImageArray(filePath);

                if (_autoDeleteImageFile)
                {
                    File.Delete(filePath);
                }

                ExposureReady?.Invoke(this, new ExposureReadyEventArgs(imageArray));
            }
        }

        private bool CanAccessFile(string filePath)
        {
            try
            {
                using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    fs.Close();
                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }
        }

        public event EventHandler<ExposureReadyEventArgs> ExposureReady;
        public event EventHandler<ExposureCompletedEventArgs> ExposureCompleted;
        public event EventHandler<ExposureFailedEventArgs> ExposureFailed;
        public event EventHandler<ExposureAbortedEventArgs> ExposureAborted;
        public event EventHandler<ExposureStoppedEventArgs> ExposureStopped;

        public void Connect()
        {
            try
            {
                var hRemoteAppWindow = (IntPtr)FindWindow(null, "Remote");

                if (hRemoteAppWindow == IntPtr.Zero)
                {
                    throw new Exception("Unable to locate Imaging Edge Remote app main window.");
                }

                try
                {
                    _hWindowTree = BuildWindowHandlesTree(hRemoteAppWindow);
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to build control hierarchy of Imaging Edge Remote app main window.",e);
                }
                
                //populate important UI elements window handlersv
                try
                {
                    _shutterButtonHandle = _hWindowTree.Children[3].Children[0].Children[0].Value;
                    if (_shutterButtonHandle == IntPtr.Zero)
                    {
                        throw new Exception("Shutter Button handle not found.");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate shutter button in Imaging Edge Remote app main window",e);
                }

                try
                {
                    _shutterSpeedLabelHandle = _hWindowTree.Children[3].Children[2].Children[7].Value;
                    if (_shutterSpeedLabelHandle == IntPtr.Zero)
                    {
                        throw new Exception("Shutter speed label handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate current shutter speed label in Imaging Edge Remote app main window",e);
                }

                try
                {
                    _isoLabelHandle = _hWindowTree.Children[3].Children[2].Children[9].Value;
                    if (_isoLabelHandle == IntPtr.Zero)
                    {
                        throw new Exception("Current ISO label handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate current ISO label in Imaging Edge Remote app main window", e);
                }

                try
                {
                    _shutterSpeedIncreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[14].Value;
                    if (_shutterSpeedIncreaseButtonHandle == IntPtr.Zero)
                    {
                        throw new Exception("Increase shutter speed button handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate increase shutter speed button in Imaging Edge Remote app main window",e);
                }

                try
                {
                    _shutterSpeedDecreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[15].Value;
                    if (_shutterSpeedDecreaseButtonHandle == IntPtr.Zero)
                    {
                        throw new Exception("Decrease shutter speed button handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate decrease shutter speed button in Imaging Edge Remote app main window",e);
                }

                try
                {
                    _isoIncreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[18].Value;
                    if (_isoIncreaseButtonHandle == IntPtr.Zero)
                    {
                        throw new Exception("Increase ISO button handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate increase ISO button in Imaging Edge Remote app main window", e);
                }

                try
                {
                    _isoDecreaseButtonHandle = _hWindowTree.Children[3].Children[2].Children[19].Value;
                    if (_isoIncreaseButtonHandle == IntPtr.Zero)
                    {
                        throw new Exception("Decrease ISO button handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate decrease ISO button in Imaging Edge Remote app main window", e);
                }

                try
                {
                    _fileFormatButtonHandle = _hWindowTree.Children[3].Children[3].Children[0].Value;
                    if (_fileFormatButtonHandle == IntPtr.Zero)
                    {
                        throw new Exception("File format button handle not found");
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Unable to locate file format button in Imaging Edge Remote app main window");
                }

                string monitorFolderPath;

                try
                {
                    _folderComboboxHandle = _hWindowTree.Children[3].Children[6].Children[9].Value;
                    if (_folderComboboxHandle == IntPtr.Zero)
                    {
                        throw new Exception("Save folder combobox handle not found");
                    }

                    monitorFolderPath = GetCurrentSaveFolder();
                    
                    if (string.IsNullOrEmpty(monitorFolderPath))
                    {
                        throw new Exception("Unable to determine path for save folder. Make sure save folder path is selected in Imaging Edge Remote app");
                    }
                }
                catch (Exception e)
                {
                    throw  new Exception("Unable to locate save folder combobox in Imaging Edge Remote app main window",e);
                }

                //create save folder if not exists
                if (Directory.Exists(monitorFolderPath) == false)
                {
                    Directory.CreateDirectory(monitorFolderPath);
                }

                //create file system watcher to monitor save folder
                _fileSystemWatcher = new FileSystemWatcher(monitorFolderPath);
                _fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
                _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
                _fileSystemWatcher.EnableRaisingEvents = false;

            }
            catch (Exception e)
            {
                throw new ASCOM.NotConnectedException("Unable to communicate with Imaging Edge Remote app. Ensure the app is running and camera is connected.");
            }

            IsConnected = true;
        }

        public void Disconnect()
        {
            _hWindowTree = null;
            _shutterButtonHandle = IntPtr.Zero;
            _shutterSpeedLabelHandle = IntPtr.Zero;
            _shutterSpeedIncreaseButtonHandle = IntPtr.Zero;
            _shutterSpeedDecreaseButtonHandle = IntPtr.Zero;
            _isoLabelHandle = IntPtr.Zero;
            _isoIncreaseButtonHandle= IntPtr.Zero;
            _isoDecreaseButtonHandle= IntPtr.Zero;

            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
                _fileSystemWatcher = null;
            }
            

            IsConnected = false;
        }

        private BackgroundWorker _exposureBackgroundWorker;

        private FileSystemWatcher _fileSystemWatcher;

        private bool exposureInProgress = false;
        private static object _lock = new object();

        private void SetISO(short iso)
        {
            string currentISOAsString = GetCurrentISO();

            bool increase = true;
            
            if (string.Compare(currentISOAsString,"AUTO",StringComparison.InvariantCultureIgnoreCase)!=0)
            {
                short currentISO = short.Parse(currentISOAsString);

                if (currentISO == iso)
                {
                    return;
                }

                if (currentISO > iso)
                {
                    increase = false;
                }

                while (string.Compare(GetCurrentISO(), iso.ToString(), StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    if (increase)
                    {
                        IncreaseISO();
                    }
                    else
                    {
                        DecreaseISO();
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        private void SyncSaveFolder()
        {
            //it is possible that user changed save folder in remote app - this method ensure that we looking for right folder
            if (_fileSystemWatcher!=null && _fileSystemWatcher.Path != GetCurrentSaveFolder())
            {
                _fileSystemWatcher.Path = GetCurrentSaveFolder();
            }
        }

        private void SetShutterSpeed(double durationSeconds, double minDurationForBulbMode)
        {
            string shutterSpeed;
            if (durationSeconds < minDurationForBulbMode)
            {
                shutterSpeed =
                    (_cameraModel.ShutterSpeeds.Where(ss => ss.DurationSeconds <= durationSeconds)
                         .OrderByDescending(ss => ss.DurationSeconds).FirstOrDefault() ??
                     _cameraModel.ShutterSpeeds.OrderBy(ss => ss.DurationSeconds).First()).Name;
            }
            else
            {
                shutterSpeed = "BULB";
            }

            bool increase = true;
            if (shutterSpeed != "BULB")
            {
                string currentShutterSpeedAsString = GetCurrentShutterSpeed();

                var currentShutterSpeed = _cameraModel.ShutterSpeeds.FirstOrDefault(ss => ss.Name == currentShutterSpeedAsString);

                if (currentShutterSpeed == null)
                {
                    increase = false;
                }
                else
                {
                    var requestedShutterSpeed = _cameraModel.ShutterSpeeds.First(ss => ss.Name == shutterSpeed);

                    increase = Array.IndexOf(_cameraModel.ShutterSpeeds, requestedShutterSpeed) > Array.IndexOf(_cameraModel.ShutterSpeeds, currentShutterSpeed);
                }
            }

            while (GetCurrentShutterSpeed() != shutterSpeed)
            {
                if (increase)
                {
                    IncreaseShutterSpeed();
                }
                else
                {
                    DecreaseShutterSpeed();
                }
                Thread.Sleep(1000);
            }
        }

        public void StartExposure(short iso, double durationSeconds, double minDurationForBulbMode)
        {

            if (IsConnected == false)
            {
                throw new ASCOM.NotConnectedException();
            }

            if (exposureInProgress)
            {
                throw new ASCOM.InvalidOperationException("Exposure already in progress");
            }

            _exposureBackgroundWorker = new BackgroundWorker(){ WorkerReportsProgress=false, WorkerSupportsCancellation = true};

            _exposureBackgroundWorker.DoWork += new DoWorkEventHandler( ((sender, args) =>
            {

                SetShutterSpeed(durationSeconds, minDurationForBulbMode);
                SetISO(iso);
                SyncSaveFolder();

                if (args.Cancel == false)
                {
                    try
                    {
                        BeginExposure();
                        if (durationSeconds >= minDurationForBulbMode)
                        {
                            Thread.Sleep((int)(durationSeconds * 1000));
                        }

                        if (args.Cancel == false)
                        {
                            EndExposure(durationSeconds >= minDurationForBulbMode); //we dont call EndExposure in case if background worker cancelled as we assume it has been already ended
                            _fileSystemWatcher.EnableRaisingEvents = true;
                        }
                    }
                    finally
                    {
                        lock (_lock)
                        {
                            exposureInProgress = false;
                        }
                    }
                    
                }
            }));

            _exposureBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(((sender, args) =>
            {
                if (args.Cancelled == false)
                {
                    ExposureCompleted?.Invoke(this, new ExposureCompletedEventArgs());
                }
            }));

            _exposureBackgroundWorker.RunWorkerAsync();
        }

        public void AbortExposure()
        {
            EndExposure(true);
            if (_exposureBackgroundWorker != null && _exposureBackgroundWorker.IsBusy)
            {
                _exposureBackgroundWorker.CancelAsync();
            }
            ExposureAborted?.Invoke(this, new ExposureAbortedEventArgs());
        }

        public void StopExposure()
        {
            EndExposure(true);
            _fileSystemWatcher.EnableRaisingEvents = true;
            if (_exposureBackgroundWorker != null && _exposureBackgroundWorker.IsBusy)
            {
                _exposureBackgroundWorker.CancelAsync();
            }
            ExposureStopped?.Invoke(this, new ExposureStoppedEventArgs());
            ExposureCompleted?.Invoke(this, new ExposureCompletedEventArgs());
        }

        private void BeginExposure()
        {
            lock (_lock)
            {
                if (exposureInProgress == false)
                {
                    exposureInProgress = true;
                    PostMessage(_shutterButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                    PostMessage(_shutterButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        private void EndExposure(bool shutter)
        {
            lock (_lock)
            {
                if (exposureInProgress == true)
                {
                    exposureInProgress = false;
                    if (shutter)
                    {
                        PostMessage(_shutterButtonHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
                        PostMessage(_shutterButtonHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }

        private string GetCurrentISO()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            return GetWindowText(_isoLabelHandle);
        }

        private void IncreaseISO()
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

        private void DecreaseISO()
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

        private void IncreaseShutterSpeed()
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

        private void DecreaseShutterSpeed()
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

        private string GetCurrentShutterSpeed()
        {
            if (IsConnected == false)
            {
                throw new InvalidOperationException("Camera not connected.");
            }

            return GetWindowText(_shutterSpeedLabelHandle);
        }

        private string GetCurrentSaveFolder()
        {
            int length = SendMessage(_folderComboboxHandle, WM_GETTEXTLENGTH, 0, 0).ToInt32();

            // If titleSize is 0, there is no title so return an empty string (or null)
            if (length == 0)
            {
               return string.Empty;
            }
 
            StringBuilder sbBuffer = new StringBuilder(length + 1);

            SendMessage(_folderComboboxHandle, WM_GETTEXT, length + 1, sbBuffer);

            return sbBuffer.ToString();
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

        private static string GetWindowText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder();
            GetWindowText(hWnd, sb, 1024);
            return sb.ToString();
        }
    }
}
