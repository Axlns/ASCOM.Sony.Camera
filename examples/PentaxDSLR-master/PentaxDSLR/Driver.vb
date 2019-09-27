'tabs=4
' --------------------------------------------------------------------------------
'
' ASCOM Camera driver for PentaxDSLR
'
' Description:	Driver to activate shutter by RS-232. Image transfer is by folder monitoring.
'
' Implements:	ASCOM Camera interface version: 1.0
' Author:		Vincenzo Miceli <vincenzomiceli@hotmail.com>
'
' Edit Log:
'
' Date			Who	Vers	Description
' -----------	---	-----	-------------------------------------------------------
' 18-Jul-2014	 VM	0.0.2	Initial edit, from Camera template
' ---------------------------------------------------------------------------------
'Open issues: 1) Portrait vs landscape. 2) New folder + new file
'
' Your driver's ID is ASCOM.PentaxDSLR.Camera
'
' The Guid attribute sets the CLSID for ASCOM.DeviceName.Camera
' The ClassInterface/None addribute prevents an empty interface called
' _Camera from being created and used as the [default] interface
'

' This definition is used to select code that's only applicable for one device type

#Const Device = "Camera"

Imports ASCOM.Astrometry.AstroUtils ' Why do I need this?????
Imports ASCOM.DeviceInterface
Imports ASCOM.Utilities 'Why do I need this????
Imports System
Imports System.Collections
Imports System.Globalization
Imports System.Runtime.InteropServices
'For serial port control. I prefer to use this native than ASCOM
Imports System.Timers
'Used to time exposures and delays
Imports System.Drawing
Imports System.IO
Imports System.IO.Ports
Imports System.Reflection
Imports System.Threading
Imports System.Drawing.Imaging


'OLD Guid: <Guid("204a62e9-6316-48b2-9b27-0a6b0eb57fc4")> . 
Imports ASCOM.Utilities.Interfaces

<Guid("435DB288-5200-4C63-AE72-FBAA8025F795")> _
<ClassInterface(ClassInterfaceType.None)> _
Public Class Camera

    ' The Guid attribute sets the CLSID for ASCOM.PentaxDSLR.Camera
    ' The ClassInterface/None addribute prevents an empty interface called
    ' _PentaxDSLR from being created and used as the [default] interface

    ' TODO Replace the not implemented exceptions with code to implement the function or
    ' throw the appropriate ASCOM exception.
    '
    Implements ICameraV2

    '
    ' Driver ID and descriptive string that shows in the Chooser
    '
    Friend Shared DriverId As String = "ASCOM.PentaxDSLR.Camera"
    Private Const DriverDescription As String = "PentaxDSLR Camera"

    Friend Shared ComPortDefault As String = "COM5"
    Friend Shared ComPortProfileName As String = "COM Port" 'Constants used for Profile persistence
    Friend Shared ComPort As String ' Variables to hold the currrent device configuration

    Friend Shared TraceState As Boolean
    Friend Shared TraceStateProfileName As String = "Trace Level"
    Friend Shared TraceStateDefault As String = "False"

    'Sensor
    Friend Shared MElectronsPerAdu As Double
    Friend Shared ElectronsPerAduProfileName As String = "Electrons per AD unit"
    Friend Shared ElectronsPerAduDefault As Double = 0.2

    Friend Shared MFullWellCapacity As Double
    Friend Shared FullWellCapacityProfileName As String = "Full Well Capacity"
    Friend Shared FullWellCapacityDefault As Double = 30000

    Friend Shared MCcdTemperature As Double
    Friend Shared CcdTemperatureProfileName As String = "CCD Temperature"
    Friend Shared CcdTemperatureDefault As Double = 10

    Friend Shared MHeatSinkTemperature As Double
    Friend Shared HeatSinkTemperatureProfileName As String = "CCD Temperature"
    Friend Shared HeatSinkTemperatureDefault As Double = 10

    Friend Shared MSensorWidthPx As Integer
    Friend Shared SensorWidthPxProfileName As String = "Sensor width in px"
    Friend Shared SensorWidthPxDefault As Double = 4928

    Friend Shared MSensorHeightPx As Integer
    Friend Shared SensorHeightPxProfileName As String = "Sensor height in px"
    Friend Shared SensorHeightPxDefault As Double = 3264

    Friend Shared MPixelWidthUm As Double
    Friend Shared PixelWidthUmProfileName As String = "Pixel width in um"
    Friend Shared PixelWidthUmDefault As Double = 4.77

    Friend Shared MPixelHeightUm As Double
    Friend Shared PixelHeighthUmProfileName As String = "Pixel height in um"
    Friend Shared PixelHeightUmDefault As Double = 4.77

    Friend MBayerOffsetX As Short = 0
    Friend MBayerOffsetY As Short = 0

    Friend MBinX As Integer = 1 'binning in X
    Friend MBinY As Integer = 1 'binning in Y

    Friend MStartX As Integer = 0 'Origin for binned image. If binning>1 than this is in unis of binned pixels
    Friend MStartY As Integer = 0 'Origin for binned image. If binning>1 than this is in unis of binned pixels

    Friend MMaxAdu As Int32 = 65535

    Friend MSensorType As SensorType = SensorType.Color
    Friend m_interfaceVersion As Short = 2 'Camera v2

    ' Exposure
    Friend MCanAbortExposure As Boolean = True
    Friend MCanStopExposure As Boolean = True  '*************** to add to profile
    Friend MExposureMax As Double = 3600 '1h
    Friend MExposureMin As Double = 0.001 '1/1000th of a second
    Friend MExposureResolution As Double = 0.1 'Good enough for astronomy
    Friend m_SensorName As String = ""
    Private _mLastExposureDuration As Double
    Private _mLastExposureStartTime As String
    Private _exposureStartTime As DateTime
    Private _exposureDuration As Double
    Private _mImageReady As Boolean = False

    'File handling
    Private ReadOnly _imageData As Single(,,) ' room for a 3 plane colour image




    Friend MCameraState As CameraStates = CameraStates.cameraIdle

    Private _connectedState As Boolean ' Private variable to hold the connected state
    Private _utilities As Util ' Private variable to hold an ASCOM Utilities object
    Private _astroUtilities As AstroUtils ' Private variable to hold an AstroUtils object to provide the Range method
    Private _tl As TraceLogger ' Private variable to hold the trace logger object (creates a diagnostic log file with information that you specify)

    Private ReadOnly _serialPort1 As SerialPort 'Defines the Serial Port
    Shared _exposureTimer As Timers.Timer 'Exposure Timer
    Shared _exposureTimerMlu As Timers.Timer 'MLU Timer

    'Folder wahtching
    Public Watchfolder As FileSystemWatcher 'Folder monitoring declaration

    Friend Shared FolderToBeMonitored As String ' Variables to hold the currrent device configuration
    Friend Shared FolderToBeMonitoredProfileName As String = "Folder to be monitored" ' Variable used for profile persistence
    Friend Shared FolderToBeMonitoredDefault As String = "c:\temp"

    Friend Shared MNewFile As String
    Friend Shared MNewFileFilter As String = "*.jpg" 'Only allow jpg new files to trigger a new file flag

    Friend Shared MExposureType As String
    Friend Shared MExposureTypeProfileName As String = "Exposure Type"
    Friend Shared MExposureTypeDefault As String = "M" 'For MLU
    Friend Shared MExposureDelayBulb As Integer
    Friend Shared MExposureDelayMlu As Integer



    '
    ' Constructor - Must be public for COM registration!
    '
    Public Sub New()

        'Private _imagedata(,,) As Single
        '_imageData = imageData
        ReadProfile() ' Read device configuration from the ASCOM Profile store
        _tl = New TraceLogger("", "PentaxDSLR")
        _tl.Enabled = TraceState
        _tl.LogMessage("Camera", "Starting initialisation")

        _connectedState = False ' Initialise connected to false
        _utilities = New Util() ' Initialise util object
        _astroUtilities = New AstroUtils 'Initialise new astro utiliites object

        Watchfolder = New FileSystemWatcher() 'creates the watchfolder
        Watchfolder.NotifyFilter = NotifyFilters.DirectoryName 'flags new folders
        Watchfolder.NotifyFilter = Watchfolder.NotifyFilter Or NotifyFilters.FileName 'flags new files

        AddHandler Watchfolder.Created, AddressOf Logchange 'Adds handler


        'Creates the port
        _serialPort1 = New SerialPort()

        With _serialPort1

            .ParityReplace = &H3B                    ' replace ";" when parity error occurs 
            .PortName = ComPort
            .BaudRate = "9600"
            .Parity = Parity.None
            .DataBits = 8
            .StopBits = StopBits.One
            .Handshake = Handshake.None
            .RtsEnable = False
            .ReceivedBytesThreshold = 1             'threshold: one byte in buffer > event is fired 
            .NewLine = vbCr         ' CR must be the last char in frame. This terminates the SerialPort.readLine 
            .ReadTimeout = 10000

        End With

        'TODO: Implement your additional construction here

        _tl.LogMessage("Camera", "Completed initialisation")
    End Sub

    '
    ' PUBLIC COM INTERFACE ICameraV2 IMPLEMENTATION
    '

