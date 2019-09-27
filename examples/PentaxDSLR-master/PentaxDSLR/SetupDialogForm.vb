Imports System.Windows.Forms
Imports System.Runtime.InteropServices
'Imports ASCOM.Utilities
'Imports ASCOM.PentaxDSLR
'Imports System.Security.Cryptography.X509Certificates
Imports System.ComponentModel
Imports ASCOM.Utilities


<ComVisible(False)> _
Public Class SetupDialogForm

    Private Sub OK_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OK_Button.Click ' OK button event handler
        ' Persist new values of user settings to the ASCOM profile

        'Uses the Camera class defined in the Driver.vb to store settings.

        Camera.traceState = chkTrace.Checked 'Trace...
        Camera.comPort = DirectCast(ComboBoxShutterComPort.SelectedItem, String) 'Attach the combobox value to the camera comport


        'Makes the new runtime values from the user changes to the form.
        Camera.MSensorWidthPx = NumericUpDownSensorSizeX.Value
        Camera.MSensorHeightPx = NumericUpDownSensorSizeY.Value
        Camera.MPixelWidthUm = CType(TextBoxPixelSizeX.Text, Double)
        Camera.MPixelHeightUm = CType(TextBoxPixelSizeY.Text, Double)
        Camera.FolderToBeMonitored = TextBoxFolderToMonitor.Text

        'Exposure type, MLU or Bulb
        If RadioButtonBulb.Checked = True Then
            Camera.MExposureType = "B"
            Camera.MExposureDelayBulb = NumericUpBulb.Value
        Else
            Camera.MExposureType = "M"
            Camera.MExposureDelayMlu = NumericUpMLU.Value
        End If


        If (Camera.comPort = "" Or TextBoxFolderToMonitor.Text = "Folder to monitor") Then 'avoid closing the form while a COM port hasn't been selected

            MsgBox("Camera Port = " & Camera.comPort & " Please select a COM port AND a folder to be monitored")

        Else
            'MsgBox("Com Port= " & Camera.comPort & " " & "Folder= " & Camera.FolderToBeMonitored)
            Me.DialogResult = DialogResult.OK
            Me.Close()

        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Cancel_Button.Click 'Cancel button event handler
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ShowAscomWebPage(ByVal sender As Object, ByVal e As EventArgs) Handles PictureBox1.DoubleClick, PictureBox1.Click
        ' Click on ASCOM logo event handler
        Try
            Process.Start("http://ascom-standards.org/")
        Catch noBrowser As Win32Exception
            If noBrowser.ErrorCode = -2147467259 Then
                MessageBox.Show(noBrowser.Message)
            End If
        Catch other As Exception
            MessageBox.Show(other.Message)
        End Try
    End Sub

    Private Sub SetupDialogForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load ' Form load event handler
        ' Retrieve current values of user settings from the ASCOM Profile 



        chkTrace.Checked = Camera.traceState

        ComboBoxShutterComPort.Items.Clear()
        Using serial As Serial = New Serial()
            For Each item In serial.AvailableCOMPorts
                ComboBoxShutterComPort.Items.Add(item)
            Next
        End Using

        'Load defaults. Before opening the setup form, the ReadProfile() is called, so all the stored variables are loaded.
        ComboBoxShutterComPort.SelectedItem = Camera.comPort
        ComboBoxShutterComPort.SelectedValue = Camera.comPort
        ' ComboBoxShutterComPort.SelectedText = Camera.comPort
        'MsgBox(Camera.comPort)
        TextBoxFolderToMonitor.Text = Camera.FolderToBeMonitored
        NumericUpDownSensorSizeX.Value = Camera.MSensorWidthPx
        NumericUpDownSensorSizeY.Value = Camera.MSensorHeightPx
        TextBoxPixelSizeX.Text = Camera.MPixelWidthUm
        TextBoxPixelSizeY.Text = Camera.MPixelHeightUm

        'Exposure type from stored profile
        Select Case Camera.MExposureType
            Case "B"
                RadioButtonBulb.Checked = True

            Case Else
                RadioButtonMLUBulb.Checked = True
        End Select




    End Sub


    Private Sub ButtonFolderDialog_Click(sender As Object, e As EventArgs) Handles ButtonFolderDialog.Click

        'Initiate the selection of the folder to be monitored
        Dim folderDlg As New FolderBrowserDialog 'Define the folder dialog class

        folderDlg.ShowDialog() 'Show the folder selector

        TextBoxFolderToMonitor.Text = folderDlg.SelectedPath 'Show the selected path on the form
        Camera.FolderToBeMonitored = folderDlg.SelectedPath 'Stores the folder into the camera class

    End Sub

    Private Sub FolderTextChange(ByVal sender As Object, ByVal e As EventArgs) Handles TextBoxFolderToMonitor.TextChanged
        ' this sub gets called if a manual edit or copy\paste is done on the folder textbox
        If (TextBoxFolderToMonitor.Text <> Camera.FolderToBeMonitored And TextBoxFolderToMonitor.Text <> "") Then
            Camera.FolderToBeMonitored = TextBoxFolderToMonitor.Text
        End If


    End Sub

    Private Sub RadioButtonMLUBulb_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonMLUBulb.CheckedChanged

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBoxExposure.Enter

    End Sub
End Class
