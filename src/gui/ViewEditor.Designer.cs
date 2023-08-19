namespace ILInspect {
    partial class ViewEditor {
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
            this.tableLayoutPanelSelectView = new TableLayoutPanel();
            this.panelViewSelector = new Panel();
            this.listBoxSelectView = new ListBox();
            this.panelViewSelectorButtons = new Panel();
            this.labelSelectViewDivider = new Label();
            this.buttonClose = new Button();
            this.buttonViewRemove = new Button();
            this.buttonViewEdit = new Button();
            this.buttonViewAdd = new Button();
            this.tableLayoutPanelSelectSensor = new TableLayoutPanel();
            this.panelSensorSelector = new Panel();
            this.listBoxSelectSensor = new ListBox();
            this.panelSensorSelectorButtons = new Panel();
            this.buttonSensorRemove = new Button();
            this.buttonSensorEdit = new Button();
            this.buttonSensorAdd = new Button();
            this.panelSensorData = new Panel();
            this.labelSensorDivider = new Label();
            this.textBoxSensorAnalogLower = new TextBox();
            this.labelSensorAnalogLower = new Label();
            this.textBoxSensorAnalogUpper = new TextBox();
            this.labelSensorAnalogUpper = new Label();
            this.textBoxSensorLow = new TextBox();
            this.labelSensorLow = new Label();
            this.textBoxSensorHigh = new TextBox();
            this.labelSensorHigh = new Label();
            this.textBoxSensorShift = new TextBox();
            this.labelSensorShift = new Label();
            this.comboBoxSensorBank = new ComboBox();
            this.labelSensorBank = new Label();
            this.textBoxSensorPosY = new TextBox();
            this.labelSensorPosY = new Label();
            this.textBoxSensorPosX = new TextBox();
            this.labelSensorPosX = new Label();
            this.comboBoxSensorID = new ComboBox();
            this.labelSensorID = new Label();
            this.buttonSensorCancel = new Button();
            this.buttonSensorOK = new Button();
            this.tableLayoutPanelCenter = new TableLayoutPanel();
            this.panelViewDataTop = new Panel();
            this.buttonViewLocateImage = new Button();
            this.textBoxViewImageLocation = new TextBox();
            this.textBoxViewName = new TextBox();
            this.labelViewImageLocation = new Label();
            this.labelViewName = new Label();
            this.panelViewDataPreviewLabels = new Panel();
            this.buttonLoadPreview = new Button();
            this.labelViewPreview = new Label();
            this.panelViewDataButtons = new Panel();
            this.buttonViewCancel = new Button();
            this.buttonViewOK = new Button();
            this.panelPreviewImage = new Panel();
            this.pictureBoxSensorLocation = new PictureBox();
            this.pictureBoxPreview = new PictureBox();
            this.labelVerticalDivider1 = new Label();
            this.labelVerticalDividier2 = new Label();
            this.openFileDialogLocateImage = new OpenFileDialog();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelSelectView.SuspendLayout();
            this.panelViewSelector.SuspendLayout();
            this.panelViewSelectorButtons.SuspendLayout();
            this.tableLayoutPanelSelectSensor.SuspendLayout();
            this.panelSensorSelector.SuspendLayout();
            this.panelSensorSelectorButtons.SuspendLayout();
            this.panelSensorData.SuspendLayout();
            this.tableLayoutPanelCenter.SuspendLayout();
            this.panelViewDataTop.SuspendLayout();
            this.panelViewDataPreviewLabels.SuspendLayout();
            this.panelViewDataButtons.SuspendLayout();
            this.panelPreviewImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxSensorLocation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxPreview).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 7F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 7F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 370F));
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelSelectView, 0, 0);
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
            this.tableLayoutPanelMain.Size = new Size(984, 411);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // tableLayoutPanelSelectView
            // 
            this.tableLayoutPanelSelectView.ColumnCount = 1;
            this.tableLayoutPanelSelectView.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectView.Controls.Add(this.panelViewSelector, 0, 0);
            this.tableLayoutPanelSelectView.Controls.Add(this.panelViewSelectorButtons, 0, 1);
            this.tableLayoutPanelSelectView.Dock = DockStyle.Fill;
            this.tableLayoutPanelSelectView.Location = new Point(0, 0);
            this.tableLayoutPanelSelectView.Margin = new Padding(0);
            this.tableLayoutPanelSelectView.MinimumSize = new Size(120, 300);
            this.tableLayoutPanelSelectView.Name = "tableLayoutPanelSelectView";
            this.tableLayoutPanelSelectView.RowCount = 2;
            this.tableLayoutPanelSelectView.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectView.RowStyles.Add(new RowStyle(SizeType.Absolute, 138F));
            this.tableLayoutPanelSelectView.Size = new Size(120, 411);
            this.tableLayoutPanelSelectView.TabIndex = 0;
            // 
            // panelViewSelector
            // 
            this.panelViewSelector.Controls.Add(this.listBoxSelectView);
            this.panelViewSelector.Dock = DockStyle.Fill;
            this.panelViewSelector.Location = new Point(3, 3);
            this.panelViewSelector.Name = "panelViewSelector";
            this.panelViewSelector.Size = new Size(114, 267);
            this.panelViewSelector.TabIndex = 0;
            // 
            // listBoxSelectView
            // 
            this.listBoxSelectView.Dock = DockStyle.Fill;
            this.listBoxSelectView.FormattingEnabled = true;
            this.listBoxSelectView.ItemHeight = 15;
            this.listBoxSelectView.Location = new Point(0, 0);
            this.listBoxSelectView.Name = "listBoxSelectView";
            this.listBoxSelectView.ScrollAlwaysVisible = true;
            this.listBoxSelectView.Size = new Size(114, 267);
            this.listBoxSelectView.TabIndex = 0;
            this.listBoxSelectView.SelectedIndexChanged += this.listBoxSelectView_SelectedIndexChanged;
            // 
            // panelViewSelectorButtons
            // 
            this.panelViewSelectorButtons.Controls.Add(this.labelSelectViewDivider);
            this.panelViewSelectorButtons.Controls.Add(this.buttonClose);
            this.panelViewSelectorButtons.Controls.Add(this.buttonViewRemove);
            this.panelViewSelectorButtons.Controls.Add(this.buttonViewEdit);
            this.panelViewSelectorButtons.Controls.Add(this.buttonViewAdd);
            this.panelViewSelectorButtons.Dock = DockStyle.Fill;
            this.panelViewSelectorButtons.Location = new Point(0, 273);
            this.panelViewSelectorButtons.Margin = new Padding(0);
            this.panelViewSelectorButtons.Name = "panelViewSelectorButtons";
            this.panelViewSelectorButtons.Size = new Size(120, 138);
            this.panelViewSelectorButtons.TabIndex = 1;
            // 
            // labelSelectViewDivider
            // 
            this.labelSelectViewDivider.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.labelSelectViewDivider.BorderStyle = BorderStyle.Fixed3D;
            this.labelSelectViewDivider.Location = new Point(6, 96);
            this.labelSelectViewDivider.Name = "labelSelectViewDivider";
            this.labelSelectViewDivider.Size = new Size(108, 2);
            this.labelSelectViewDivider.TabIndex = 4;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonClose.Location = new Point(12, 103);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(92, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += this.buttonClose_Click;
            // 
            // buttonViewRemove
            // 
            this.buttonViewRemove.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonViewRemove.Enabled = false;
            this.buttonViewRemove.Location = new Point(12, 66);
            this.buttonViewRemove.Name = "buttonViewRemove";
            this.buttonViewRemove.Size = new Size(92, 23);
            this.buttonViewRemove.TabIndex = 3;
            this.buttonViewRemove.Text = "Remove View";
            this.buttonViewRemove.UseVisualStyleBackColor = true;
            this.buttonViewRemove.Click += this.buttonViewRemove_Click;
            // 
            // buttonViewEdit
            // 
            this.buttonViewEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonViewEdit.Enabled = false;
            this.buttonViewEdit.Location = new Point(12, 37);
            this.buttonViewEdit.Name = "buttonViewEdit";
            this.buttonViewEdit.Size = new Size(92, 23);
            this.buttonViewEdit.TabIndex = 2;
            this.buttonViewEdit.Text = "Edit View";
            this.buttonViewEdit.UseVisualStyleBackColor = true;
            this.buttonViewEdit.Click += this.buttonViewEdit_Click;
            // 
            // buttonViewAdd
            // 
            this.buttonViewAdd.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonViewAdd.Location = new Point(12, 8);
            this.buttonViewAdd.Name = "buttonViewAdd";
            this.buttonViewAdd.Size = new Size(92, 23);
            this.buttonViewAdd.TabIndex = 1;
            this.buttonViewAdd.Text = "Add View";
            this.buttonViewAdd.UseVisualStyleBackColor = true;
            this.buttonViewAdd.Click += this.buttonViewAdd_Click;
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
            this.tableLayoutPanelSelectSensor.Location = new Point(614, 0);
            this.tableLayoutPanelSelectSensor.Margin = new Padding(0);
            this.tableLayoutPanelSelectSensor.MinimumSize = new Size(300, 300);
            this.tableLayoutPanelSelectSensor.Name = "tableLayoutPanelSelectSensor";
            this.tableLayoutPanelSelectSensor.RowCount = 2;
            this.tableLayoutPanelSelectSensor.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelSelectSensor.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            this.tableLayoutPanelSelectSensor.Size = new Size(370, 411);
            this.tableLayoutPanelSelectSensor.TabIndex = 2;
            // 
            // panelSensorSelector
            // 
            this.panelSensorSelector.Controls.Add(this.listBoxSelectSensor);
            this.panelSensorSelector.Dock = DockStyle.Fill;
            this.panelSensorSelector.Location = new Point(3, 3);
            this.panelSensorSelector.Name = "panelSensorSelector";
            this.panelSensorSelector.Size = new Size(124, 305);
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
            this.listBoxSelectSensor.Size = new Size(124, 305);
            this.listBoxSelectSensor.TabIndex = 0;
            this.listBoxSelectSensor.SelectedIndexChanged += this.listBoxSelectSensor_SelectedIndexChanged;
            // 
            // panelSensorSelectorButtons
            // 
            this.panelSensorSelectorButtons.Controls.Add(this.buttonSensorRemove);
            this.panelSensorSelectorButtons.Controls.Add(this.buttonSensorEdit);
            this.panelSensorSelectorButtons.Controls.Add(this.buttonSensorAdd);
            this.panelSensorSelectorButtons.Dock = DockStyle.Fill;
            this.panelSensorSelectorButtons.Location = new Point(0, 311);
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
            this.panelSensorData.Controls.Add(this.labelSensorDivider);
            this.panelSensorData.Controls.Add(this.textBoxSensorAnalogLower);
            this.panelSensorData.Controls.Add(this.labelSensorAnalogLower);
            this.panelSensorData.Controls.Add(this.textBoxSensorAnalogUpper);
            this.panelSensorData.Controls.Add(this.labelSensorAnalogUpper);
            this.panelSensorData.Controls.Add(this.textBoxSensorLow);
            this.panelSensorData.Controls.Add(this.labelSensorLow);
            this.panelSensorData.Controls.Add(this.textBoxSensorHigh);
            this.panelSensorData.Controls.Add(this.labelSensorHigh);
            this.panelSensorData.Controls.Add(this.textBoxSensorShift);
            this.panelSensorData.Controls.Add(this.labelSensorShift);
            this.panelSensorData.Controls.Add(this.comboBoxSensorBank);
            this.panelSensorData.Controls.Add(this.labelSensorBank);
            this.panelSensorData.Controls.Add(this.textBoxSensorPosY);
            this.panelSensorData.Controls.Add(this.labelSensorPosY);
            this.panelSensorData.Controls.Add(this.textBoxSensorPosX);
            this.panelSensorData.Controls.Add(this.labelSensorPosX);
            this.panelSensorData.Controls.Add(this.comboBoxSensorID);
            this.panelSensorData.Controls.Add(this.labelSensorID);
            this.panelSensorData.Controls.Add(this.buttonSensorCancel);
            this.panelSensorData.Controls.Add(this.buttonSensorOK);
            this.panelSensorData.Dock = DockStyle.Fill;
            this.panelSensorData.Location = new Point(130, 0);
            this.panelSensorData.Margin = new Padding(0);
            this.panelSensorData.MinimumSize = new Size(240, 330);
            this.panelSensorData.Name = "panelSensorData";
            this.tableLayoutPanelSelectSensor.SetRowSpan(this.panelSensorData, 2);
            this.panelSensorData.Size = new Size(240, 411);
            this.panelSensorData.TabIndex = 2;
            // 
            // labelSensorDivider
            // 
            this.labelSensorDivider.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.labelSensorDivider.BorderStyle = BorderStyle.Fixed3D;
            this.labelSensorDivider.Location = new Point(3, 102);
            this.labelSensorDivider.Margin = new Padding(6);
            this.labelSensorDivider.Name = "labelSensorDivider";
            this.labelSensorDivider.Size = new Size(229, 2);
            this.labelSensorDivider.TabIndex = 6;
            // 
            // textBoxSensorAnalogLower
            // 
            this.textBoxSensorAnalogLower.Enabled = false;
            this.textBoxSensorAnalogLower.Location = new Point(132, 258);
            this.textBoxSensorAnalogLower.Name = "textBoxSensorAnalogLower";
            this.textBoxSensorAnalogLower.Size = new Size(100, 23);
            this.textBoxSensorAnalogLower.TabIndex = 18;
            // 
            // labelSensorAnalogLower
            // 
            this.labelSensorAnalogLower.Enabled = false;
            this.labelSensorAnalogLower.Location = new Point(3, 258);
            this.labelSensorAnalogLower.Name = "labelSensorAnalogLower";
            this.labelSensorAnalogLower.Size = new Size(123, 23);
            this.labelSensorAnalogLower.TabIndex = 17;
            this.labelSensorAnalogLower.Text = "Analog Lower Bound:";
            this.labelSensorAnalogLower.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorAnalogUpper
            // 
            this.textBoxSensorAnalogUpper.Enabled = false;
            this.textBoxSensorAnalogUpper.Location = new Point(132, 229);
            this.textBoxSensorAnalogUpper.Name = "textBoxSensorAnalogUpper";
            this.textBoxSensorAnalogUpper.Size = new Size(100, 23);
            this.textBoxSensorAnalogUpper.TabIndex = 16;
            // 
            // labelSensorAnalogUpper
            // 
            this.labelSensorAnalogUpper.Enabled = false;
            this.labelSensorAnalogUpper.Location = new Point(3, 229);
            this.labelSensorAnalogUpper.Name = "labelSensorAnalogUpper";
            this.labelSensorAnalogUpper.Size = new Size(123, 23);
            this.labelSensorAnalogUpper.TabIndex = 15;
            this.labelSensorAnalogUpper.Text = "Analog Upper Bound:";
            this.labelSensorAnalogUpper.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorLow
            // 
            this.textBoxSensorLow.Enabled = false;
            this.textBoxSensorLow.Location = new Point(132, 200);
            this.textBoxSensorLow.Name = "textBoxSensorLow";
            this.textBoxSensorLow.Size = new Size(100, 23);
            this.textBoxSensorLow.TabIndex = 14;
            // 
            // labelSensorLow
            // 
            this.labelSensorLow.Enabled = false;
            this.labelSensorLow.Location = new Point(3, 200);
            this.labelSensorLow.Name = "labelSensorLow";
            this.labelSensorLow.Size = new Size(123, 23);
            this.labelSensorLow.TabIndex = 13;
            this.labelSensorLow.Text = "LOW Threshold:";
            this.labelSensorLow.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorHigh
            // 
            this.textBoxSensorHigh.Enabled = false;
            this.textBoxSensorHigh.Location = new Point(132, 171);
            this.textBoxSensorHigh.Name = "textBoxSensorHigh";
            this.textBoxSensorHigh.Size = new Size(100, 23);
            this.textBoxSensorHigh.TabIndex = 12;
            // 
            // labelSensorHigh
            // 
            this.labelSensorHigh.Enabled = false;
            this.labelSensorHigh.Location = new Point(3, 171);
            this.labelSensorHigh.Name = "labelSensorHigh";
            this.labelSensorHigh.Size = new Size(123, 23);
            this.labelSensorHigh.TabIndex = 11;
            this.labelSensorHigh.Text = "HIGH Threshold:";
            this.labelSensorHigh.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorShift
            // 
            this.textBoxSensorShift.Enabled = false;
            this.textBoxSensorShift.Location = new Point(132, 142);
            this.textBoxSensorShift.Name = "textBoxSensorShift";
            this.textBoxSensorShift.Size = new Size(100, 23);
            this.textBoxSensorShift.TabIndex = 10;
            // 
            // labelSensorShift
            // 
            this.labelSensorShift.Enabled = false;
            this.labelSensorShift.Location = new Point(3, 142);
            this.labelSensorShift.Name = "labelSensorShift";
            this.labelSensorShift.Size = new Size(123, 23);
            this.labelSensorShift.TabIndex = 9;
            this.labelSensorShift.Text = "Zero Shift Amount:";
            this.labelSensorShift.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboBoxSensorBank
            // 
            this.comboBoxSensorBank.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxSensorBank.Enabled = false;
            this.comboBoxSensorBank.Items.AddRange(new object[] { "", "0", "1", "2", "3" });
            this.comboBoxSensorBank.Location = new Point(132, 113);
            this.comboBoxSensorBank.Name = "comboBoxSensorBank";
            this.comboBoxSensorBank.Size = new Size(100, 23);
            this.comboBoxSensorBank.TabIndex = 8;
            // 
            // labelSensorBank
            // 
            this.labelSensorBank.Enabled = false;
            this.labelSensorBank.Location = new Point(3, 113);
            this.labelSensorBank.Name = "labelSensorBank";
            this.labelSensorBank.Size = new Size(123, 23);
            this.labelSensorBank.TabIndex = 7;
            this.labelSensorBank.Text = "Bank:";
            this.labelSensorBank.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorPosY
            // 
            this.textBoxSensorPosY.Enabled = false;
            this.textBoxSensorPosY.Location = new Point(132, 70);
            this.textBoxSensorPosY.Name = "textBoxSensorPosY";
            this.textBoxSensorPosY.Size = new Size(100, 23);
            this.textBoxSensorPosY.TabIndex = 5;
            this.textBoxSensorPosY.TextChanged += this.textBoxSensorPosition_TextChanged;
            // 
            // labelSensorPosY
            // 
            this.labelSensorPosY.Enabled = false;
            this.labelSensorPosY.Location = new Point(3, 70);
            this.labelSensorPosY.Name = "labelSensorPosY";
            this.labelSensorPosY.Size = new Size(123, 23);
            this.labelSensorPosY.TabIndex = 4;
            this.labelSensorPosY.Text = "Position Y (%):";
            this.labelSensorPosY.TextAlign = ContentAlignment.MiddleRight;
            // 
            // textBoxSensorPosX
            // 
            this.textBoxSensorPosX.Enabled = false;
            this.textBoxSensorPosX.Location = new Point(132, 41);
            this.textBoxSensorPosX.Name = "textBoxSensorPosX";
            this.textBoxSensorPosX.Size = new Size(100, 23);
            this.textBoxSensorPosX.TabIndex = 3;
            this.textBoxSensorPosX.TextChanged += this.textBoxSensorPosition_TextChanged;
            // 
            // labelSensorPosX
            // 
            this.labelSensorPosX.Enabled = false;
            this.labelSensorPosX.Location = new Point(3, 41);
            this.labelSensorPosX.Name = "labelSensorPosX";
            this.labelSensorPosX.Size = new Size(123, 23);
            this.labelSensorPosX.TabIndex = 2;
            this.labelSensorPosX.Text = "Position X (%):";
            this.labelSensorPosX.TextAlign = ContentAlignment.MiddleRight;
            // 
            // comboBoxSensorID
            // 
            this.comboBoxSensorID.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxSensorID.Enabled = false;
            this.comboBoxSensorID.Location = new Point(132, 12);
            this.comboBoxSensorID.Name = "comboBoxSensorID";
            this.comboBoxSensorID.Size = new Size(100, 23);
            this.comboBoxSensorID.TabIndex = 1;
            // 
            // labelSensorID
            // 
            this.labelSensorID.Enabled = false;
            this.labelSensorID.Location = new Point(3, 12);
            this.labelSensorID.Name = "labelSensorID";
            this.labelSensorID.Size = new Size(123, 23);
            this.labelSensorID.TabIndex = 0;
            this.labelSensorID.Text = "ID:";
            this.labelSensorID.TextAlign = ContentAlignment.MiddleRight;
            // 
            // buttonSensorCancel
            // 
            this.buttonSensorCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonSensorCancel.Enabled = false;
            this.buttonSensorCancel.Location = new Point(122, 376);
            this.buttonSensorCancel.Name = "buttonSensorCancel";
            this.buttonSensorCancel.Size = new Size(110, 23);
            this.buttonSensorCancel.TabIndex = 20;
            this.buttonSensorCancel.Text = "Cancel";
            this.buttonSensorCancel.UseVisualStyleBackColor = true;
            this.buttonSensorCancel.Click += this.buttonSensorCancel_Click;
            // 
            // buttonSensorOK
            // 
            this.buttonSensorOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorOK.Location = new Point(6, 376);
            this.buttonSensorOK.Name = "buttonSensorOK";
            this.buttonSensorOK.Size = new Size(110, 23);
            this.buttonSensorOK.TabIndex = 19;
            this.buttonSensorOK.Text = "OK";
            this.buttonSensorOK.UseVisualStyleBackColor = true;
            this.buttonSensorOK.Click += this.buttonSensorOK_Click;
            // 
            // tableLayoutPanelCenter
            // 
            this.tableLayoutPanelCenter.ColumnCount = 2;
            this.tableLayoutPanelCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            this.tableLayoutPanelCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelCenter.Controls.Add(this.panelViewDataTop, 0, 0);
            this.tableLayoutPanelCenter.Controls.Add(this.panelViewDataPreviewLabels, 0, 1);
            this.tableLayoutPanelCenter.Controls.Add(this.panelViewDataButtons, 0, 2);
            this.tableLayoutPanelCenter.Controls.Add(this.panelPreviewImage, 1, 1);
            this.tableLayoutPanelCenter.Dock = DockStyle.Fill;
            this.tableLayoutPanelCenter.Location = new Point(127, 0);
            this.tableLayoutPanelCenter.Margin = new Padding(0);
            this.tableLayoutPanelCenter.Name = "tableLayoutPanelCenter";
            this.tableLayoutPanelCenter.RowCount = 3;
            this.tableLayoutPanelCenter.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            this.tableLayoutPanelCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelCenter.RowStyles.Add(new RowStyle(SizeType.Absolute, 72F));
            this.tableLayoutPanelCenter.Size = new Size(480, 411);
            this.tableLayoutPanelCenter.TabIndex = 1;
            // 
            // panelViewDataTop
            // 
            this.tableLayoutPanelCenter.SetColumnSpan(this.panelViewDataTop, 2);
            this.panelViewDataTop.Controls.Add(this.buttonViewLocateImage);
            this.panelViewDataTop.Controls.Add(this.textBoxViewImageLocation);
            this.panelViewDataTop.Controls.Add(this.textBoxViewName);
            this.panelViewDataTop.Controls.Add(this.labelViewImageLocation);
            this.panelViewDataTop.Controls.Add(this.labelViewName);
            this.panelViewDataTop.Dock = DockStyle.Fill;
            this.panelViewDataTop.Location = new Point(0, 0);
            this.panelViewDataTop.Margin = new Padding(0);
            this.panelViewDataTop.MinimumSize = new Size(300, 72);
            this.panelViewDataTop.Name = "panelViewDataTop";
            this.panelViewDataTop.Size = new Size(480, 72);
            this.panelViewDataTop.TabIndex = 0;
            // 
            // buttonViewLocateImage
            // 
            this.buttonViewLocateImage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.buttonViewLocateImage.Enabled = false;
            this.buttonViewLocateImage.Location = new Point(402, 41);
            this.buttonViewLocateImage.Name = "buttonViewLocateImage";
            this.buttonViewLocateImage.Size = new Size(75, 23);
            this.buttonViewLocateImage.TabIndex = 4;
            this.buttonViewLocateImage.Text = "Browse...";
            this.buttonViewLocateImage.UseVisualStyleBackColor = true;
            this.buttonViewLocateImage.Click += this.buttonViewLocateImage_Click;
            // 
            // textBoxViewImageLocation
            // 
            this.textBoxViewImageLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxViewImageLocation.Enabled = false;
            this.textBoxViewImageLocation.Location = new Point(119, 41);
            this.textBoxViewImageLocation.Name = "textBoxViewImageLocation";
            this.textBoxViewImageLocation.Size = new Size(277, 23);
            this.textBoxViewImageLocation.TabIndex = 3;
            // 
            // textBoxViewName
            // 
            this.textBoxViewName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxViewName.Enabled = false;
            this.textBoxViewName.Location = new Point(119, 12);
            this.textBoxViewName.Name = "textBoxViewName";
            this.textBoxViewName.Size = new Size(358, 23);
            this.textBoxViewName.TabIndex = 1;
            // 
            // labelViewImageLocation
            // 
            this.labelViewImageLocation.Enabled = false;
            this.labelViewImageLocation.Location = new Point(3, 44);
            this.labelViewImageLocation.Name = "labelViewImageLocation";
            this.labelViewImageLocation.Size = new Size(107, 20);
            this.labelViewImageLocation.TabIndex = 2;
            this.labelViewImageLocation.Text = "Image Location:";
            this.labelViewImageLocation.TextAlign = ContentAlignment.TopRight;
            // 
            // labelViewName
            // 
            this.labelViewName.Enabled = false;
            this.labelViewName.Location = new Point(3, 15);
            this.labelViewName.Name = "labelViewName";
            this.labelViewName.Size = new Size(107, 20);
            this.labelViewName.TabIndex = 0;
            this.labelViewName.Text = "Name:";
            this.labelViewName.TextAlign = ContentAlignment.TopRight;
            // 
            // panelViewDataPreviewLabels
            // 
            this.panelViewDataPreviewLabels.Controls.Add(this.buttonLoadPreview);
            this.panelViewDataPreviewLabels.Controls.Add(this.labelViewPreview);
            this.panelViewDataPreviewLabels.Dock = DockStyle.Fill;
            this.panelViewDataPreviewLabels.Location = new Point(0, 77);
            this.panelViewDataPreviewLabels.Margin = new Padding(0, 5, 0, 0);
            this.panelViewDataPreviewLabels.Name = "panelViewDataPreviewLabels";
            this.panelViewDataPreviewLabels.Size = new Size(120, 262);
            this.panelViewDataPreviewLabels.TabIndex = 1;
            // 
            // buttonLoadPreview
            // 
            this.buttonLoadPreview.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonLoadPreview.Enabled = false;
            this.buttonLoadPreview.Location = new Point(12, 18);
            this.buttonLoadPreview.Name = "buttonLoadPreview";
            this.buttonLoadPreview.Size = new Size(95, 23);
            this.buttonLoadPreview.TabIndex = 6;
            this.buttonLoadPreview.Text = "Load Preview";
            this.buttonLoadPreview.UseVisualStyleBackColor = true;
            this.buttonLoadPreview.Click += this.buttonLoadPreview_Click;
            // 
            // labelViewPreview
            // 
            this.labelViewPreview.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.labelViewPreview.Enabled = false;
            this.labelViewPreview.Location = new Point(3, 0);
            this.labelViewPreview.Name = "labelViewPreview";
            this.labelViewPreview.Size = new Size(107, 23);
            this.labelViewPreview.TabIndex = 5;
            this.labelViewPreview.Text = "Preview:";
            this.labelViewPreview.TextAlign = ContentAlignment.TopRight;
            // 
            // panelViewDataButtons
            // 
            this.panelViewDataButtons.Controls.Add(this.buttonViewCancel);
            this.panelViewDataButtons.Controls.Add(this.buttonViewOK);
            this.panelViewDataButtons.Dock = DockStyle.Fill;
            this.panelViewDataButtons.Location = new Point(0, 339);
            this.panelViewDataButtons.Margin = new Padding(0);
            this.panelViewDataButtons.MinimumSize = new Size(120, 72);
            this.panelViewDataButtons.Name = "panelViewDataButtons";
            this.panelViewDataButtons.Size = new Size(120, 72);
            this.panelViewDataButtons.TabIndex = 2;
            // 
            // buttonViewCancel
            // 
            this.buttonViewCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonViewCancel.Enabled = false;
            this.buttonViewCancel.Location = new Point(12, 37);
            this.buttonViewCancel.Name = "buttonViewCancel";
            this.buttonViewCancel.Size = new Size(95, 23);
            this.buttonViewCancel.TabIndex = 8;
            this.buttonViewCancel.Text = "Cancel";
            this.buttonViewCancel.UseVisualStyleBackColor = true;
            this.buttonViewCancel.Click += this.buttonViewCancel_Click;
            // 
            // buttonViewOK
            // 
            this.buttonViewOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonViewOK.Enabled = false;
            this.buttonViewOK.Location = new Point(12, 8);
            this.buttonViewOK.Name = "buttonViewOK";
            this.buttonViewOK.Size = new Size(95, 23);
            this.buttonViewOK.TabIndex = 7;
            this.buttonViewOK.Text = "OK";
            this.buttonViewOK.UseVisualStyleBackColor = true;
            this.buttonViewOK.Click += this.buttonViewOK_Click;
            // 
            // panelPreviewImage
            // 
            this.panelPreviewImage.Controls.Add(this.pictureBoxSensorLocation);
            this.panelPreviewImage.Controls.Add(this.pictureBoxPreview);
            this.panelPreviewImage.Dock = DockStyle.Fill;
            this.panelPreviewImage.Location = new Point(123, 75);
            this.panelPreviewImage.Name = "panelPreviewImage";
            this.tableLayoutPanelCenter.SetRowSpan(this.panelPreviewImage, 2);
            this.panelPreviewImage.Size = new Size(354, 333);
            this.panelPreviewImage.TabIndex = 3;
            // 
            // pictureBoxSensorLocation
            // 
            this.pictureBoxSensorLocation.BackColor = Color.Transparent;
            this.pictureBoxSensorLocation.Image = Properties.Resources.Cross;
            this.pictureBoxSensorLocation.Location = new Point(0, 0);
            this.pictureBoxSensorLocation.Name = "pictureBoxSensorLocation";
            this.pictureBoxSensorLocation.Size = new Size(40, 40);
            this.pictureBoxSensorLocation.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxSensorLocation.TabIndex = 1;
            this.pictureBoxSensorLocation.TabStop = false;
            this.pictureBoxSensorLocation.Visible = false;
            this.pictureBoxSensorLocation.MouseDown += this.pictureBoxSensorLocation_MouseDown;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pictureBoxPreview.BackColor = Color.Gray;
            this.pictureBoxPreview.Enabled = false;
            this.pictureBoxPreview.Location = new Point(0, 0);
            this.pictureBoxPreview.Margin = new Padding(0);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new Size(354, 333);
            this.pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.SizeChanged += this.pictureBoxPreview_SizeChanged;
            this.pictureBoxPreview.MouseDown += this.pictureBoxPreview_MouseDown;
            // 
            // labelVerticalDivider1
            // 
            this.labelVerticalDivider1.BorderStyle = BorderStyle.Fixed3D;
            this.labelVerticalDivider1.Dock = DockStyle.Fill;
            this.labelVerticalDivider1.Location = new Point(124, 4);
            this.labelVerticalDivider1.Margin = new Padding(4);
            this.labelVerticalDivider1.Name = "labelVerticalDivider1";
            this.labelVerticalDivider1.Size = new Size(1, 403);
            this.labelVerticalDivider1.TabIndex = 3;
            // 
            // labelVerticalDividier2
            // 
            this.labelVerticalDividier2.BorderStyle = BorderStyle.Fixed3D;
            this.labelVerticalDividier2.Dock = DockStyle.Fill;
            this.labelVerticalDividier2.Location = new Point(611, 4);
            this.labelVerticalDividier2.Margin = new Padding(4);
            this.labelVerticalDividier2.Name = "labelVerticalDividier2";
            this.labelVerticalDividier2.Size = new Size(1, 403);
            this.labelVerticalDividier2.TabIndex = 4;
            // 
            // openFileDialogLocateImage
            // 
            this.openFileDialogLocateImage.Filter = "Image Files(*.BMP;*.JPG;*:JPEG;*.GIF;*.PNG;*.TIFF)|*.BMP;*.JPG;*:JPEG;*.GIF;*.PNG;*.TIFF|All files (*.*)|*.*";
            // 
            // ViewEditor
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new Size(984, 411);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(820, 370);
            this.Name = "ViewEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Config - Edit View Settings";
            this.FormClosing += this.ViewEditor_FormClosing;
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelSelectView.ResumeLayout(false);
            this.panelViewSelector.ResumeLayout(false);
            this.panelViewSelectorButtons.ResumeLayout(false);
            this.tableLayoutPanelSelectSensor.ResumeLayout(false);
            this.panelSensorSelector.ResumeLayout(false);
            this.panelSensorSelectorButtons.ResumeLayout(false);
            this.panelSensorData.ResumeLayout(false);
            this.panelSensorData.PerformLayout();
            this.tableLayoutPanelCenter.ResumeLayout(false);
            this.panelViewDataTop.ResumeLayout(false);
            this.panelViewDataTop.PerformLayout();
            this.panelViewDataPreviewLabels.ResumeLayout(false);
            this.panelViewDataButtons.ResumeLayout(false);
            this.panelPreviewImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxSensorLocation).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxPreview).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private TableLayoutPanel tableLayoutPanelSelectView;
        private TableLayoutPanel tableLayoutPanelCenter;
        private TableLayoutPanel tableLayoutPanelSelectSensor;
        private Panel panelViewSelector;
        private Panel panelViewSelectorButtons;
        private Panel panelViewDataTop;
        private Panel panelViewDataPreviewLabels;
        private Panel panelViewDataButtons;
        private Panel panelPreviewImage;
        private Panel panelSensorSelector;
        private Panel panelSensorSelectorButtons;
        private Panel panelSensorData;
        private ListBox listBoxSelectView;
        private Button buttonViewRemove;
        private Button buttonViewEdit;
        private Button buttonViewAdd;
        private ListBox listBoxSelectSensor;
        private Button buttonSensorRemove;
        private Button buttonSensorEdit;
        private Button buttonSensorAdd;
        private PictureBox pictureBoxPreview;
        private Button buttonSensorCancel;
        private Button buttonSensorOK;
        private Button buttonViewLocateImage;
        private TextBox textBoxViewImageLocation;
        private TextBox textBoxViewName;
        private Label labelViewImageLocation;
        private Label labelViewName;
        private Button buttonLoadPreview;
        private Label labelViewPreview;
        private Button buttonViewCancel;
        private Button buttonViewOK;
        private Label labelSelectViewDivider;
        private Button buttonClose;
        private Label labelVerticalDivider1;
        private Label labelVerticalDividier2;
        private Label labelSensorDivider;
        private TextBox textBoxSensorAnalogLower;
        private Label labelSensorAnalogLower;
        private TextBox textBoxSensorAnalogUpper;
        private Label labelSensorAnalogUpper;
        private TextBox textBoxSensorLow;
        private Label labelSensorLow;
        private TextBox textBoxSensorHigh;
        private Label labelSensorHigh;
        private TextBox textBoxSensorShift;
        private Label labelSensorShift;
        private ComboBox comboBoxSensorBank;
        private Label labelSensorBank;
        private TextBox textBoxSensorPosY;
        private Label labelSensorPosY;
        private TextBox textBoxSensorPosX;
        private Label labelSensorPosX;
        private ComboBox comboBoxSensorID;
        private Label labelSensorID;
        private OpenFileDialog openFileDialogLocateImage;
        private PictureBox pictureBoxSensorLocation;
    }
}