#Region "Common properties and methods"
    ''' <summary>
    ''' Displays the Setup Dialog form.
    ''' If the user clicks the OK button to dismiss the form, then
    ''' the new settings are saved, otherwise the old values are reloaded.
    ''' THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
    ''' </summary>
    Public Sub SetupDialog() Implements ICameraV2.SetupDialog
        ' consider only showing the setup dialog if not connected
        ' or call a different dialog if connected
        If IsConnected Then
            MessageBox.Show("Already connected, just press OK")
        End If

        Using f As SetupDialogForm = New SetupDialogForm()
            Dim result As DialogResult = f.ShowDialog()
            If result = DialogResult.OK Then
                WriteProfile() ' Persist device configuration values to the ASCOM Profile store
            End If
        End Using
    End Sub

    Public ReadOnly Property SupportedActions() As ArrayList Implements ICameraV2.SupportedActions
        Get
            _tl.LogMessage("SupportedActions Get", "Returning empty arraylist")
            Return New ArrayList()
        End Get
    End Property

    Public Function Action(ByVal actionName As String, ByVal actionParameters As String) As String Implements ICameraV2.Action
        Throw New ActionNotImplementedException("Action " & actionName & " is not supported by this driver")
    End Function

    Public Sub CommandBlind(ByVal Command As String, Optional ByVal raw As Boolean = False) Implements ICameraV2.CommandBlind
        CheckConnected("CommandBlind")
        ' Call CommandString and return as soon as it finishes
        CommandString(Command, raw)
        ' or
        Throw New MethodNotImplementedException("CommandBlind")
    End Sub

    Public Function CommandBool(ByVal Command As String, Optional ByVal raw As Boolean = False) As Boolean _
        Implements ICameraV2.CommandBool
        CheckConnected("CommandBool")
        CommandString(Command, raw)
        ' TODO decode the return string and return true or false
        ' or
        Throw New MethodNotImplementedException("CommandBool")
    End Function

    Public Function CommandString(ByVal Command As String, Optional ByVal raw As Boolean = False) As String _
        Implements ICameraV2.CommandString
        CheckConnected("CommandString")
        ' it's a good idea to put all the low level communication with the device here,
        ' then all communication calls this function
        ' you need something to ensure that only one command is in progress at a time
        Throw New MethodNotImplementedException("CommandString")
    End Function

    Public Property Connected() As Boolean Implements ICameraV2.Connected
        Get
            _tl.LogMessage("Connected Get", IsConnected.ToString())
            Return IsConnected
        End Get
        Set(value As Boolean)
            _tl.LogMessage("Connected Set", value.ToString())
            If value = IsConnected Then
                Return
            End If

            If value Then
                _connectedState = True
                _tl.LogMessage("Connected Set", "Connecting to port " + ComPort)
                MCameraState = CameraStates.cameraIdle


            Else
                _connectedState = False
                _tl.LogMessage("Connected Set", "Disconnecting from port " + ComPort)

                'Disconnects the serial port
                ' SerPort.Connected = False 'Disconnect and clean up
                ' SerPort.Dispose()
                ' SerPort = Nothing



                ' TODO disconnect from the device
            End If
        End Set
    End Property

    Public ReadOnly Property Description As String Implements ICameraV2.Description
        Get
            ' this pattern seems to be needed to allow a public property to return a private field
            Dim d As String = DriverDescription
            _tl.LogMessage("Description Get", d)
            Return d
        End Get
    End Property

    Public ReadOnly Property DriverInfo As String Implements ICameraV2.DriverInfo
        Get
            Dim mVersion As Version = Assembly.GetExecutingAssembly().GetName().Version
            ' TODO customise this driver description
            Dim sDriverInfo As String = "Pentax DSLR ASCOM drver. Version: 0.1 " + mVersion.Major.ToString() + "." + mVersion.Minor.ToString()
            _tl.LogMessage("DriverInfo Get", sDriverInfo)
            Return sDriverInfo
        End Get
    End Property

    Public ReadOnly Property DriverVersion() As String Implements ICameraV2.DriverVersion
        Get
            ' Get our own assembly and report its version number
            _tl.LogMessage("DriverVersion Get", Assembly.GetExecutingAssembly.GetName.Version.ToString(2))
            Return Assembly.GetExecutingAssembly.GetName.Version.ToString(2)
        End Get
    End Property

    Public ReadOnly Property InterfaceVersion() As Short Implements ICameraV2.InterfaceVersion
        Get
            _tl.LogMessage("InterfaceVersion Get", "2")
            Return 2
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ICameraV2.Name
        Get
            Const sName As String = "Short driver name - please customise"
            _tl.LogMessage("Name Get", sName)
            Return sName
        End Get
    End Property

    Public Sub Dispose() Implements ICameraV2.Dispose
        ' Clean up the tracelogger and util objects
        _tl.Enabled = False
        _tl.Dispose()
        _tl = Nothing
        _utilities.Dispose()
        _utilities = Nothing
        _astroUtilities.Dispose()
        _astroUtilities = Nothing
        Watchfolder = Nothing
        _exposureTimer = Nothing
        _exposureTimerMlu = Nothing
        MCameraState = Nothing

    End Sub

