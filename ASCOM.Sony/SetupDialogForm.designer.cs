namespace ASCOM.Sony
{
    partial class SetupDialogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.picASCOM = new System.Windows.Forms.PictureBox();
            this.chkTrace = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCameraModel = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbISO = new System.Windows.Forms.ComboBox();
            this.checkAutoDeleteFile = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.cbImageFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbShowCameraSettings = new System.Windows.Forms.CheckBox();
            this.pnlCustomCamera = new System.Windows.Forms.Panel();
            this.lbShutterSpeed = new System.Windows.Forms.ListBox();
            this.label22 = new System.Windows.Forms.Label();
            this.lbISO = new System.Windows.Forms.ListBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txtExposureMax = new System.Windows.Forms.TextBox();
            this.txtExposureMin = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtSensorName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtSensorSizeHeight = new System.Windows.Forms.TextBox();
            this.txtSensorSizeWidth = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtPixelSizeHeight = new System.Windows.Forms.TextBox();
            this.txtPixelSizeWidth = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtCropSizeHeight = new System.Windows.Forms.TextBox();
            this.txtCropSizeWidth = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.txtFrameSizeHeight = new System.Windows.Forms.TextBox();
            this.txtFrameSizeWidth = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.txtCameraModel = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.btnAddISO = new System.Windows.Forms.LinkLabel();
            this.btnAddShutterSpeed = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).BeginInit();
            this.pnlCustomCamera.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(778, 491);
            this.cmdOK.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(88, 37);
            this.cmdOK.TabIndex = 0;
            this.cmdOK.Text = "OK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(778, 537);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(88, 38);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // picASCOM
            // 
            this.picASCOM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picASCOM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picASCOM.Image = global::ASCOM.Sony.Properties.Resources.ASCOM;
            this.picASCOM.Location = new System.Drawing.Point(18, 490);
            this.picASCOM.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.picASCOM.Name = "picASCOM";
            this.picASCOM.Size = new System.Drawing.Size(48, 56);
            this.picASCOM.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picASCOM.TabIndex = 3;
            this.picASCOM.TabStop = false;
            this.picASCOM.Click += new System.EventHandler(this.BrowseToAscom);
            this.picASCOM.DoubleClick += new System.EventHandler(this.BrowseToAscom);
            // 
            // chkTrace
            // 
            this.chkTrace.AutoSize = true;
            this.chkTrace.Location = new System.Drawing.Point(138, 235);
            this.chkTrace.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkTrace.Name = "chkTrace";
            this.chkTrace.Size = new System.Drawing.Size(97, 24);
            this.chkTrace.TabIndex = 6;
            this.chkTrace.Text = "Trace on";
            this.chkTrace.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 20);
            this.label3.TabIndex = 9;
            this.label3.Text = "Camera Model";
            // 
            // cbCameraModel
            // 
            this.cbCameraModel.FormattingEnabled = true;
            this.cbCameraModel.Location = new System.Drawing.Point(138, 17);
            this.cbCameraModel.Name = "cbCameraModel";
            this.cbCameraModel.Size = new System.Drawing.Size(200, 28);
            this.cbCameraModel.TabIndex = 10;
            this.cbCameraModel.SelectedValueChanged += new System.EventHandler(this.cbCameraModel_SelectedValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(94, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 20);
            this.label2.TabIndex = 11;
            this.label2.Text = "ISO";
            // 
            // cbISO
            // 
            this.cbISO.FormattingEnabled = true;
            this.cbISO.Location = new System.Drawing.Point(138, 54);
            this.cbISO.Name = "cbISO";
            this.cbISO.Size = new System.Drawing.Size(121, 28);
            this.cbISO.TabIndex = 12;
            // 
            // checkAutoDeleteFile
            // 
            this.checkAutoDeleteFile.AutoSize = true;
            this.checkAutoDeleteFile.Location = new System.Drawing.Point(138, 205);
            this.checkAutoDeleteFile.Name = "checkAutoDeleteFile";
            this.checkAutoDeleteFile.Size = new System.Drawing.Size(189, 24);
            this.checkAutoDeleteFile.TabIndex = 13;
            this.checkAutoDeleteFile.Text = "Auto-delete image file";
            this.toolTip1.SetToolTip(this.checkAutoDeleteFile, "Check if you want captured image file be deleted automatically after image data r" +
        "eturned by driver (will keep your monitoring folder clear)");
            this.checkAutoDeleteFile.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 20);
            this.label4.TabIndex = 14;
            this.label4.Text = "Image Format";
            // 
            // cbImageFormat
            // 
            this.cbImageFormat.FormattingEnabled = true;
            this.cbImageFormat.Location = new System.Drawing.Point(138, 91);
            this.cbImageFormat.Name = "cbImageFormat";
            this.cbImageFormat.Size = new System.Drawing.Size(121, 28);
            this.cbImageFormat.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(134, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(345, 20);
            this.label1.TabIndex = 16;
            this.label1.Text = "Note: Image Format selection is not automated.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(134, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(317, 20);
            this.label5.TabIndex = 17;
            this.label5.Text = "Please ensure you have matched file format";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(134, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(284, 20);
            this.label6.TabIndex = 18;
            this.label6.Text = "selected in Imaging Edge Remote app.";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(0, 470);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(1352, 2);
            this.label7.TabIndex = 20;
            // 
            // cbShowCameraSettings
            // 
            this.cbShowCameraSettings.AutoSize = true;
            this.cbShowCameraSettings.Location = new System.Drawing.Point(344, 19);
            this.cbShowCameraSettings.Name = "cbShowCameraSettings";
            this.cbShowCameraSettings.Size = new System.Drawing.Size(150, 24);
            this.cbShowCameraSettings.TabIndex = 22;
            this.cbShowCameraSettings.Text = "Custom Camera";
            this.cbShowCameraSettings.UseVisualStyleBackColor = true;
            this.cbShowCameraSettings.CheckedChanged += new System.EventHandler(this.cbShowCameraSettings_CheckedChanged);
            // 
            // pnlCustomCamera
            // 
            this.pnlCustomCamera.Controls.Add(this.btnAddShutterSpeed);
            this.pnlCustomCamera.Controls.Add(this.btnAddISO);
            this.pnlCustomCamera.Controls.Add(this.lbShutterSpeed);
            this.pnlCustomCamera.Controls.Add(this.label22);
            this.pnlCustomCamera.Controls.Add(this.lbISO);
            this.pnlCustomCamera.Controls.Add(this.label21);
            this.pnlCustomCamera.Controls.Add(this.label20);
            this.pnlCustomCamera.Controls.Add(this.txtExposureMax);
            this.pnlCustomCamera.Controls.Add(this.txtExposureMin);
            this.pnlCustomCamera.Controls.Add(this.label19);
            this.pnlCustomCamera.Controls.Add(this.txtSensorName);
            this.pnlCustomCamera.Controls.Add(this.label18);
            this.pnlCustomCamera.Controls.Add(this.label13);
            this.pnlCustomCamera.Controls.Add(this.label16);
            this.pnlCustomCamera.Controls.Add(this.txtSensorSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtSensorSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label17);
            this.pnlCustomCamera.Controls.Add(this.label14);
            this.pnlCustomCamera.Controls.Add(this.txtPixelSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtPixelSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label15);
            this.pnlCustomCamera.Controls.Add(this.label12);
            this.pnlCustomCamera.Controls.Add(this.label9);
            this.pnlCustomCamera.Controls.Add(this.label10);
            this.pnlCustomCamera.Controls.Add(this.txtCropSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtCropSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label11);
            this.pnlCustomCamera.Controls.Add(this.label8);
            this.pnlCustomCamera.Controls.Add(this.label23);
            this.pnlCustomCamera.Controls.Add(this.txtFrameSizeHeight);
            this.pnlCustomCamera.Controls.Add(this.txtFrameSizeWidth);
            this.pnlCustomCamera.Controls.Add(this.label24);
            this.pnlCustomCamera.Controls.Add(this.txtCameraModel);
            this.pnlCustomCamera.Controls.Add(this.label25);
            this.pnlCustomCamera.Location = new System.Drawing.Point(446, 3);
            this.pnlCustomCamera.Name = "pnlCustomCamera";
            this.pnlCustomCamera.Size = new System.Drawing.Size(438, 466);
            this.pnlCustomCamera.TabIndex = 23;
            // 
            // lbShutterSpeed
            // 
            this.lbShutterSpeed.FormattingEnabled = true;
            this.lbShutterSpeed.ItemHeight = 20;
            this.lbShutterSpeed.Location = new System.Drawing.Point(172, 366);
            this.lbShutterSpeed.Name = "lbShutterSpeed";
            this.lbShutterSpeed.Size = new System.Drawing.Size(245, 84);
            this.lbShutterSpeed.Sorted = true;
            this.lbShutterSpeed.TabIndex = 109;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(43, 366);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(123, 20);
            this.label22.TabIndex = 108;
            this.label22.Text = "Shutter Speed *";
            // 
            // lbISO
            // 
            this.lbISO.FormattingEnabled = true;
            this.lbISO.ItemHeight = 20;
            this.lbISO.Location = new System.Drawing.Point(172, 266);
            this.lbISO.Name = "lbISO";
            this.lbISO.Size = new System.Drawing.Size(245, 84);
            this.lbISO.Sorted = true;
            this.lbISO.TabIndex = 107;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(119, 263);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(47, 20);
            this.label21.TabIndex = 106;
            this.label21.Text = "ISO *";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(327, 228);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(69, 20);
            this.label20.TabIndex = 105;
            this.label20.Text = "seconds";
            // 
            // txtExposureMax
            // 
            this.txtExposureMax.Location = new System.Drawing.Point(263, 225);
            this.txtExposureMax.Name = "txtExposureMax";
            this.txtExposureMax.Size = new System.Drawing.Size(63, 26);
            this.txtExposureMax.TabIndex = 104;
            // 
            // txtExposureMin
            // 
            this.txtExposureMin.Location = new System.Drawing.Point(172, 225);
            this.txtExposureMin.Name = "txtExposureMin";
            this.txtExposureMin.Size = new System.Drawing.Size(63, 26);
            this.txtExposureMin.TabIndex = 103;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(10, 228);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(156, 20);
            this.label19.TabIndex = 102;
            this.label19.Text = "Exposure Min / Max *";
            // 
            // txtSensorName
            // 
            this.txtSensorName.Location = new System.Drawing.Point(172, 49);
            this.txtSensorName.Name = "txtSensorName";
            this.txtSensorName.Size = new System.Drawing.Size(245, 26);
            this.txtSensorName.TabIndex = 101;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(50, 52);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(110, 20);
            this.label18.TabIndex = 100;
            this.label18.Text = "Sensor Name ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(332, 87);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(35, 20);
            this.label13.TabIndex = 99;
            this.label13.Text = "mm";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(241, 228);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(13, 20);
            this.label16.TabIndex = 98;
            this.label16.Text = "/";
            // 
            // txtSensorSizeHeight
            // 
            this.txtSensorSizeHeight.Location = new System.Drawing.Point(263, 84);
            this.txtSensorSizeHeight.Name = "txtSensorSizeHeight";
            this.txtSensorSizeHeight.Size = new System.Drawing.Size(63, 26);
            this.txtSensorSizeHeight.TabIndex = 97;
            // 
            // txtSensorSizeWidth
            // 
            this.txtSensorSizeWidth.Location = new System.Drawing.Point(172, 84);
            this.txtSensorSizeWidth.Name = "txtSensorSizeWidth";
            this.txtSensorSizeWidth.Size = new System.Drawing.Size(63, 26);
            this.txtSensorSizeWidth.TabIndex = 96;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(63, 87);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(103, 20);
            this.label17.TabIndex = 95;
            this.label17.Text = "Sensor Size  ";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(241, 193);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(16, 20);
            this.label14.TabIndex = 94;
            this.label14.Text = "x";
            // 
            // txtPixelSizeHeight
            // 
            this.txtPixelSizeHeight.Location = new System.Drawing.Point(263, 190);
            this.txtPixelSizeHeight.Name = "txtPixelSizeHeight";
            this.txtPixelSizeHeight.Size = new System.Drawing.Size(63, 26);
            this.txtPixelSizeHeight.TabIndex = 93;
            // 
            // txtPixelSizeWidth
            // 
            this.txtPixelSizeWidth.Location = new System.Drawing.Point(172, 190);
            this.txtPixelSizeWidth.Name = "txtPixelSizeWidth";
            this.txtPixelSizeWidth.Size = new System.Drawing.Size(63, 26);
            this.txtPixelSizeWidth.TabIndex = 92;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(80, 193);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(80, 20);
            this.label15.TabIndex = 91;
            this.label15.Text = "Pixel Size ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(332, 193);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(31, 20);
            this.label12.TabIndex = 90;
            this.label12.Text = "μm";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(332, 158);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 20);
            this.label9.TabIndex = 89;
            this.label9.Text = "pixels";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(241, 158);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 20);
            this.label10.TabIndex = 88;
            this.label10.Text = "x";
            // 
            // txtCropSizeHeight
            // 
            this.txtCropSizeHeight.Location = new System.Drawing.Point(263, 155);
            this.txtCropSizeHeight.Name = "txtCropSizeHeight";
            this.txtCropSizeHeight.Size = new System.Drawing.Size(63, 26);
            this.txtCropSizeHeight.TabIndex = 87;
            // 
            // txtCropSizeWidth
            // 
            this.txtCropSizeWidth.Location = new System.Drawing.Point(172, 155);
            this.txtCropSizeWidth.Name = "txtCropSizeWidth";
            this.txtCropSizeWidth.Size = new System.Drawing.Size(63, 26);
            this.txtCropSizeWidth.TabIndex = 86;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(80, 158);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(86, 20);
            this.label11.TabIndex = 85;
            this.label11.Text = "Crop Size  ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(332, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 20);
            this.label8.TabIndex = 84;
            this.label8.Text = "pixels";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(241, 123);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(16, 20);
            this.label23.TabIndex = 83;
            this.label23.Text = "x";
            // 
            // txtFrameSizeHeight
            // 
            this.txtFrameSizeHeight.Location = new System.Drawing.Point(263, 120);
            this.txtFrameSizeHeight.Name = "txtFrameSizeHeight";
            this.txtFrameSizeHeight.Size = new System.Drawing.Size(63, 26);
            this.txtFrameSizeHeight.TabIndex = 82;
            // 
            // txtFrameSizeWidth
            // 
            this.txtFrameSizeWidth.Location = new System.Drawing.Point(172, 120);
            this.txtFrameSizeWidth.Name = "txtFrameSizeWidth";
            this.txtFrameSizeWidth.Size = new System.Drawing.Size(63, 26);
            this.txtFrameSizeWidth.TabIndex = 81;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(66, 123);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(100, 20);
            this.label24.TabIndex = 80;
            this.label24.Text = "Frame Size *";
            // 
            // txtCameraModel
            // 
            this.txtCameraModel.Location = new System.Drawing.Point(172, 14);
            this.txtCameraModel.Name = "txtCameraModel";
            this.txtCameraModel.Size = new System.Drawing.Size(245, 26);
            this.txtCameraModel.TabIndex = 79;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(104, 17);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(62, 20);
            this.label25.TabIndex = 78;
            this.label25.Text = "Model *";
            // 
            // btnAddISO
            // 
            this.btnAddISO.AutoSize = true;
            this.btnAddISO.Location = new System.Drawing.Point(90, 294);
            this.btnAddISO.Name = "btnAddISO";
            this.btnAddISO.Size = new System.Drawing.Size(70, 20);
            this.btnAddISO.TabIndex = 110;
            this.btnAddISO.TabStop = true;
            this.btnAddISO.Text = "Add ISO";
            this.btnAddISO.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnAddISO_LinkClicked);
            // 
            // btnAddShutterSpeed
            // 
            this.btnAddShutterSpeed.AutoSize = true;
            this.btnAddShutterSpeed.Location = new System.Drawing.Point(14, 400);
            this.btnAddShutterSpeed.Name = "btnAddShutterSpeed";
            this.btnAddShutterSpeed.Size = new System.Drawing.Size(146, 20);
            this.btnAddShutterSpeed.TabIndex = 111;
            this.btnAddShutterSpeed.TabStop = true;
            this.btnAddShutterSpeed.Text = "Add Shutter Speed";
            this.btnAddShutterSpeed.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.btnAddShutterSpeed_LinkClicked);
            // 
            // SetupDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 594);
            this.Controls.Add(this.cbShowCameraSettings);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbImageFormat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkAutoDeleteFile);
            this.Controls.Add(this.cbISO);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbCameraModel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkTrace);
            this.Controls.Add(this.picASCOM);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.pnlCustomCamera);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetupDialogForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ASCOM Sony Camera Settings";
            ((System.ComponentModel.ISupportInitialize)(this.picASCOM)).EndInit();
            this.pnlCustomCamera.ResumeLayout(false);
            this.pnlCustomCamera.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.PictureBox picASCOM;
        private System.Windows.Forms.CheckBox chkTrace;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbCameraModel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbISO;
        private System.Windows.Forms.CheckBox checkAutoDeleteFile;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbImageFormat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbShowCameraSettings;
        private System.Windows.Forms.Panel pnlCustomCamera;
        private System.Windows.Forms.ListBox lbShutterSpeed;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.ListBox lbISO;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtExposureMax;
        private System.Windows.Forms.TextBox txtExposureMin;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtSensorName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtSensorSizeHeight;
        private System.Windows.Forms.TextBox txtSensorSizeWidth;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtPixelSizeHeight;
        private System.Windows.Forms.TextBox txtPixelSizeWidth;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtCropSizeHeight;
        private System.Windows.Forms.TextBox txtCropSizeWidth;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtFrameSizeHeight;
        private System.Windows.Forms.TextBox txtFrameSizeWidth;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtCameraModel;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.LinkLabel btnAddShutterSpeed;
        private System.Windows.Forms.LinkLabel btnAddISO;
    }
}