using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.Sony;

namespace ASCOM.Sony
{
    [ComVisible(false)]					// Form not registered for COM!
    public partial class SetupDialogForm : Form
    {
        public SetupDialogForm()
        {
            InitializeComponent();
            // Initialise current values of user settings from the ASCOM Profile
            InitUI();
        }

        private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
        {
            // Place any validation constraint checks here
            // Update the state variables with results from the dialogue
            Camera.cameraModel = (cbCameraModel.SelectedItem as CameraModel) ?? CameraModel.Models.First();
            Camera.iso = (short?) cbISO.SelectedItem ?? Camera.cameraModel.Gains.First();
            Camera.imageFormat = (ImageFormat?) cbImageFormat.SelectedItem ?? ImageFormat.CFA;
            Camera.autoDeleteImageFile = checkAutoDeleteFile.Checked;
            Camera.tl.Enabled = chkTrace.Checked;
        }

        private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
        {
            Close();
        }

        private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
        {
            try
            {
                System.Diagnostics.Process.Start("http://ascom-standards.org/");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private CameraModel _cameraModel;

        private void InitUI()
        {
            _cameraModel = Camera.cameraModel;
            chkTrace.Checked = Camera.tl.Enabled;

            cbImageFormat.Items.Clear();
            cbImageFormat.Items.AddRange(new[]
            {
                (object)ImageFormat.CFA,
                ImageFormat.Debayered,
                ImageFormat.JPG
            });
            cbImageFormat.SelectedItem = Camera.imageFormat;

            checkAutoDeleteFile.Checked = Camera.autoDeleteImageFile;

            cbCameraModel.Items.Clear();
            cbCameraModel.Items.AddRange(CameraModel.Models.Select(m=>(object)m).ToArray());
            cbCameraModel.DisplayMember = "Name";
            if (string.IsNullOrEmpty(_cameraModel.ID) == false)
            {
                cbCameraModel.SelectedItem = _cameraModel;
            }
            
            cbISO.Items.Clear();
            if (_cameraModel != null)
            {
                cbISO.Items.AddRange(Camera.cameraModel.Gains.Select(iso=>(object)iso).ToArray());
                cbISO.SelectedItem = Camera.iso;

                txtCameraModel.Text = _cameraModel.Name;
                txtSensorName.Text = _cameraModel.Sensor.Name;
                txtSensorSizeWidth.Text = _cameraModel.Sensor.Width.ToString();
                txtSensorSizeHeight.Text = _cameraModel.Sensor.Height.ToString();
                txtFrameSizeWidth.Text = _cameraModel.Sensor.FrameWidth.ToString();
                txtFrameSizeHeight.Text = _cameraModel.Sensor.FrameHeight.ToString();
                txtCropSizeWidth.Text = _cameraModel.Sensor.CropWidth.ToString();
                txtCropSizeHeight.Text = _cameraModel.Sensor.CropHeight.ToString();
                txtPixelSizeWidth.Text = _cameraModel.Sensor.PixelSizeWidth.ToString();
                txtPixelSizeHeight.Text = _cameraModel.Sensor.PixelSizeHeight.ToString();
                txtExposureMin.Text = _cameraModel.ExposureMin.ToString();
                txtExposureMax.Text = _cameraModel.ExposureMax.ToString();

                lbISO.Items.Clear();
                lbISO.Items.Add(_cameraModel.Gains.Select(g => g.ToString()));

                lbShutterSpeed.Items.Clear();
                lbShutterSpeed.Items.Add(_cameraModel.ShutterSpeeds.Select(s => $"{s.Name};{s.DurationSeconds.ToString()}"));
            }

            ToggleCameraSettings();
        }

        private void cbCameraModel_SelectedValueChanged(object sender, EventArgs e)
        {
            cbISO.Items.Clear();
            CameraModel selectedCameraModel = (CameraModel) cbCameraModel.SelectedItem;

            if (selectedCameraModel != null)
            {
                cbISO.Items.AddRange(selectedCameraModel.Gains.Select(iso => (object)iso).ToArray());
                if (selectedCameraModel.Gains.Contains(Camera.iso))
                {
                    cbISO.SelectedItem = Camera.iso;
                }
                else
                {
                    cbISO.SelectedItem = selectedCameraModel.Gains.FirstOrDefault();
                }
            }
        }

        private bool _showCustomCameraControls = false;

        private void cbShowCameraSettings_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCameraSettings();
        }

        private void ToggleCameraSettings()
        {
            _showCustomCameraControls = cbShowCameraSettings.Checked;
            pnlCustomCamera.Enabled = _showCustomCameraControls;
            cbCameraModel.Enabled = !_showCustomCameraControls;
        }

        private void btnAddISO_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void btnAddShutterSpeed_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}