namespace ILInspect {
    partial class ConnectionEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tableLayoutPanelMain = new TableLayoutPanel();
            this.tableLayoutPanelSelectConnection = new TableLayoutPanel();
            this.panelConnectionSelector = new Panel();
            this.listBoxSelectConnection = new ListBox();
            this.panelConnectionSelectorButtons = new Panel();
            this.labelSelectConnectionDivider = new Label();
            this.buttonClose = new Button();
            this.buttonConnectionRemove = new Button();
            this.buttonConnectionEdit = new Button();
            this.buttonConnectionAdd = new Button();
            this.tableLayoutPanelSelectSensor = new TableLayoutPanel();
            this.panelSensorSelector = new Panel();
            this.listBoxSelectSensor = new ListBox();
            this.panelSensorSelectorButtons = new Panel();
            this.buttonSensorRemove = new Button();
            this.buttonSensorEdit = new Button();
            this.buttonSensorAdd = new Button();
            this.panelSensorData = new Panel();
            this.comboBoxSensorDecimalPlaces = new ComboBox();
            this.labelSensorDecimals = new Label();
            this.textBoxSensorID = new TextBox();
            this.labelSensorID = new Label();
            this.buttonSensorCancel = new Button();
            this.buttonSensorOK = new Button();
            this.tableLayoutPanelCenter = new TableLayoutPanel();
            this.panelConnectionDataTop = new Panel();
            this.checkBoxConnectionLaserStop = new CheckBox();
            this.textBoxConnectionLaserDelay = new TextBox();
            this.labelConnectionLaserDelay = new Label();
            this.textBoxConnectionPort = new TextBox();
            this.textBoxConnectionHost = new TextBox();
            this.labelConnectionPort = new Label();
            this.labelConnectionHost = new Label();
            this.panelConnectionDataButtons = new Panel();
            this.buttonConnectionCancel = new Button();
            this.buttonConnectionOK = new Button();
            this.labelVerticalDivider1 = new Label();
            this.labelVerticalDividier2 = new Label();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelSelectConnection.SuspendLayout();
            this.panelConnectionSelector.SuspendLayout();
            this.panelConnectionSelectorButtons.SuspendLayout();
            this.tableLayoutPanelSelectSensor.SuspendLayout();
            this.panelSensorSelector.SuspendLayout();
            this.panelSensorSelectorButtons.SuspendLayout();
            this.panelSensorData.SuspendLayout();
            this.tableLayoutPanelCenter.SuspendLayout();
            this.panelConnectionDataTop.SuspendLayout();
            this.panelConnectionDataButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 7F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 7F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 350F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSelectConnection, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSelectSensor, 4, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelCenter, 2, 0);
            this.tableLayoutPanelMain.Controls.Add(this.labelVerticalDivider1, 1, 0);
            this.tableLayoutPanelMain.Controls.Add(this.labelVerticalDividier2, 3, 0);
            this.tableLayoutPanelMain.Dock = DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new Point(0, 0);
            this.tableLayoutPanelMain.Margin = new Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 1;
            this.tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new Size(844, 261);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelSelectConnection
            // 
            this.tableLayoutPanelSelectConnection.ColumnCount = 1;
            this.tableLayoutPanelSelectConnection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectConnection.Controls.Add(this.panelConnectionSelector, 0, 0);
            this.tableLayoutPanelSelectConnection.Controls.Add(this.panelConnectionSelectorButtons, 0, 1);
            this.tableLayoutPanelSelectConnection.Dock = DockStyle.Fill;
            this.tableLayoutPanelSelectConnection.Location = new Point(0, 0);
            this.tableLayoutPanelSelectConnection.Margin = new Padding(0);
            this.tableLayoutPanelSelectConnection.MinimumSize = new Size(120, 0);
            this.tableLayoutPanelSelectConnection.Name = "tableLayoutPanelSelectConnection";
            this.tableLayoutPanelSelectConnection.RowCount = 2;
            this.tableLayoutPanelSelectConnection.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectConnection.RowStyles.Add(new RowStyle(SizeType.Absolute, 138F));
            this.tableLayoutPanelSelectConnection.Size = new Size(160, 261);
            this.tableLayoutPanelSelectConnection.TabIndex = 0;
            // 
            // panelConnectionSelector
            // 
            this.panelConnectionSelector.Controls.Add(this.listBoxSelectConnection);
            this.panelConnectionSelector.Dock = DockStyle.Fill;
            this.panelConnectionSelector.Location = new Point(3, 3);
            this.panelConnectionSelector.Name = "panelConnectionSelector";
            this.panelConnectionSelector.Size = new Size(154, 117);
            this.panelConnectionSelector.TabIndex = 0;
            // 
            // listBoxSelectConnection
            // 
            this.listBoxSelectConnection.Dock = DockStyle.Fill;
            this.listBoxSelectConnection.FormattingEnabled = true;
            this.listBoxSelectConnection.ItemHeight = 15;
            this.listBoxSelectConnection.Location = new Point(0, 0);
            this.listBoxSelectConnection.Name = "listBoxSelectConnection";
            this.listBoxSelectConnection.ScrollAlwaysVisible = true;
            this.listBoxSelectConnection.Size = new Size(154, 117);
            this.listBoxSelectConnection.TabIndex = 0;
            this.listBoxSelectConnection.SelectedIndexChanged += this.listBoxSelectConnection_SelectedIndexChanged;
            // 
            // panelConnectionSelectorButtons
            // 
            this.panelConnectionSelectorButtons.Controls.Add(this.labelSelectConnectionDivider);
            this.panelConnectionSelectorButtons.Controls.Add(this.buttonClose);
            this.panelConnectionSelectorButtons.Controls.Add(this.buttonConnectionRemove);
            this.panelConnectionSelectorButtons.Controls.Add(this.buttonConnectionEdit);
            this.panelConnectionSelectorButtons.Controls.Add(this.buttonConnectionAdd);
            this.panelConnectionSelectorButtons.Dock = DockStyle.Fill;
            this.panelConnectionSelectorButtons.Location = new Point(0, 123);
            this.panelConnectionSelectorButtons.Margin = new Padding(0);
            this.panelConnectionSelectorButtons.Name = "panelConnectionSelectorButtons";
            this.panelConnectionSelectorButtons.Size = new Size(160, 138);
            this.panelConnectionSelectorButtons.TabIndex = 1;
            // 
            // labelSelectConnectionDivider
            // 
            this.labelSelectConnectionDivider.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.labelSelectConnectionDivider.BorderStyle = BorderStyle.Fixed3D;
            this.labelSelectConnectionDivider.Location = new Point(6, 96);
            this.labelSelectConnectionDivider.Name = "labelSelectConnectionDivider";
            this.labelSelectConnectionDivider.Size = new Size(148, 2);
            this.labelSelectConnectionDivider.TabIndex = 4;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonClose.Location = new Point(12, 103);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(132, 23);
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // buttonConnectionRemove
            // 
            this.buttonConnectionRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonConnectionRemove.Enabled = false;
            this.buttonConnectionRemove.Location = new Point(12, 66);
            this.buttonConnectionRemove.Name = "buttonConnectionRemove";
            this.buttonConnectionRemove.Size = new Size(132, 23);
            this.buttonConnectionRemove.TabIndex = 2;
            this.buttonConnectionRemove.Text = "Remove Connection";
            this.buttonConnectionRemove.UseVisualStyleBackColor = true;
            this.buttonConnectionRemove.Click += this.buttonConnectionRemove_Click;
            // 
            // buttonConnectionEdit
            // 
            this.buttonConnectionEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonConnectionEdit.Enabled = false;
            this.buttonConnectionEdit.Location = new Point(12, 37);
            this.buttonConnectionEdit.Name = "buttonConnectionEdit";
            this.buttonConnectionEdit.Size = new Size(132, 23);
            this.buttonConnectionEdit.TabIndex = 1;
            this.buttonConnectionEdit.Text = "Edit Connection";
            this.buttonConnectionEdit.UseVisualStyleBackColor = true;
            this.buttonConnectionEdit.Click += this.buttonConnectionEdit_Click;
            // 
            // buttonConnectionAdd
            // 
            this.buttonConnectionAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonConnectionAdd.Location = new Point(12, 8);
            this.buttonConnectionAdd.Name = "buttonConnectionAdd";
            this.buttonConnectionAdd.Size = new Size(132, 23);
            this.buttonConnectionAdd.TabIndex = 0;
            this.buttonConnectionAdd.Text = "Add Connection";
            this.buttonConnectionAdd.UseVisualStyleBackColor = true;
            this.buttonConnectionAdd.Click += this.buttonConnectionAdd_Click;
            // 
            // tableLayoutPanelSelectSensor
            // 
            this.tableLayoutPanelSelectSensor.ColumnCount = 2;
            this.tableLayoutPanelSelectSensor.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            this.tableLayoutPanelSelectSensor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectSensor.Controls.Add(this.panelSensorSelector, 0, 0);
            this.tableLayoutPanelSelectSensor.Controls.Add(this.panelSensorSelectorButtons, 0, 1);
            this.tableLayoutPanelSelectSensor.Controls.Add(this.panelSensorData, 1, 0);
            this.tableLayoutPanelSelectSensor.Dock = DockStyle.Fill;
            this.tableLayoutPanelSelectSensor.Location = new Point(494, 0);
            this.tableLayoutPanelSelectSensor.Margin = new Padding(0);
            this.tableLayoutPanelSelectSensor.MinimumSize = new Size(300, 0);
            this.tableLayoutPanelSelectSensor.Name = "tableLayoutPanelSelectSensor";
            this.tableLayoutPanelSelectSensor.RowCount = 2;
            this.tableLayoutPanelSelectSensor.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectSensor.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            this.tableLayoutPanelSelectSensor.Size = new Size(350, 261);
            this.tableLayoutPanelSelectSensor.TabIndex = 2;
            // 
            // panelSensorSelector
            // 
            this.panelSensorSelector.Controls.Add(this.listBoxSelectSensor);
            this.panelSensorSelector.Dock = DockStyle.Fill;
            this.panelSensorSelector.Location = new Point(3, 3);
            this.panelSensorSelector.Name = "panelSensorSelector";
            this.panelSensorSelector.Size = new Size(124, 155);
            this.panelSensorSelector.TabIndex = 0;
            // 
            // listBoxSelectSensor
            // 
            this.listBoxSelectSensor.Dock = DockStyle.Fill;
            this.listBoxSelectSensor.FormattingEnabled = true;
            this.listBoxSelectSensor.ItemHeight = 15;
            this.listBoxSelectSensor.Location = new Point(0, 0);
            this.listBoxSelectSensor.Name = "listBoxSelectSensor";
            this.listBoxSelectSensor.ScrollAlwaysVisible = true;
            this.listBoxSelectSensor.Size = new Size(124, 155);
            this.listBoxSelectSensor.TabIndex = 0;
            this.listBoxSelectSensor.SelectedIndexChanged += this.listBoxSelectSensor_SelectedIndexChanged;
            // 
            // panelSensorSelectorButtons
            // 
            this.panelSensorSelectorButtons.Controls.Add(this.buttonSensorRemove);
            this.panelSensorSelectorButtons.Controls.Add(this.buttonSensorEdit);
            this.panelSensorSelectorButtons.Controls.Add(this.buttonSensorAdd);
            this.panelSensorSelectorButtons.Dock = DockStyle.Fill;
            this.panelSensorSelectorButtons.Location = new Point(0, 161);
            this.panelSensorSelectorButtons.Margin = new Padding(0);
            this.panelSensorSelectorButtons.Name = "panelSensorSelectorButtons";
            this.panelSensorSelectorButtons.Size = new Size(130, 100);
            this.panelSensorSelectorButtons.TabIndex = 1;
            // 
            // buttonSensorRemove
            // 
            this.buttonSensorRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorRemove.Location = new Point(12, 65);
            this.buttonSensorRemove.Name = "buttonSensorRemove";
            this.buttonSensorRemove.Size = new Size(100, 23);
            this.buttonSensorRemove.TabIndex = 2;
            this.buttonSensorRemove.Text = "Remove Sensor";
            this.buttonSensorRemove.UseVisualStyleBackColor = true;
            this.buttonSensorRemove.Click += this.buttonSensorRemove_Click;
            // 
            // buttonSensorEdit
            // 
            this.buttonSensorEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorEdit.Location = new Point(12, 36);
            this.buttonSensorEdit.Name = "buttonSensorEdit";
            this.buttonSensorEdit.Size = new Size(100, 23);
            this.buttonSensorEdit.TabIndex = 1;
            this.buttonSensorEdit.Text = "Edit Sensor";
            this.buttonSensorEdit.UseVisualStyleBackColor = true;
            this.buttonSensorEdit.Click += this.buttonSensorEdit_Click;
            // 
            // buttonSensorAdd
            // 
            this.buttonSensorAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorAdd.Location = new Point(12, 7);
            this.buttonSensorAdd.Name = "buttonSensorAdd";
            this.buttonSensorAdd.Size = new Size(100, 23);
            this.buttonSensorAdd.TabIndex = 0;
            this.buttonSensorAdd.Text = "Add Sensor";
            this.buttonSensorAdd.UseVisualStyleBackColor = true;
            this.buttonSensorAdd.Click += this.buttonSensorAdd_Click;
            // 
            // panelSensorData
            // 
            this.panelSensorData.Controls.Add(this.comboBoxSensorDecimalPlaces);
            this.panelSensorData.Controls.Add(this.labelSensorDecimals);
            this.panelSensorData.Controls.Add(this.textBoxSensorID);
            this.panelSensorData.Controls.Add(this.labelSensorID);
            this.panelSensorData.Controls.Add(this.buttonSensorCancel);
            this.panelSensorData.Controls.Add(this.buttonSensorOK);
            this.panelSensorData.Dock = DockStyle.Fill;
            this.panelSensorData.Location = new Point(130, 0);
            this.panelSensorData.Margin = new Padding(0);
            this.panelSensorData.MinimumSize = new Size(220, 0);
            this.panelSensorData.Name = "panelSensorData";
            this.tableLayoutPanelSelectSensor.SetRowSpan(this.panelSensorData, 2);
            this.panelSensorData.Size = new Size(220, 261);
            this.panelSensorData.TabIndex = 2;
            // 
            // comboBoxSensorDecimalPlaces
            // 
            this.comboBoxSensorDecimalPlaces.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.comboBoxSensorDecimalPlaces.Enabled = false;
            this.comboBoxSensorDecimalPlaces.FormattingEnabled = true;
            this.comboBoxSensorDecimalPlaces.Location = new Point(109, 41);
            this.comboBoxSensorDecimalPlaces.Name = "comboBoxSensorDecimalPlaces";
            this.comboBoxSensorDecimalPlaces.Size = new Size(103, 23);
            this.comboBoxSensorDecimalPlaces.TabIndex = 3;
            // 
            // labelSensorDecimals
            // 
            this.labelSensorDecimals.Enabled = false;
            this.labelSensorDecimals.Location = new Point(3, 41);
            this.labelSensorDecimals.Name = "labelSensorDecimals";
            this.labelSensorDecimals.Size = new Size(100, 23);
            this.labelSensorDecimals.TabIndex = 2;
            this.labelSensorDecimals.Text = "Decimal Places:";
            this.labelSensorDecimals.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorID
            // 
            this.textBoxSensorID.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxSensorID.Enabled = false;
            this.textBoxSensorID.Location = new Point(109, 12);
            this.textBoxSensorID.Name = "textBoxSensorID";
            this.textBoxSensorID.Size = new Size(103, 23);
            this.textBoxSensorID.TabIndex = 1;
            // 
            // labelSensorID
            // 
            this.labelSensorID.Enabled = false;
            this.labelSensorID.Location = new Point(3, 12);
            this.labelSensorID.Name = "labelSensorID";
            this.labelSensorID.Size = new Size(100, 23);
            this.labelSensorID.TabIndex = 0;
            this.labelSensorID.Text = "ID:";
            this.labelSensorID.TextAlign = ContentAlignment.MiddleRight;
            // 
            // buttonSensorCancel
            // 
            this.buttonSensorCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.buttonSensorCancel.Enabled = false;
            this.buttonSensorCancel.Location = new Point(112, 226);
            this.buttonSensorCancel.Name = "buttonSensorCancel";
            this.buttonSensorCancel.Size = new Size(100, 23);
            this.buttonSensorCancel.TabIndex = 5;
            this.buttonSensorCancel.Text = "Cancel";
            this.buttonSensorCancel.UseVisualStyleBackColor = true;
            this.buttonSensorCancel.Click += this.buttonSensorCancel_Click;
            // 
            // buttonSensorOK
            // 
            this.buttonSensorOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorOK.Location = new Point(6, 226);
            this.buttonSensorOK.Name = "buttonSensorOK";
            this.buttonSensorOK.Size = new Size(100, 23);
            this.buttonSensorOK.TabIndex = 4;
            this.buttonSensorOK.Text = "OK";
            this.buttonSensorOK.UseVisualStyleBackColor = true;
            this.buttonSensorOK.Click += this.buttonSensorOK_Click;
            // 
            // tableLayoutPanelCenter
            // 
            this.tableLayoutPanelCenter.ColumnCount = 1;
            this.tableLayoutPanelCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this.tableLayoutPanelCenter.Controls.Add(this.panelConnectionDataTop, 0, 0);
            this.tableLayoutPanelCenter.Controls.Add(this.panelConnectionDataButtons, 0, 1);
            this.tableLayoutPanelCenter.Dock = DockStyle.Fill;
            this.tableLayoutPanelCenter.Location = new Point(167, 0);
            this.tableLayoutPanelCenter.Margin = new Padding(0);
            this.tableLayoutPanelCenter.MinimumSize = new Size(300, 0);
            this.tableLayoutPanelCenter.Name = "tableLayoutPanelCenter";
            this.tableLayoutPanelCenter.RowCount = 2;
            this.tableLayoutPanelCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelCenter.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            this.tableLayoutPanelCenter.Size = new Size(320, 261);
            this.tableLayoutPanelCenter.TabIndex = 1;
            // 
            // panelConnectionDataTop
            // 
            this.tableLayoutPanelCenter.SetColumnSpan(this.panelConnectionDataTop, 2);
            this.panelConnectionDataTop.Controls.Add(this.checkBoxConnectionLaserStop);
            this.panelConnectionDataTop.Controls.Add(this.textBoxConnectionLaserDelay);
            this.panelConnectionDataTop.Controls.Add(this.labelConnectionLaserDelay);
            this.panelConnectionDataTop.Controls.Add(this.textBoxConnectionPort);
            this.panelConnectionDataTop.Controls.Add(this.textBoxConnectionHost);
            this.panelConnectionDataTop.Controls.Add(this.labelConnectionPort);
            this.panelConnectionDataTop.Controls.Add(this.labelConnectionHost);
            this.panelConnectionDataTop.Dock = DockStyle.Fill;
            this.panelConnectionDataTop.Location = new Point(0, 0);
            this.panelConnectionDataTop.Margin = new Padding(0);
            this.panelConnectionDataTop.MinimumSize = new Size(300, 72);
            this.panelConnectionDataTop.Name = "panelConnectionDataTop";
            this.panelConnectionDataTop.Size = new Size(320, 211);
            this.panelConnectionDataTop.TabIndex = 0;
            // 
            // checkBoxConnectionLaserStop
            // 
            this.checkBoxConnectionLaserStop.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.checkBoxConnectionLaserStop.CheckAlign = ContentAlignment.MiddleRight;
            this.checkBoxConnectionLaserStop.Enabled = false;
            this.checkBoxConnectionLaserStop.Location = new Point(15, 70);
            this.checkBoxConnectionLaserStop.Name = "checkBoxConnectionLaserStop";
            this.checkBoxConnectionLaserStop.Size = new Size(295, 23);
            this.checkBoxConnectionLaserStop.TabIndex = 4;
            this.checkBoxConnectionLaserStop.Text = "Automatically stop Lasers when not in Use?";
            this.checkBoxConnectionLaserStop.TextAlign = ContentAlignment.MiddleRight;
            this.checkBoxConnectionLaserStop.UseVisualStyleBackColor = true;
            // 
            // textBoxConnectionLaserDelay
            // 
            this.textBoxConnectionLaserDelay.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxConnectionLaserDelay.Enabled = false;
            this.textBoxConnectionLaserDelay.Location = new Point(209, 99);
            this.textBoxConnectionLaserDelay.Name = "textBoxConnectionLaserDelay";
            this.textBoxConnectionLaserDelay.Size = new Size(101, 23);
            this.textBoxConnectionLaserDelay.TabIndex = 6;
            // 
            // labelConnectionLaserDelay
            // 
            this.labelConnectionLaserDelay.Enabled = false;
            this.labelConnectionLaserDelay.Location = new Point(3, 99);
            this.labelConnectionLaserDelay.Name = "labelConnectionLaserDelay";
            this.labelConnectionLaserDelay.Size = new Size(200, 23);
            this.labelConnectionLaserDelay.TabIndex = 5;
            this.labelConnectionLaserDelay.Text = "Startup Delay after restarting Lasers:";
            this.labelConnectionLaserDelay.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxConnectionPort
            // 
            this.textBoxConnectionPort.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxConnectionPort.Enabled = false;
            this.textBoxConnectionPort.Location = new Point(209, 41);
            this.textBoxConnectionPort.Name = "textBoxConnectionPort";
            this.textBoxConnectionPort.Size = new Size(101, 23);
            this.textBoxConnectionPort.TabIndex = 3;
            // 
            // textBoxConnectionHost
            // 
            this.textBoxConnectionHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxConnectionHost.Enabled = false;
            this.textBoxConnectionHost.Location = new Point(209, 12);
            this.textBoxConnectionHost.Name = "textBoxConnectionHost";
            this.textBoxConnectionHost.Size = new Size(101, 23);
            this.textBoxConnectionHost.TabIndex = 1;
            // 
            // labelConnectionPort
            // 
            this.labelConnectionPort.Enabled = false;
            this.labelConnectionPort.Location = new Point(3, 41);
            this.labelConnectionPort.Name = "labelConnectionPort";
            this.labelConnectionPort.Size = new Size(200, 23);
            this.labelConnectionPort.TabIndex = 2;
            this.labelConnectionPort.Text = "Port:";
            this.labelConnectionPort.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelConnectionHost
            // 
            this.labelConnectionHost.Enabled = false;
            this.labelConnectionHost.Location = new Point(3, 12);
            this.labelConnectionHost.Name = "labelConnectionHost";
            this.labelConnectionHost.Size = new Size(200, 23);
            this.labelConnectionHost.TabIndex = 0;
            this.labelConnectionHost.Text = "Host:";
            this.labelConnectionHost.TextAlign = ContentAlignment.MiddleRight;
            // 
            // panelConnectionDataButtons
            // 
            this.panelConnectionDataButtons.Controls.Add(this.buttonConnectionCancel);
            this.panelConnectionDataButtons.Controls.Add(this.buttonConnectionOK);
            this.panelConnectionDataButtons.Dock = DockStyle.Fill;
            this.panelConnectionDataButtons.Location = new Point(0, 211);
            this.panelConnectionDataButtons.Margin = new Padding(0);
            this.panelConnectionDataButtons.Name = "panelConnectionDataButtons";
            this.panelConnectionDataButtons.Size = new Size(320, 50);
            this.panelConnectionDataButtons.TabIndex = 1;
            // 
            // buttonConnectionCancel
            // 
            this.buttonConnectionCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.buttonConnectionCancel.Enabled = false;
            this.buttonConnectionCancel.Location = new Point(121, 15);
            this.buttonConnectionCancel.Name = "buttonConnectionCancel";
            this.buttonConnectionCancel.Size = new Size(110, 23);
            this.buttonConnectionCancel.TabIndex = 8;
            this.buttonConnectionCancel.Text = "Cancel";
            this.buttonConnectionCancel.UseVisualStyleBackColor = true;
            this.buttonConnectionCancel.Click += this.buttonConnectionCancel_Click;
            // 
            // buttonConnectionOK
            // 
            this.buttonConnectionOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.buttonConnectionOK.Enabled = false;
            this.buttonConnectionOK.Location = new Point(5, 15);
            this.buttonConnectionOK.Name = "buttonConnectionOK";
            this.buttonConnectionOK.Size = new Size(110, 23);
            this.buttonConnectionOK.TabIndex = 7;
            this.buttonConnectionOK.Text = "OK";
            this.buttonConnectionOK.UseVisualStyleBackColor = true;
            this.buttonConnectionOK.Click += this.buttonConnectionOK_Click;
            // 
            // labelVerticalDivider1
            // 
            this.labelVerticalDivider1.BorderStyle = BorderStyle.Fixed3D;
            this.labelVerticalDivider1.Dock = DockStyle.Fill;
            this.labelVerticalDivider1.Location = new Point(164, 4);
            this.labelVerticalDivider1.Margin = new Padding(4);
            this.labelVerticalDivider1.Name = "labelVerticalDivider1";
            this.labelVerticalDivider1.Size = new Size(1, 253);
            this.labelVerticalDivider1.TabIndex = 3;
            // 
            // labelVerticalDividier2
            // 
            this.labelVerticalDividier2.BorderStyle = BorderStyle.Fixed3D;
            this.labelVerticalDividier2.Dock = DockStyle.Fill;
            this.labelVerticalDividier2.Location = new Point(491, 4);
            this.labelVerticalDividier2.Margin = new Padding(4);
            this.labelVerticalDividier2.Name = "labelVerticalDividier2";
            this.labelVerticalDividier2.Size = new Size(1, 253);
            this.labelVerticalDividier2.TabIndex = 4;
            // 
            // ConnectionEditor
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new Size(844, 261);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.MaximizeBox = false;
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(860, 220);
            this.Name = "ConnectionEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Config - Edit Connection Settings";
            this.FormClosing += this.ConnectionEditor_FormClosing;
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelSelectConnection.ResumeLayout(false);
            this.panelConnectionSelector.ResumeLayout(false);
            this.panelConnectionSelectorButtons.ResumeLayout(false);
            this.tableLayoutPanelSelectSensor.ResumeLayout(false);
            this.panelSensorSelector.ResumeLayout(false);
            this.panelSensorSelectorButtons.ResumeLayout(false);
            this.panelSensorData.ResumeLayout(false);
            this.panelSensorData.PerformLayout();
            this.tableLayoutPanelCenter.ResumeLayout(false);
            this.panelConnectionDataTop.ResumeLayout(false);
            this.panelConnectionDataTop.PerformLayout();
            this.panelConnectionDataButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private TableLayoutPanel tableLayoutPanelSelectConnection;
        private TableLayoutPanel tableLayoutPanelCenter;
        private TableLayoutPanel tableLayoutPanelSelectSensor;
        private Panel panelConnectionSelector;
        private Panel panelConnectionSelectorButtons;
        private Panel panelConnectionDataTop;
        private Panel panelConnectionDataButtons;
        private Panel panelSensorSelector;
        private Panel panelSensorSelectorButtons;
        private Panel panelSensorData;
        private ListBox listBoxSelectConnection;
        private Button buttonConnectionRemove;
        private Button buttonConnectionEdit;
        private Button buttonConnectionAdd;
        private ListBox listBoxSelectSensor;
        private Button buttonSensorRemove;
        private Button buttonSensorEdit;
        private Button buttonSensorAdd;
        private Button buttonSensorCancel;
        private Button buttonSensorOK;
        private TextBox textBoxConnectionPort;
        private TextBox textBoxConnectionHost;
        private Label labelConnectionPort;
        private Label labelConnectionHost;
        private Button buttonConnectionCancel;
        private Button buttonConnectionOK;
        private Label labelSelectConnectionDivider;
        private Button buttonClose;
        private Label labelVerticalDivider1;
        private Label labelVerticalDividier2;
        private Label labelSensorDecimals;
        private TextBox textBoxSensorID;
        private Label labelSensorID;
        private TextBox textBoxConnectionLaserDelay;
        private Label labelConnectionLaserDelay;
        private CheckBox checkBoxConnectionLaserStop;
        private ComboBox comboBoxSensorDecimalPlaces;
    }
}