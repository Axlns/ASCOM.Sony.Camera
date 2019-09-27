<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SetupDialogForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.chkTrace = New System.Windows.Forms.CheckBox()
        Me.ComboBoxShutterComPort = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.RadioButtonMLUBulb = New System.Windows.Forms.RadioButton()
        Me.RadioButtonBulb = New System.Windows.Forms.RadioButton()
        Me.NumericUpMLU = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpBulb = New System.Windows.Forms.NumericUpDown()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.ButtonFolderDialog = New System.Windows.Forms.Button()
        Me.TextBoxFolderToMonitor = New System.Windows.Forms.TextBox()
        Me.NumericUpDownSensorSizeX = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDownSensorSizeY = New System.Windows.Forms.NumericUpDown()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBoxPixelSizeX = New System.Windows.Forms.TextBox()
        Me.TextBoxPixelSizeY = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.GroupBoxExposure = New System.Windows.Forms.GroupBox()
        Me.GroupBoxSensor = New System.Windows.Forms.GroupBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Label10 = New System.Windows.Forms.Label()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpMLU, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpBulb, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDownSensorSizeX, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDownSensorSizeY, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBoxExposure.SuspendLayout()
        Me.GroupBoxSensor.SuspendLayout()
        Me.SuspendLayout()
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(83, 335)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(167, 335)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBox1.Image = Global.ASCOM.PentaxDSLR.My.Resources.Resources.ASCOM
        Me.PictureBox1.Location = New System.Drawing.Point(19, 302)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(48, 56)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        Me.ToolTip1.SetToolTip(Me.PictureBox1, "Go to the ASCOM website.")
        '
        'chkTrace
        '
        Me.chkTrace.AutoSize = True
        Me.chkTrace.Location = New System.Drawing.Point(19, 279)
        Me.chkTrace.Name = "chkTrace"
        Me.chkTrace.Size = New System.Drawing.Size(69, 17)
        Me.chkTrace.TabIndex = 8
        Me.chkTrace.Text = "Trace on"
        Me.ToolTip1.SetToolTip(Me.chkTrace, "This will enable a very detailed log of the driver's actions. It is used mostly f" & _
        "or deugging. Log files are saved to My Dcocuments\ASCOM\logs.")
        Me.chkTrace.UseVisualStyleBackColor = True
        '
        'ComboBoxShutterComPort
        '
        Me.ComboBoxShutterComPort.FormattingEnabled = True
        Me.ComboBoxShutterComPort.Location = New System.Drawing.Point(107, 14)
        Me.ComboBoxShutterComPort.Name = "ComboBoxShutterComPort"
        Me.ComboBoxShutterComPort.Size = New System.Drawing.Size(127, 21)
        Me.ComboBoxShutterComPort.TabIndex = 9
        Me.ToolTip1.SetToolTip(Me.ComboBoxShutterComPort, "Select the serial port connected to the shutter release cable.")
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(16, 17)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 13)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "Shutter Com Port"
        '
        'RadioButtonMLUBulb
        '
        Me.RadioButtonMLUBulb.AutoSize = True
        Me.RadioButtonMLUBulb.Checked = True
        Me.RadioButtonMLUBulb.Location = New System.Drawing.Point(6, 19)
        Me.RadioButtonMLUBulb.Name = "RadioButtonMLUBulb"
        Me.RadioButtonMLUBulb.Size = New System.Drawing.Size(72, 17)
        Me.RadioButtonMLUBulb.TabIndex = 11
        Me.RadioButtonMLUBulb.TabStop = True
        Me.RadioButtonMLUBulb.Text = "MLU Bulb"
        Me.ToolTip1.SetToolTip(Me.RadioButtonMLUBulb, "Exposure type where a first shutter action flips the reflex mirror up, and after " & _
        "the selected delay, a second shutter action will start the exposure.")
        Me.RadioButtonMLUBulb.UseVisualStyleBackColor = True
        '
        'RadioButtonBulb
        '
        Me.RadioButtonBulb.AutoSize = True
        Me.RadioButtonBulb.Location = New System.Drawing.Point(6, 45)
        Me.RadioButtonBulb.Name = "RadioButtonBulb"
        Me.RadioButtonBulb.Size = New System.Drawing.Size(46, 17)
        Me.RadioButtonBulb.TabIndex = 11
        Me.RadioButtonBulb.Text = "Bulb"
        Me.ToolTip1.SetToolTip(Me.RadioButtonBulb, "Exposure type with a single shutter action. If the camera is using a predefined m" & _
        "irror lock up function, then select that time delay here so the exposure time wi" & _
        "ll be adjusted accordingly.")
        Me.RadioButtonBulb.UseVisualStyleBackColor = True
        '
        'NumericUpMLU
        '
        Me.NumericUpMLU.Location = New System.Drawing.Point(96, 19)
        Me.NumericUpMLU.Name = "NumericUpMLU"
        Me.NumericUpMLU.Size = New System.Drawing.Size(50, 20)
        Me.NumericUpMLU.TabIndex = 12
        Me.NumericUpMLU.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'NumericUpBulb
        '
        Me.NumericUpBulb.Location = New System.Drawing.Point(96, 45)
        Me.NumericUpBulb.Name = "NumericUpBulb"
        Me.NumericUpBulb.Size = New System.Drawing.Size(50, 20)
        Me.NumericUpBulb.TabIndex = 13
        Me.NumericUpBulb.Value = New Decimal(New Integer() {2, 0, 0, 0})
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(159, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(49, 13)
        Me.Label2.TabIndex = 15
        Me.Label2.Text = "Seconds"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(159, 49)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(49, 13)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "Seconds"
        '
        'ButtonFolderDialog
        '
        Me.ButtonFolderDialog.Location = New System.Drawing.Point(17, 43)
        Me.ButtonFolderDialog.Name = "ButtonFolderDialog"
        Me.ButtonFolderDialog.Size = New System.Drawing.Size(84, 43)
        Me.ButtonFolderDialog.TabIndex = 17
        Me.ButtonFolderDialog.Text = "Folder to monitor"
        Me.ToolTip1.SetToolTip(Me.ButtonFolderDialog, "Folder to monitor. This is where the jpg files will be saved by the camera or by " & _
        "the camera's software.")
        Me.ButtonFolderDialog.UseVisualStyleBackColor = True
        '
        'TextBoxFolderToMonitor
        '
        Me.TextBoxFolderToMonitor.Location = New System.Drawing.Point(107, 55)
        Me.TextBoxFolderToMonitor.Name = "TextBoxFolderToMonitor"
        Me.TextBoxFolderToMonitor.Size = New System.Drawing.Size(127, 20)
        Me.TextBoxFolderToMonitor.TabIndex = 19
        Me.ToolTip1.SetToolTip(Me.TextBoxFolderToMonitor, "Folder to monitor. This is where the jpg files will be saved by the camera or by " & _
        "the camera's software.")
        '
        'NumericUpDownSensorSizeX
        '
        Me.NumericUpDownSensorSizeX.Location = New System.Drawing.Point(15, 37)
        Me.NumericUpDownSensorSizeX.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.NumericUpDownSensorSizeX.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDownSensorSizeX.Name = "NumericUpDownSensorSizeX"
        Me.NumericUpDownSensorSizeX.Size = New System.Drawing.Size(60, 20)
        Me.NumericUpDownSensorSizeX.TabIndex = 20
        Me.ToolTip1.SetToolTip(Me.NumericUpDownSensorSizeX, "Sensor width in px")
        Me.NumericUpDownSensorSizeX.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'NumericUpDownSensorSizeY
        '
        Me.NumericUpDownSensorSizeY.Location = New System.Drawing.Point(15, 63)
        Me.NumericUpDownSensorSizeY.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.NumericUpDownSensorSizeY.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericUpDownSensorSizeY.Name = "NumericUpDownSensorSizeY"
        Me.NumericUpDownSensorSizeY.Size = New System.Drawing.Size(60, 20)
        Me.NumericUpDownSensorSizeY.TabIndex = 21
        Me.ToolTip1.SetToolTip(Me.NumericUpDownSensorSizeY, "Sensor height in px")
        Me.NumericUpDownSensorSizeY.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 19)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(75, 13)
        Me.Label4.TabIndex = 22
        Me.Label4.Text = "Sensor size px"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(81, 39)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(14, 13)
        Me.Label5.TabIndex = 23
        Me.Label5.Text = "X"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(81, 65)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(14, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "Y"
        '
        'TextBoxPixelSizeX
        '
        Me.TextBoxPixelSizeX.Location = New System.Drawing.Point(128, 37)
        Me.TextBoxPixelSizeX.Name = "TextBoxPixelSizeX"
        Me.TextBoxPixelSizeX.Size = New System.Drawing.Size(57, 20)
        Me.TextBoxPixelSizeX.TabIndex = 25
        Me.ToolTip1.SetToolTip(Me.TextBoxPixelSizeX, "Pixel size X in microns")
        '
        'TextBoxPixelSizeY
        '
        Me.TextBoxPixelSizeY.Location = New System.Drawing.Point(128, 65)
        Me.TextBoxPixelSizeY.Name = "TextBoxPixelSizeY"
        Me.TextBoxPixelSizeY.Size = New System.Drawing.Size(57, 20)
        Me.TextBoxPixelSizeY.TabIndex = 26
        Me.ToolTip1.SetToolTip(Me.TextBoxPixelSizeY, "Pixel size Y in microns")
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(191, 66)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(14, 13)
        Me.Label7.TabIndex = 28
        Me.Label7.Text = "Y"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(191, 40)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(14, 13)
        Me.Label8.TabIndex = 27
        Me.Label8.Text = "X"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(125, 19)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(67, 13)
        Me.Label9.TabIndex = 29
        Me.Label9.Text = "Pixel size um"
        '
        'GroupBoxExposure
        '
        Me.GroupBoxExposure.Controls.Add(Me.RadioButtonMLUBulb)
        Me.GroupBoxExposure.Controls.Add(Me.RadioButtonBulb)
        Me.GroupBoxExposure.Controls.Add(Me.NumericUpMLU)
        Me.GroupBoxExposure.Controls.Add(Me.NumericUpBulb)
        Me.GroupBoxExposure.Controls.Add(Me.Label2)
        Me.GroupBoxExposure.Controls.Add(Me.Label1)
        Me.GroupBoxExposure.Location = New System.Drawing.Point(19, 92)
        Me.GroupBoxExposure.Name = "GroupBoxExposure"
        Me.GroupBoxExposure.Size = New System.Drawing.Size(215, 80)
        Me.GroupBoxExposure.TabIndex = 30
        Me.GroupBoxExposure.TabStop = False
        Me.GroupBoxExposure.Text = "Exposure type"
        '
        'GroupBoxSensor
        '
        Me.GroupBoxSensor.Controls.Add(Me.NumericUpDownSensorSizeX)
        Me.GroupBoxSensor.Controls.Add(Me.NumericUpDownSensorSizeY)
        Me.GroupBoxSensor.Controls.Add(Me.Label9)
        Me.GroupBoxSensor.Controls.Add(Me.Label4)
        Me.GroupBoxSensor.Controls.Add(Me.Label7)
        Me.GroupBoxSensor.Controls.Add(Me.Label5)
        Me.GroupBoxSensor.Controls.Add(Me.Label8)
        Me.GroupBoxSensor.Controls.Add(Me.Label6)
        Me.GroupBoxSensor.Controls.Add(Me.TextBoxPixelSizeY)
        Me.GroupBoxSensor.Controls.Add(Me.TextBoxPixelSizeX)
        Me.GroupBoxSensor.Location = New System.Drawing.Point(17, 178)
        Me.GroupBoxSensor.Name = "GroupBoxSensor"
        Me.GroupBoxSensor.Size = New System.Drawing.Size(217, 95)
        Me.GroupBoxSensor.TabIndex = 31
        Me.GroupBoxSensor.TabStop = False
        Me.GroupBoxSensor.Text = "Sensor details"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(193, 283)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(41, 13)
        Me.Label10.TabIndex = 32
        Me.Label10.Text = "Ver 0.3"
        '
        'SetupDialogForm
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(256, 372)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Cancel_Button)
        Me.Controls.Add(Me.OK_Button)
        Me.Controls.Add(Me.GroupBoxSensor)
        Me.Controls.Add(Me.GroupBoxExposure)
        Me.Controls.Add(Me.TextBoxFolderToMonitor)
        Me.Controls.Add(Me.ButtonFolderDialog)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ComboBoxShutterComPort)
        Me.Controls.Add(Me.chkTrace)
        Me.Controls.Add(Me.PictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SetupDialogForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Pentax DSLR Setup"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpMLU, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpBulb, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDownSensorSizeX, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDownSensorSizeY, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBoxExposure.ResumeLayout(False)
        Me.GroupBoxExposure.PerformLayout()
        Me.GroupBoxSensor.ResumeLayout(False)
        Me.GroupBoxSensor.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents chkTrace As System.Windows.Forms.CheckBox
    Friend WithEvents ComboBoxShutterComPort As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents RadioButtonMLUBulb As System.Windows.Forms.RadioButton
    Friend WithEvents RadioButtonBulb As System.Windows.Forms.RadioButton
    Friend WithEvents NumericUpMLU As System.Windows.Forms.NumericUpDown
    Friend WithEvents NumericUpBulb As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents ButtonFolderDialog As System.Windows.Forms.Button
    Friend WithEvents TextBoxFolderToMonitor As System.Windows.Forms.TextBox
    Friend WithEvents NumericUpDownSensorSizeX As System.Windows.Forms.NumericUpDown
    Friend WithEvents NumericUpDownSensorSizeY As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TextBoxPixelSizeX As System.Windows.Forms.TextBox
    Friend WithEvents TextBoxPixelSizeY As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents GroupBoxExposure As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBoxSensor As System.Windows.Forms.GroupBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents Label10 As System.Windows.Forms.Label

End Class