#End Region

#Region "ICamera Implementation"

    ' Private Const ccdWidth As Integer = m_SensorWidthPx ' Constants to define the ccd pixel dimenstions
    'Private Const ccdHeight As Integer = m_SensorHeightPx
    'Private Const pixelSizeW As Double = 4.5 ' Constant for the pixel physical dimension
    'Private Const pixelSizeH As Double = 4.5 ' Constant for the pixel physical dimension


    Private _cameraNumX As Integer = MSensorWidthPx ' Initialise variables to hold values required for functionality tested by Conform
    Private _cameraNumY As Integer = MSensorHeightPx
    Private _cameraStartX As Integer = 0
    Private _cameraStartY As Integer = 0
    Private _exposureStart As DateTime = DateTime.MinValue
    Private _cameraLastExposureDuration As Double = 0.0
    ' Private cameraImageReady As Boolean = False 'replaced by m_imageready
    Private _cameraImageArray As Integer(,,)
    Private _cameraImageArrayVariant As Object(,,)



    Public Sub AbortExposure() Implements ICameraV2.AbortExposure
        If Not Connected Then
            Throw New NotConnectedException("Can't abort exposure when not connected")
        End If
        If Not MCanAbortExposure Then
            Throw New MethodNotImplementedException("AbortExposure")
        End If
        _tl.LogMessage("Abort Exposure", "Abort Exposure")
        Select Case MCameraState
            Case CameraStates.cameraWaiting, CameraStates.cameraExposing, CameraStates.cameraReading, CameraStates.cameraDownload
                ' these are all possible exposure states so we can abort the exposure

                _exposureTimer.Enabled = False
                If _serialPort1.IsOpen = True Then
                    _serialPort1.RtsEnable = False
                    _serialPort1.Close()
                End If
                'Folder monitoring is not required for abort exposure
                Watchfolder.EnableRaisingEvents = False 'Start monitoring

                MCameraState = CameraStates.cameraIdle
                _mImageReady = False
                Exit Select
            Case CameraStates.cameraIdle
                Exit Select
            Case CameraStates.cameraError
                Throw New InvalidOperationException("AbortExposure not possible because camera is in an error state")
        End Select

    End Sub

    Public ReadOnly Property BayerOffsetX() As Short Implements ICameraV2.BayerOffsetX
        Get
            If m_interfaceVersion = 1 Then
                Throw New System.NotSupportedException("BayerOffsetX (not supported for Interface V1)")
            End If
            If Not Me.Connected Then
                Throw New NotConnectedException("Can't read BayerOffsetX when not connected")
            End If
            If Me.MSensorType = DeviceInterface.SensorType.Monochrome Then
                Throw New PropertyNotImplementedException("BayerOffsetX is not available with a monochrome camera", False)
            End If
            Return Me.MBayerOffsetX
        End Get
    End Property

    Public ReadOnly Property BayerOffsetY() As Short Implements ICameraV2.BayerOffsetY
        Get
            If m_interfaceVersion = 1 Then
                Throw New System.NotSupportedException("BayerOffsetX (not supported for Interface V1)")
            End If
            If Not Me.Connected Then
                Throw New NotConnectedException("Can't read BayerOffsetX when not connected")
            End If
            If Me.MSensorType = DeviceInterface.SensorType.Monochrome Then
                Throw New PropertyNotImplementedException("BayerOffsetX is not available with a monochrome camera", False)
            End If
            Return Me.MBayerOffsetY
        End Get
    End Property

    Public Property BinX() As Short Implements ICameraV2.BinX
        Get
            _tl.LogMessage("BinX Get", "1")
            Return 1
        End Get
        Set(value As Short)
            _tl.LogMessage("BinX Set", value.ToString())
            If (Not (value = 1)) Then
                _tl.LogMessage("BinX Set", "Value out of range, throwing InvalidValueException")
                Throw New InvalidValueException("BinX", value.ToString(), "1") ' No binning on this DSLR driver. Binning would be done after image download so no much point in it.
            End If
        End Set
    End Property

    Public Property BinY() As Short Implements ICameraV2.BinY
        Get
            _tl.LogMessage("BinY Get", "1")
            Return 1
        End Get
        Set(value As Short)
            _tl.LogMessage("BinY Set", value.ToString())
            If (Not (value = 1)) Then
                _tl.LogMessage("BinX Set", "Value out of range, throwing InvalidValueException")
                Throw New InvalidValueException("BinY", value.ToString(), "1") ' No binning on this DSLR driver. Binning would be done after image download so no much point in it.
            End If
        End Set
    End Property

    Public ReadOnly Property CCDTemperature() As Double Implements ICameraV2.CCDTemperature
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read CCD Temperature when not connected")
            End If
            Return MCcdTemperature
            'Can I read it from the EXIF data?
        End Get
    End Property

    Public ReadOnly Property CameraState() As CameraStates Implements ICameraV2.CameraState
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read the camera state when not connected")
            End If
            _tl.LogMessage("CameraState Get", CameraStates.cameraIdle.ToString())
            'Threading.Thread.Sleep(1000)
            Return MCameraState
        End Get
    End Property

    Public ReadOnly Property CameraXSize() As Integer Implements ICameraV2.CameraXSize
        Get
            _tl.LogMessage("CameraXSize Get", MSensorWidthPx.ToString())
            Return MSensorWidthPx
        End Get
    End Property

    Public ReadOnly Property CameraYSize() As Integer Implements ICameraV2.CameraYSize
        Get
            _tl.LogMessage("CameraYSize Get", MSensorHeightPx.ToString())
            Return MSensorHeightPx
        End Get
    End Property

    Public ReadOnly Property CanAbortExposure() As Boolean Implements ICameraV2.CanAbortExposure
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read CanAbortExposure when not connected")
            End If
            Return MCanAbortExposure
        End Get
    End Property

    Public ReadOnly Property CanAsymmetricBin() As Boolean Implements ICameraV2.CanAsymmetricBin
        Get
            _tl.LogMessage("CanAsymmetricBin Get", False.ToString())
            Return False
        End Get
    End Property

    Public ReadOnly Property CanFastReadout() As Boolean Implements ICameraV2.CanFastReadout
        Get
            _tl.LogMessage("CanFastReadout Get", False.ToString())
            Return False
        End Get
    End Property

    Public ReadOnly Property CanGetCoolerPower() As Boolean Implements ICameraV2.CanGetCoolerPower
        Get
            _tl.LogMessage("CanGetCoolerPower Get", False.ToString())
            Return False
        End Get
    End Property

    Public ReadOnly Property CanPulseGuide() As Boolean Implements ICameraV2.CanPulseGuide
        Get
            _tl.LogMessage("CanPulseGuide Get", False.ToString())
            Return False
        End Get
    End Property

    Public ReadOnly Property CanSetCCDTemperature() As Boolean Implements ICameraV2.CanSetCCDTemperature
        Get
            _tl.LogMessage("CanSetCCDTemperature Get", False.ToString())
            Return False
        End Get
    End Property

    Public ReadOnly Property CanStopExposure() As Boolean Implements ICameraV2.CanStopExposure
        Get
            If Not Connected Then
                _tl.LogMessage("CanStopExposure Get", False.ToString())
                Throw New NotConnectedException("Can't read CanStopExposure when not connected")
            End If
            Return MCanStopExposure
        End Get
    End Property

    Public Property CoolerOn() As Boolean Implements ICameraV2.CoolerOn
        Get
            _tl.LogMessage("CoolerOn Get", "Not implemented")
            Throw New PropertyNotImplementedException("CoolerOn", False)
        End Get
        Set(value As Boolean)
            _tl.LogMessage("CoolerOn Set", "Not implemented")
            Throw New PropertyNotImplementedException("CoolerOn", True)
        End Set
    End Property

    Public ReadOnly Property CoolerPower() As Double Implements ICameraV2.CoolerPower
        Get
            _tl.LogMessage("AbortExposure Get", "Not implemented")
            Throw New PropertyNotImplementedException("CoolerPower", False)
        End Get
    End Property

    Public ReadOnly Property ElectronsPerADU() As Double Implements ICameraV2.ElectronsPerADU
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read ElectronsPerADU when not connected")
            End If
            Return MElectronsPerAdu
            'TL.LogMessage("ElectronsPerADU Get", "Not implemented")
            'Throw New ASCOM.PropertyNotImplementedException("ElectronsPerADU", False)
        End Get
    End Property


    Public ReadOnly Property ExposureMax() As Double Implements ICameraV2.ExposureMax
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read Exposure Max when not connected")
            End If
            Return MExposureMax
        End Get

    End Property

    Public ReadOnly Property ExposureMin() As Double Implements ICameraV2.ExposureMin
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read Exposure Min when not connected")
            End If
            Return MExposureMin
        End Get
    End Property

    Public ReadOnly Property ExposureResolution() As Double Implements ICameraV2.ExposureResolution
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read Exposure Resolution when not connected")
            End If
            Return MExposureResolution
        End Get
    End Property

    Public Property FastReadout() As Boolean Implements ICameraV2.FastReadout
        Get
            _tl.LogMessage("FastReadout Get", "Not implemented")
            Throw New PropertyNotImplementedException("FastReadout", False)
        End Get
        Set(value As Boolean)
            _tl.LogMessage("FastReadout Set", "Not implemented")
            Throw New PropertyNotImplementedException("FastReadout", True)
        End Set
    End Property

    Public ReadOnly Property FullWellCapacity() As Double Implements ICameraV2.FullWellCapacity
        Get
            If Not Me.Connected Then
                Throw New NotConnectedException("Can't read FullWellCapacity when not connected")
            End If
            Return MFullWellCapacity

        End Get
    End Property

    Public Property Gain() As Short Implements ICameraV2.Gain
        Get
            _tl.LogMessage("Gain Get", "Not implemented")
            Throw New PropertyNotImplementedException("Gain", False)
        End Get
        Set(value As Short)
            _tl.LogMessage("Gain Set", "Not implemented")
            Throw New PropertyNotImplementedException("Gain", True)
        End Set
    End Property

    Public ReadOnly Property GainMax() As Short Implements ICameraV2.GainMax
        Get
            _tl.LogMessage("GainMax Get", "Not implemented")
            Throw New PropertyNotImplementedException("GainMax", False)
        End Get
    End Property

    Public ReadOnly Property GainMin() As Short Implements ICameraV2.GainMin
        Get
            _tl.LogMessage("GainMin Get", "Not implemented")
            Throw New PropertyNotImplementedException("GainMin", False)
        End Get
    End Property

    Public ReadOnly Property Gains() As ArrayList Implements ICameraV2.Gains
        Get
            _tl.LogMessage("Gains Get", "Not implemented")
            Throw New PropertyNotImplementedException("Gains", False)
        End Get
    End Property

    Public ReadOnly Property ICameraV2_PercentCompleted() As Short Implements ICameraV2.PercentCompleted
        Get
            If m_interfaceVersion = 1 Then
                Throw New System.NotSupportedException("PercentCompleted (not supported for Interface V1)")
            End If
            If Not Me.Connected Then
                Throw New NotConnectedException("Can't get PercentCompleted when not connected")
            End If
            Select Case Me.MCameraState
                Case CameraStates.cameraWaiting, CameraStates.cameraExposing, CameraStates.cameraReading, CameraStates.cameraDownload
                    Return CShort(((DateTime.Now - _exposureStartTime).TotalSeconds / _exposureDuration) * 100)
                Case CameraStates.cameraIdle
                    Return CShort(If(_mImageReady, 100, 0))
                Case Else
                    Throw New ASCOM.InvalidOperationException("get PercentCompleted is not valid if the camera is not active")
            End Select
        End Get
    End Property


    Public ReadOnly Property HasShutter() As Boolean Implements ICameraV2.HasShutter
        Get
            _tl.LogMessage("HasShutter Get", False.ToString())
            Return False
        End Get
    End Property

    Public ReadOnly Property HeatSinkTemperature() As Double Implements ICameraV2.HeatSinkTemperature
        Get
            If Not Connected Then
                Throw New NotConnectedException("Can't read Heatsink temperature when not connected")
            End If
            Return MHeatSinkTemperature

        End Get
    End Property


    'This is what gets returned to the host application.
    Public ReadOnly Property ImageArray() As Object Implements ICameraV2.ImageArray
        Get
            If (Not _mImageReady) Then
                _tl.LogMessage("ImageArray Get", "Throwing InvalidOperationException because of a call to ImageArray before the first image has been taken!")
                Throw New InvalidOperationException("Call to ImageArray before the first image has been taken!")
            End If

            Return _cameraImageArray
        End Get
    End Property



    Public ReadOnly Property ImageArrayVariant() As Object Implements ICameraV2.ImageArrayVariant 'Scrpts deal with variant arrays so this is needed for scripts.
        Get
            If (Not _mImageReady) Then
                _tl.LogMessage("ImageArrayVariant Get", "Throwing InvalidOperationException because of a call to ImageArrayVariant before the first image has been taken!")
                Throw New InvalidOperationException("Call to ImageArrayVariant before the first image has been taken!")
            End If

            ReDim _cameraImageArrayVariant(_cameraNumX - 1, _cameraNumY - 1, 3)
            'For i As Integer = 0 To _cameraImageArray.GetLength(1) - 1
            'For j As Integer = 0 To _cameraImageArray.GetLength(0) - 1
            'For k As Integer = 0 To 2
            '_cameraImageArrayVariant(j, i, k) = _cameraImageArray(j, i, k)
            'Next
            'Next
            'Next
            'Convert the int array into a Variant array
            _cameraImageArrayVariant = _utilities.ArrayToVariantArray(_cameraImageArray)

            Return _cameraImageArrayVariant
        End Get
    End Property

    Public ReadOnly Property ImageReady() As Boolean Implements ICameraV2.ImageReady
        Get
            _tl.LogMessage("ImageReady Get", _mImageReady.ToString())
            Return _mImageReady
        End Get
    End Property

    Public ReadOnly Property IsPulseGuiding() As Boolean Implements ICameraV2.IsPulseGuiding
        Get
            _tl.LogMessage("IsPulseGuiding Get", "Not implemented")
            Throw New PropertyNotImplementedException("IsPulseGuiding", False)
        End Get
    End Property

    Public ReadOnly Property LastExposureDuration() As Double Implements ICameraV2.LastExposureDuration
        Get
            If (Not _mImageReady) Then
                _tl.LogMessage("LastExposureDuration Get", "Throwing InvalidOperationException because of a call to LastExposureDuration before the first image has been taken!")
                Throw New InvalidOperationException("Call to LastExposureDuration before the first image has been taken!")
            End If
            _tl.LogMessage("LastExposureDuration Get", _cameraLastExposureDuration.ToString())
            Return _mLastExposureDuration
        End Get
    End Property

    Public ReadOnly Property LastExposureStartTime() As String Implements ICameraV2.LastExposureStartTime
        Get
            If (Not _mImageReady) Then
                _tl.LogMessage("LastExposureStartTime Get", "Throwing InvalidOperationException because of a call to LastExposureStartTime before the first image has been taken!")
                Throw New InvalidOperationException("Call to LastExposureStartTime before the first image has been taken!")
            End If
            Dim exposureStartString As String = _exposureStart.ToString("yyyy-MM-ddTHH:mm:ss")
            _tl.LogMessage("LastExposureStartTime Get", exposureStartString.ToString())
            Return exposureStartString
        End Get
    End Property

    Public ReadOnly Property MaxADU() As Integer Implements ICameraV2.MaxADU
        Get
            _tl.LogMessage("MaxADU Get", "20000")
            Return 20000
        End Get
    End Property

    Public ReadOnly Property MaxBinX() As Short Implements ICameraV2.MaxBinX
        Get
            _tl.LogMessage("MaxBinX Get", "1")
            Return 1
        End Get
    End Property

    Public ReadOnly Property MaxBinY() As Short Implements ICameraV2.MaxBinY
        Get
            _tl.LogMessage("MaxBinY Get", "1")
            Return 1
        End Get
    End Property

    Public Property NumX() As Integer Implements ICameraV2.NumX
        Get
            _tl.LogMessage("NumX Get", _cameraNumX.ToString())
            Return _cameraNumX
        End Get
        Set(value As Integer)
            _cameraNumX = value
            _tl.LogMessage("NumX set", value.ToString())
        End Set
    End Property

    Public Property NumY() As Integer Implements ICameraV2.NumY
        Get
            _tl.LogMessage("NumY Get", _cameraNumY.ToString())
            Return _cameraNumY
        End Get
        Set(value As Integer)
            _cameraNumY = value
            _tl.LogMessage("NumY set", value.ToString())
        End Set
    End Property





    Public ReadOnly Property PixelSizeX() As Double Implements ICameraV2.PixelSizeX
        Get
            _tl.LogMessage("PixelSizeX Get", MPixelWidthUm.ToString())
            Return MPixelWidthUm
        End Get
    End Property

    Public ReadOnly Property PixelSizeY() As Double Implements ICameraV2.PixelSizeY
        Get
            _tl.LogMessage("PixelSizeY Get", MPixelHeightUm.ToString())
            Return MPixelHeightUm
        End Get
    End Property

    Public Sub PulseGuide(direction As GuideDirections, duration As Integer) Implements ICameraV2.PulseGuide
        _tl.LogMessage("PulseGuide", "Not implemented - " & direction.ToString)
        Throw New MethodNotImplementedException("Direction")
    End Sub

    Public Property ReadoutMode() As Short Implements ICameraV2.ReadoutMode
        Get
            _tl.LogMessage("ReadoutMode Get", "Not implemented")
            Throw New PropertyNotImplementedException("ReadoutMode", False)
        End Get
        Set(value As Short)
            _tl.LogMessage("ReadoutMode Set", "Not implemented")
            Throw New PropertyNotImplementedException("ReadoutMode", True)
        End Set
    End Property

    Public ReadOnly Property ReadoutModes() As ArrayList Implements ICameraV2.ReadoutModes
        Get
            _tl.LogMessage("ReadoutModes Get", "Not implemented")
            Throw New PropertyNotImplementedException("ReadoutModes", False)
        End Get
    End Property

    Public ReadOnly Property SensorName() As String Implements ICameraV2.SensorName
        Get
            If Not Me.Connected Then
                Throw New NotConnectedException("Can't get SensorName when not connected")
            End If
            Return m_SensorName
        End Get
    End Property

    Public ReadOnly Property SensorType() As SensorType Implements ICameraV2.SensorType
        Get
            If Not Me.Connected Then
                Throw New NotConnectedException("Can't read the Sensor type when not connected")
            End If

            Return MSensorType
        End Get
    End Property

    Public Property SetCCDTemperature() As Double Implements ICameraV2.SetCCDTemperature
        Get
            _tl.LogMessage("SetCCDTemperature Get", "Not implemented")
            Throw New PropertyNotImplementedException("SetCCDTemperature", False)
        End Get
        Set(value As Double)
            _tl.LogMessage("SetCCDTemperature Set", "Not implemented")
            Throw New PropertyNotImplementedException("SetCCDTemperature", True)
        End Set
    End Property

    Public Sub StartExposure(duration As Double, light As Boolean) Implements ICameraV2.StartExposure
        If (duration < 0.0) Then Throw New InvalidValueException("StartExposure", duration.ToString(), "0.0 upwards")
        If (_cameraNumX > MSensorWidthPx) Then Throw New InvalidValueException("StartExposure", _cameraNumX.ToString(), MSensorWidthPx.ToString())
        If (_cameraNumY > MSensorHeightPx) Then Throw New InvalidValueException("StartExposure", _cameraNumY.ToString(), MSensorHeightPx.ToString())
        If (_cameraStartX > MSensorWidthPx) Then Throw New InvalidValueException("StartExposure", _cameraStartX.ToString(), MSensorWidthPx.ToString())
        If (_cameraStartY > MSensorHeightPx) Then Throw New InvalidValueException("StartExposure", _cameraStartY.ToString(), MSensorHeightPx.ToString())

        _mImageReady = False

        _cameraImageArray = New Integer(MSensorWidthPx - 1, MSensorHeightPx - 1, 2) {} 'Size the array that will hold the image

        If _serialPort1.IsOpen = True Then
            _serialPort1.Close()
        End If
        'exposureTimerMLU.Enabled = False 'make sure the timers are off (Probably not needed?)
        'exposureTimer.Enabled = False

        _serialPort1.Open() 'Open the port

        _cameraLastExposureDuration = duration

        _tl.LogMessage("StartExposure", duration.ToString() + " " + light.ToString())
        _exposureStart = DateTime.Now
        _mLastExposureStartTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture)

        '*************** Timing the exposure ****************

        If _exposureTimer Is Nothing Then
            _exposureTimer = New Timers.Timer()
            AddHandler _exposureTimer.Elapsed, AddressOf exposureTimer_Elapsed
        End If

        'Take into account the exposure type, Bulb or MLU

        Select Case MExposureType
            Case "B"
                'I need to increase the actual exposure time by the number of seconds that the Bulb MLU has
                _exposureTimer.Interval = CInt((duration + MExposureDelayBulb) * 1000)
                MCameraState = CameraStates.cameraExposing
                _serialPort1.RtsEnable = True 'Turn on RTSEnable to trigger the shutter
                _exposureStartTime = DateTime.Now
                _exposureTimer.Enabled = True
            Case Else
                'For the MLU type exposure I not only need to increase the exposure time by the estimated 
                'delay but also trigger a preliminary shutter release to bring the refelx mirror up
                'The extra shutter action will be controlled
                If _exposureTimerMlu Is Nothing Then
                    _exposureTimerMlu = New Timers.Timer()
                    AddHandler _exposureTimerMlu.Elapsed, AddressOf exposureTimerMLU_Elapsed
                End If

                _exposureTimerMlu.Interval = CInt((MExposureDelayMlu - 0.5) * 1000) 'I use a 0.5 sec trigger for the MLU

                MCameraState = CameraStates.cameraExposing
                _serialPort1.RtsEnable = True 'Turn on RTSEnable to trigger the shutter
                Thread.Sleep(500) 'Half a second MLU trigger
                _serialPort1.RtsEnable = False 'No more need for trigger button pressed. Mirror up. Next signal will open the shutter.
                _exposureStartTime = DateTime.Now
                _exposureTimerMlu.Enabled = True


        End Select


    End Sub

    Private Sub exposureTimerMLU_Elapsed(sender As Object, e As ElapsedEventArgs) 'This is linked to the MLU timer
        'This subroutine will execute at the end of the MLU timer tick. Now the real exposure can be triggered:
        'cameraLastExposureDuration will give the exposure time.
        _exposureTimerMlu.Enabled = False 'stop the MLU clock
        _exposureTimerMlu = Nothing 'kills the timer. The driver didn't work without this line after a camera disconnect-reconnect. It drove me crazy. 
        _exposureTimer.Interval = CInt(_cameraLastExposureDuration * 1000)
        MCameraState = CameraStates.cameraExposing 'No need, it should be like that already
        _serialPort1.RtsEnable = True 'Turn on RTSEnable to trigger the shutter
        _exposureStartTime = DateTime.Now
        _exposureTimer.Enabled = True ' Now the regular timer will check the time
    End Sub


    Private Sub exposureTimer_Elapsed(sender As Object, e As ElapsedEventArgs) 'This is used by the regular Bulb expsoure
        _exposureTimer.Enabled = False
        _serialPort1.RtsEnable = False 'Close the Shutter
        _serialPort1.Close()
        _mLastExposureDuration = (DateTime.Now - _exposureStartTime).TotalSeconds
        _exposureTimer = Nothing 'kills the timer. The driver didn't work without this line after a camera disconnect-reconnect. It drove me crazy. 

        'Folder monitoring will grab the newest file
        MCameraState = CameraStates.cameraDownload
        Watchfolder.Path = FolderToBeMonitored
        Watchfolder.Filter = MNewFileFilter
        Watchfolder.EnableRaisingEvents = True 'Start monitoring


    End Sub

    Private Sub Logchange(ByVal source As Object, ByVal e As FileSystemEventArgs)

        'Only flag created files (how about new folder with new file inside?
        If e.ChangeType = WatcherChangeTypes.Created Then
            _tl.LogMessage("Found file", e.FullPath)
            ' MsgBox("File " & e.FullPath & " has been detected")
            MNewFile = e.FullPath 'This is the new found file.
            ReadImageFileQuick() ' testing a quicker way to load a bitmap. This one replaces the ReadImageFile() and FillImageArray() for speed. No Binning and no pixel math.
            ' ReadImageFile() 'This function will load the file into memory into the imagedata array. This is slower as it has the ability to add read noise etc.
            ' FillImageArray() ' This one manipulates the image data and makes the Image Array out of it.
            Watchfolder.EnableRaisingEvents = False 'Stop monitoring

            _mImageReady = True

            MCameraState = CameraStates.cameraIdle
        End If

    End Sub


    Public Property StartX() As Integer Implements ICameraV2.StartX
        Get
            _tl.LogMessage("StartX Get", _cameraStartX.ToString())
            Return _cameraStartX
        End Get
        Set(value As Integer)
            _cameraStartX = value
            _tl.LogMessage("StartX set", value.ToString())
        End Set
    End Property

    Public Property StartY() As Integer Implements ICameraV2.StartY
        Get
            _tl.LogMessage("StartY Get", _cameraStartY.ToString())
            Return _cameraStartY
        End Get
        Set(value As Integer)
            _cameraStartY = value
            _tl.LogMessage("StartY set", value.ToString())
        End Set
    End Property

    Public Sub StopExposure() Implements ICameraV2.StopExposure
        If Not Connected Then
            Throw New NotConnectedException("Can't stop exposure when not connected")
        End If
        If Not MCanStopExposure Then
            Throw New MethodNotImplementedException("AbortExposure")
        End If
        _tl.LogMessage("Stop Exposure", "Abort Exposure")
        Select Case MCameraState
            Case CameraStates.cameraWaiting, CameraStates.cameraExposing, CameraStates.cameraReading, CameraStates.cameraDownload
                ' these are all possible exposure states so we can stop the exposure
                MCameraState = CameraStates.cameraReading
                _exposureTimer.Enabled = False
                If _serialPort1.IsOpen = True Then
                    _serialPort1.RtsEnable = False
                    _serialPort1.Close()
                End If

                'Stop exposure still grabs the picture. Abort does not.
                'Folder monitoring will grab the newest file
                MCameraState = CameraStates.cameraDownload
                Watchfolder.Path = FolderToBeMonitored
                Watchfolder.Filter = MNewFileFilter
                Watchfolder.EnableRaisingEvents = True 'Start monitoring

                Exit Select
            Case CameraStates.cameraIdle
                Exit Select
            Case CameraStates.cameraError
                Throw New InvalidOperationException("Stop Exposure not possible because camera is in an error state")
        End Select


        ' Stop exposure will still read the image file. While abort will not.

    End Sub

#End Region

#Region "Private properties and methods"
    ' here are some useful properties and methods that can be used as required
    ' to help with

#Region "ASCOM Registration"

    Private Shared Sub RegUnregAscom(ByVal bRegister As Boolean)

        Using p As New Profile() With {.DeviceType = "Camera"}
            If bRegister Then
                p.Register(DriverId, DriverDescription)
            Else
                p.Unregister(DriverId)
            End If
        End Using

    End Sub

    <ComRegisterFunction()> _
    Public Shared Sub RegisterAscom(ByVal T As Type)

        RegUnregAscom(True)

    End Sub

    <ComUnregisterFunction()> _
    Public Shared Sub UnregisterAscom(ByVal T As Type)

        RegUnregAscom(False)

    End Sub

#End Region

    ''' <summary>
    ''' Returns true if there is a valid connection to the driver hardware
    ''' </summary>
    Private ReadOnly Property IsConnected As Boolean
        Get
            ' TODO check that the driver hardware connection exists and is connected to the hardware
            Return _connectedState
        End Get
    End Property

    ''' <summary>
    ''' Use this function to throw an exception if we aren't connected to the hardware
    ''' </summary>
    ''' <param name="message"></param>
    Private Sub CheckConnected(ByVal message As String)
        If Not IsConnected Then
            Throw New NotConnectedException(message)
        End If
    End Sub

    ' get data using the sensor types. For DSLR only use color...
    Private Sub ColorData(x As Integer, y As Integer)
        _imageData(x, y, 0) = (_bmp.GetPixel(x, y).R)
        _imageData(x, y, 1) = (_bmp.GetPixel(x, y).G)
        _imageData(x, y, 2) = (_bmp.GetPixel(x, y).B)
    End Sub

    Private Delegate Sub GetData(x As Integer, y As Integer)
    Private _bmp As Bitmap
    ' bayer offsets
    Private _x0 As Integer
    Private _x1 As Integer
    Private _x2 As Integer
    Private _x3 As Integer

    Private _y0 As Integer
    Private _y1 As Integer
    Private _y2 As Integer
    Private _y3 As Integer

    Private _stepX As Integer
    Private _stepY As Integer


    Public Sub ReadImageFileQuick()
        'This is a quicker way than the one used in the simulator (ReadImageFile() and FillImageArray()).

        Try
            'The folder watcher flag a file as it is created even if it is not fully copied yet. This can lead the driver to open the file and get an whole black image.
            'So I can do this:

            Do While IsFileClosed(MNewFile) = False 'Waits for the file to be closed and available.
            Loop

            'Threading.Thread.Sleep(3000)
            _bmp = DirectCast(Image.FromFile(MNewFile), Bitmap) 'Load the newly discovered file

            ' Lock the bitmap's bits.   
            Dim rect As New Rectangle(0, 0, _bmp.Width, _bmp.Height)
            Dim bmpData As BitmapData = _bmp.LockBits(rect, ImageLockMode.ReadWrite, _bmp.PixelFormat)

            ' Get the address of the first line. 
            Dim ptr As IntPtr = bmpData.Scan0

            ' Declare an array to hold the bytes of the bitmap. 
            ' This code is specific to a bitmap with 24 bits per pixels. 

            ' Copy the RGB values into the array.
            ' System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes)

            'Format32BppArgb Given X and Y coordinates,  the address of the first element in the pixel is Scan0+(y * stride)+(x*4).
            'This Points to the blue byte. The following three bytes contain the green, red and alpha bytes.

            'Format24BppRgb Given X and Y coordinates, the address of the first element in the pixel is Scan0+(y*Stride)+(x*3). 
            'This points to the blue byte which is followed by the green and the red.

            Dim x As Integer
            Dim y As Integer

            For y = 0 To _bmp.Height - 1
                For x = 0 To _bmp.Width - 1
                    _cameraImageArray(x, y, 2) = Marshal.ReadByte(ptr, (bmpData.Stride * y) + (3 * x))
                    _cameraImageArray(x, y, 1) = Marshal.ReadByte(ptr + 1, (bmpData.Stride * y) + (3 * x))
                    _cameraImageArray(x, y, 0) = Marshal.ReadByte(ptr + 2, (bmpData.Stride * y) + (3 * x))

                    '_cameraImageArrayVariant
                Next
            Next

            ' Unlock the bits.
            _bmp.UnlockBits(bmpData)

        Catch
        End Try


        _bmp.Dispose() 'It looks like each file loaded into the host application remains locked. With this the files are unlocked
    End Sub

    Public Shared Function IsFileClosed(filename As String) As Boolean
        'Verify that the file being opened has been released from the application that generated it. This avoids opening a partial file that results in a black image.
        Try
            Using inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None)
                Return True
            End Using
        Catch generatedExceptionName As IOException
            Return False
        End Try
    End Function


    ''' <summary>
    ''' Read the device configuration from the ASCOM Profile store
    ''' </summary>
    Friend Sub ReadProfile()
        Using driverProfile As New Profile()
            driverProfile.DeviceType = "Camera"
            TraceState = Convert.ToBoolean(driverProfile.GetValue(DriverId, TraceStateProfileName, String.Empty, TraceStateDefault))
            ComPort = driverProfile.GetValue(DriverId, ComPortProfileName, String.Empty, ComPortDefault)
            FolderToBeMonitored = driverProfile.GetValue(DriverId, FolderToBeMonitoredProfileName, String.Empty, FolderToBeMonitoredDefault)
            MElectronsPerAdu = driverProfile.GetValue(DriverId, ElectronsPerAduProfileName, String.Empty, ElectronsPerAduDefault)
            MFullWellCapacity = driverProfile.GetValue(DriverId, FullWellCapacityProfileName, String.Empty, FullWellCapacityDefault)
            MCcdTemperature = driverProfile.GetValue(DriverId, CcdTemperatureProfileName, String.Empty, CcdTemperatureDefault)
            MHeatSinkTemperature = driverProfile.GetValue(DriverId, HeatSinkTemperatureProfileName, String.Empty, HeatSinkTemperatureDefault)
            MSensorWidthPx = driverProfile.GetValue(DriverId, SensorWidthPxProfileName, String.Empty, SensorWidthPxDefault)
            MSensorHeightPx = driverProfile.GetValue(DriverId, SensorHeightPxProfileName, String.Empty, SensorHeightPxDefault)
            MPixelWidthUm = driverProfile.GetValue(DriverId, PixelWidthUmProfileName, String.Empty, PixelWidthUmDefault)
            MPixelHeightUm = driverProfile.GetValue(DriverId, PixelHeighthUmProfileName, String.Empty, PixelHeightUmDefault)
            MExposureType = driverProfile.GetValue(DriverId, MExposureTypeProfileName, String.Empty, MExposureTypeDefault)
        End Using
    End Sub

    ''' <summary>
    ''' Write the device configuration to the  ASCOM  Profile store
    ''' </summary>
    Friend Sub WriteProfile()
        Using driverProfile As New Profile()
            driverProfile.DeviceType = "Camera"
            driverProfile.WriteValue(DriverId, TraceStateProfileName, TraceState.ToString())
            driverProfile.WriteValue(DriverId, ComPortProfileName, ComPort.ToString())
            driverProfile.WriteValue(DriverId, FolderToBeMonitoredProfileName, FolderToBeMonitored.ToString())
            driverProfile.WriteValue(DriverId, ElectronsPerAduProfileName, MElectronsPerAdu.ToString())
            driverProfile.WriteValue(DriverId, FullWellCapacityProfileName, MFullWellCapacity.ToString())
            driverProfile.WriteValue(DriverId, CcdTemperatureProfileName, MCcdTemperature.ToString())
            driverProfile.WriteValue(DriverId, HeatSinkTemperatureProfileName, MHeatSinkTemperature.ToString())
            driverProfile.WriteValue(DriverId, SensorWidthPxProfileName, MSensorWidthPx.ToString())
            driverProfile.WriteValue(DriverId, SensorHeightPxProfileName, MSensorHeightPx.ToString())
            driverProfile.WriteValue(DriverId, PixelWidthUmProfileName, MPixelWidthUm.ToString())
            driverProfile.WriteValue(DriverId, PixelHeighthUmProfileName, MPixelHeightUm.ToString())
            driverProfile.WriteValue(DriverId, MExposureTypeProfileName, MExposureType.ToString())


        End Using

    End Sub

#End Region

End Class
