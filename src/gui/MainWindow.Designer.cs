namespace ILInspect {
    partial class MainWindow {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.buttonMeasure = new Button();
            this.backgroundWorkerInitSensors = new System.ComponentModel.BackgroundWorker();
            this.groupBox_Sidebar = new GroupBox();
            this.splitContainerMessages = new SplitContainer();
            this.panel1 = new Panel();
            this.labelMessagesCollapse = new LinkLabel();
            this.textBoxMessages = new TextBox();
            this.textBoxUnit = new TextBox();
            this.labelStatus = new Label();
            this.labelStatusLabel = new Label();
            this.labelUnit = new Label();
            this.labelView = new Label();
            this.comboBoxView = new ComboBox();
            this.buttonRemoveFilter = new Button();
            this.buttonFilter = new Button();
            this.buttonInit = new Button();
            this.splitContainerMainView = new SplitContainer();
            this.pictureBoxViewImage = new PictureBox();
            this.dataGridViewDB = new DataGridView();
            this.tableLayoutPanelMain = new TableLayoutPanel();
            this.databaseBindingSource = new BindingSource(this.components);
            this.backgroundWorkerMeasure = new System.ComponentModel.BackgroundWorker();
            this.menuStripMain = new MenuStrip();
            this.toolStripMenuItemFile = new ToolStripMenuItem();
            this.contextMenuFile = new ContextMenuStrip(this.components);
            this.toolStripMenuItemFileNewConfig = new ToolStripMenuItem();
            this.toolStripMenuItemFileLoadConfig = new ToolStripMenuItem();
            this.toolStripMenuItemFileSaveConfig = new ToolStripMenuItem();
            this.toolStripSeparatorMain = new ToolStripSeparator();
            this.toolStripMenuItemFileExit = new ToolStripMenuItem();
            this.toolStripMenuItemEdit = new ToolStripMenuItem();
            this.contextMenuEdit = new ContextMenuStrip(this.components);
            this.toolStripMenuItemEditConnections = new ToolStripMenuItem();
            this.toolStripMenuItemEditViews = new ToolStripMenuItem();
            this.toolStripMenuItemEditDatabase = new ToolStripMenuItem();
            this.toolStripMenuItemConnections = new ToolStripMenuItem();
            this.contextMenuConnections = new ContextMenuStrip(this.components);
            this.toolStripMenuItemAbout = new ToolStripMenuItem();
            this.contextMenuAbout = new ContextMenuStrip(this.components);
            this.toolStripMenuItemAboutAbout = new ToolStripMenuItem();
            this.openFileDialogConfig = new OpenFileDialog();
            this.saveFileDialogConfig = new SaveFileDialog();
            this.groupBox_Sidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.splitContainerMessages).BeginInit();
            this.splitContainerMessages.Panel1.SuspendLayout();
            this.splitContainerMessages.Panel2.SuspendLayout();
            this.splitContainerMessages.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.splitContainerMainView).BeginInit();
            this.splitContainerMainView.Panel1.SuspendLayout();
            this.splitContainerMainView.Panel2.SuspendLayout();
            this.splitContainerMainView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxViewImage).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.dataGridViewDB).BeginInit();
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.databaseBindingSource).BeginInit();
            this.menuStripMain.SuspendLayout();
            this.contextMenuFile.SuspendLayout();
            this.contextMenuEdit.SuspendLayout();
            this.contextMenuAbout.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonMeasure
            // 
            this.buttonMeasure.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonMeasure.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point);
            this.buttonMeasure.Location = new Point(198, 149);
            this.buttonMeasure.Name = "buttonMeasure";
            this.buttonMeasure.Size = new Size(190, 50);
            this.buttonMeasure.TabIndex = 9;
            this.buttonMeasure.Text = "Measure";
            this.buttonMeasure.UseVisualStyleBackColor = true;
            this.buttonMeasure.Click += this.buttonMeasure_Click;
            // 
            // backgroundWorkerInitSensors
            // 
            this.backgroundWorkerInitSensors.DoWork += this.backgroundWorkerInitSensors_DoWork;
            // 
            // groupBox_Sidebar
            // 
            this.groupBox_Sidebar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.groupBox_Sidebar.Controls.Add(this.splitContainerMessages);
            this.groupBox_Sidebar.Controls.Add(this.textBoxUnit);
            this.groupBox_Sidebar.Controls.Add(this.labelStatus);
            this.groupBox_Sidebar.Controls.Add(this.labelStatusLabel);
            this.groupBox_Sidebar.Controls.Add(this.labelUnit);
            this.groupBox_Sidebar.Controls.Add(this.labelView);
            this.groupBox_Sidebar.Controls.Add(this.comboBoxView);
            this.groupBox_Sidebar.Controls.Add(this.buttonRemoveFilter);
            this.groupBox_Sidebar.Controls.Add(this.buttonFilter);
            this.groupBox_Sidebar.Controls.Add(this.buttonInit);
            this.groupBox_Sidebar.Controls.Add(this.buttonMeasure);
            this.groupBox_Sidebar.Location = new Point(384, 0);
            this.groupBox_Sidebar.Margin = new Padding(0);
            this.groupBox_Sidebar.Name = "groupBox_Sidebar";
            this.groupBox_Sidebar.Padding = new Padding(0);
            this.tableLayoutPanelMain.SetRowSpan(this.groupBox_Sidebar, 2);
            this.groupBox_Sidebar.Size = new Size(400, 337);
            this.groupBox_Sidebar.TabIndex = 3;
            this.groupBox_Sidebar.TabStop = false;
            // 
            // splitContainerMessages
            // 
            this.splitContainerMessages.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.splitContainerMessages.FixedPanel = FixedPanel.Panel1;
            this.splitContainerMessages.IsSplitterFixed = true;
            this.splitContainerMessages.Location = new Point(0, 202);
            this.splitContainerMessages.Margin = new Padding(0);
            this.splitContainerMessages.Name = "splitContainerMessages";
            this.splitContainerMessages.Orientation = Orientation.Horizontal;
            // 
            // splitContainerMessages.Panel1
            // 
            this.splitContainerMessages.Panel1.Controls.Add(this.panel1);
            this.splitContainerMessages.Panel1MinSize = 15;
            // 
            // splitContainerMessages.Panel2
            // 
            this.splitContainerMessages.Panel2.Controls.Add(this.textBoxMessages);
            this.splitContainerMessages.Panel2MinSize = 15;
            this.splitContainerMessages.Size = new Size(390, 132);
            this.splitContainerMessages.SplitterDistance = 25;
            this.splitContainerMessages.SplitterWidth = 1;
            this.splitContainerMessages.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelMessagesCollapse);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Margin = new Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(390, 25);
            this.panel1.TabIndex = 1;
            // 
            // labelMessagesCollapse
            // 
            this.labelMessagesCollapse.AutoSize = true;
            this.labelMessagesCollapse.Dock = DockStyle.Fill;
            this.labelMessagesCollapse.LinkColor = SystemColors.ControlText;
            this.labelMessagesCollapse.Location = new Point(0, 0);
            this.labelMessagesCollapse.MaximumSize = new Size(400, 25);
            this.labelMessagesCollapse.Name = "labelMessagesCollapse";
            this.labelMessagesCollapse.Size = new Size(120, 15);
            this.labelMessagesCollapse.TabIndex = 0;
            this.labelMessagesCollapse.TabStop = true;
            this.labelMessagesCollapse.Text = "Show/Hide Messages";
            this.labelMessagesCollapse.TextAlign = ContentAlignment.MiddleLeft;
            this.labelMessagesCollapse.VisitedLinkColor = SystemColors.ControlText;
            this.labelMessagesCollapse.LinkClicked += this.labelMessagesCollapse_Click;
            // 
            // textBoxMessages
            // 
            this.textBoxMessages.Dock = DockStyle.Fill;
            this.textBoxMessages.Location = new Point(0, 0);
            this.textBoxMessages.Multiline = true;
            this.textBoxMessages.Name = "textBoxMessages";
            this.textBoxMessages.ReadOnly = true;
            this.textBoxMessages.ScrollBars = ScrollBars.Vertical;
            this.textBoxMessages.Size = new Size(390, 106);
            this.textBoxMessages.TabIndex = 0;
            this.textBoxMessages.WordWrap = false;
            this.textBoxMessages.TextChanged += this.textBoxMessages_TextChanged;
            // 
            // textBoxUnit
            // 
            this.textBoxUnit.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            this.textBoxUnit.Location = new Point(238, 19);
            this.textBoxUnit.Name = "textBoxUnit";
            this.textBoxUnit.Size = new Size(150, 39);
            this.textBoxUnit.TabIndex = 3;
            this.textBoxUnit.TextChanged += this.textBoxUnit_TextChanged;
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new Point(51, 65);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new Size(337, 15);
            this.labelStatus.TabIndex = 5;
            this.labelStatus.Text = "Starting...";
            // 
            // labelStatusLabel
            // 
            this.labelStatusLabel.AutoSize = true;
            this.labelStatusLabel.Location = new Point(3, 65);
            this.labelStatusLabel.Name = "labelStatusLabel";
            this.labelStatusLabel.Size = new Size(42, 15);
            this.labelStatusLabel.TabIndex = 4;
            this.labelStatusLabel.Text = "Status:";
            // 
            // labelUnit
            // 
            this.labelUnit.AutoSize = true;
            this.labelUnit.Location = new Point(201, 30);
            this.labelUnit.Name = "labelUnit";
            this.labelUnit.Size = new Size(32, 15);
            this.labelUnit.TabIndex = 2;
            this.labelUnit.Text = "Unit:";
            // 
            // labelView
            // 
            this.labelView.AutoSize = true;
            this.labelView.Location = new Point(4, 30);
            this.labelView.Name = "labelView";
            this.labelView.Size = new Size(35, 15);
            this.labelView.TabIndex = 0;
            this.labelView.Text = "View:";
            // 
            // comboBoxView
            // 
            this.comboBoxView.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxView.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            this.comboBoxView.FormattingEnabled = true;
            this.comboBoxView.Location = new Point(44, 19);
            this.comboBoxView.Name = "comboBoxView";
            this.comboBoxView.Size = new Size(150, 40);
            this.comboBoxView.TabIndex = 1;
            this.comboBoxView.SelectedIndexChanged += this.comboBoxView_SelectedIndexChanged;
            // 
            // buttonRemoveFilter
            // 
            this.buttonRemoveFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonRemoveFilter.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            this.buttonRemoveFilter.Location = new Point(198, 93);
            this.buttonRemoveFilter.Name = "buttonRemoveFilter";
            this.buttonRemoveFilter.Size = new Size(190, 50);
            this.buttonRemoveFilter.TabIndex = 7;
            this.buttonRemoveFilter.Text = "Remove Filter";
            this.buttonRemoveFilter.UseVisualStyleBackColor = true;
            this.buttonRemoveFilter.Click += this.buttonRemoveFilter_Click;
            // 
            // buttonFilter
            // 
            this.buttonFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonFilter.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            this.buttonFilter.Location = new Point(4, 93);
            this.buttonFilter.Name = "buttonFilter";
            this.buttonFilter.Size = new Size(190, 50);
            this.buttonFilter.TabIndex = 6;
            this.buttonFilter.Text = "Apply Filter";
            this.buttonFilter.UseVisualStyleBackColor = true;
            this.buttonFilter.Click += this.buttonFilter_Click;
            // 
            // buttonInit
            // 
            this.buttonInit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.buttonInit.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            this.buttonInit.Location = new Point(4, 149);
            this.buttonInit.Name = "buttonInit";
            this.buttonInit.Size = new Size(190, 50);
            this.buttonInit.TabIndex = 8;
            this.buttonInit.Text = "Re-Initialize";
            this.buttonInit.UseVisualStyleBackColor = true;
            this.buttonInit.Click += this.buttonInit_Click;
            // 
            // splitContainerMainView
            // 
            this.splitContainerMainView.Dock = DockStyle.Fill;
            this.splitContainerMainView.Location = new Point(3, 3);
            this.splitContainerMainView.Name = "splitContainerMainView";
            this.splitContainerMainView.Orientation = Orientation.Horizontal;
            // 
            // splitContainerMainView.Panel1
            // 
            this.splitContainerMainView.Panel1.Controls.Add(this.pictureBoxViewImage);
            // 
            // splitContainerMainView.Panel2
            // 
            this.splitContainerMainView.Panel2.Controls.Add(this.dataGridViewDB);
            this.tableLayoutPanelMain.SetRowSpan(this.splitContainerMainView, 2);
            this.splitContainerMainView.Size = new Size(378, 331);
            this.splitContainerMainView.SplitterDistance = 265;
            this.splitContainerMainView.TabIndex = 0;
            // 
            // pictureBoxViewImage
            // 
            this.pictureBoxViewImage.BackColor = Color.Gray;
            this.pictureBoxViewImage.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxViewImage.Dock = DockStyle.Fill;
            this.pictureBoxViewImage.Location = new Point(0, 0);
            this.pictureBoxViewImage.Margin = new Padding(0);
            this.pictureBoxViewImage.Name = "pictureBoxViewImage";
            this.pictureBoxViewImage.Size = new Size(378, 265);
            this.pictureBoxViewImage.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxViewImage.TabIndex = 1;
            this.pictureBoxViewImage.TabStop = false;
            this.pictureBoxViewImage.SizeChanged += this.pictureBoxViewImage_SizeChanged;
            // 
            // dataGridViewDB
            // 
            this.dataGridViewDB.AllowUserToAddRows = false;
            this.dataGridViewDB.AllowUserToDeleteRows = false;
            this.dataGridViewDB.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridViewDB.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.ControlDark;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            this.dataGridViewDB.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewDB.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Window;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            this.dataGridViewDB.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewDB.Dock = DockStyle.Fill;
            this.dataGridViewDB.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewDB.Location = new Point(0, 0);
            this.dataGridViewDB.Name = "dataGridViewDB";
            this.dataGridViewDB.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.ControlDark;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            this.dataGridViewDB.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewDB.RowHeadersWidth = 72;
            this.dataGridViewDB.RowTemplate.DefaultCellStyle.BackColor = SystemColors.Window;
            this.dataGridViewDB.RowTemplate.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            this.dataGridViewDB.RowTemplate.Height = 25;
            this.dataGridViewDB.Size = new Size(378, 62);
            this.dataGridViewDB.TabIndex = 4;
            this.dataGridViewDB.TabStop = false;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
            this.tableLayoutPanelMain.Controls.Add(this.splitContainerMainView, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.groupBox_Sidebar, 1, 0);
            this.tableLayoutPanelMain.Dock = DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new Point(0, 24);
            this.tableLayoutPanelMain.Margin = new Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            this.tableLayoutPanelMain.Size = new Size(784, 337);
            this.tableLayoutPanelMain.TabIndex = 4;
            // 
            // backgroundWorkerMeasure
            // 
            this.backgroundWorkerMeasure.DoWork += this.backgroundWorkerMeasure_DoWork;
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new ToolStripItem[] { this.toolStripMenuItemFile, this.toolStripMenuItemEdit, this.toolStripMenuItemConnections, this.toolStripMenuItemAbout });
            this.menuStripMain.Location = new Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new Size(784, 24);
            this.menuStripMain.TabIndex = 5;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // toolStripMenuItemFile
            // 
            this.toolStripMenuItemFile.DropDown = this.contextMenuFile;
            this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
            this.toolStripMenuItemFile.Size = new Size(37, 20);
            this.toolStripMenuItemFile.Text = "File";
            // 
            // contextMenuFile
            // 
            this.contextMenuFile.Items.AddRange(new ToolStripItem[] { this.toolStripMenuItemFileNewConfig, this.toolStripMenuItemFileLoadConfig, this.toolStripMenuItemFileSaveConfig, this.toolStripSeparatorMain, this.toolStripMenuItemFileExit });
            this.contextMenuFile.Name = "contextMenuFile";
            this.contextMenuFile.OwnerItem = this.toolStripMenuItemFile;
            this.contextMenuFile.Size = new Size(238, 98);
            // 
            // toolStripMenuItemFileNewConfig
            // 
            this.toolStripMenuItemFileNewConfig.Name = "toolStripMenuItemFileNewConfig";
            this.toolStripMenuItemFileNewConfig.Size = new Size(237, 22);
            this.toolStripMenuItemFileNewConfig.Text = "New empty configuration";
            this.toolStripMenuItemFileNewConfig.Click += this.toolStripMenuItemFileNewConfig_Click;
            // 
            // toolStripMenuItemFileLoadConfig
            // 
            this.toolStripMenuItemFileLoadConfig.Name = "toolStripMenuItemFileLoadConfig";
            this.toolStripMenuItemFileLoadConfig.Size = new Size(237, 22);
            this.toolStripMenuItemFileLoadConfig.Text = "Load configuration file...";
            this.toolStripMenuItemFileLoadConfig.Click += this.toolStripMenuItemFileLoadConfig_Click;
            // 
            // toolStripMenuItemFileSaveConfig
            // 
            this.toolStripMenuItemFileSaveConfig.Name = "toolStripMenuItemFileSaveConfig";
            this.toolStripMenuItemFileSaveConfig.Size = new Size(237, 22);
            this.toolStripMenuItemFileSaveConfig.Text = "Save current configuration as...";
            this.toolStripMenuItemFileSaveConfig.Click += this.toolStripMenuItemFileSaveConfig_Click;
            // 
            // toolStripSeparatorMain
            // 
            this.toolStripSeparatorMain.Name = "toolStripSeparatorMain";
            this.toolStripSeparatorMain.Size = new Size(234, 6);
            // 
            // toolStripMenuItemFileExit
            // 
            this.toolStripMenuItemFileExit.Name = "toolStripMenuItemFileExit";
            this.toolStripMenuItemFileExit.Size = new Size(237, 22);
            this.toolStripMenuItemFileExit.Text = "Exit";
            this.toolStripMenuItemFileExit.Click += this.toolStripMenuItemFileExit_Click;
            // 
            // toolStripMenuItemEdit
            // 
            this.toolStripMenuItemEdit.DropDown = this.contextMenuEdit;
            this.toolStripMenuItemEdit.Name = "toolStripMenuItemEdit";
            this.toolStripMenuItemEdit.Size = new Size(39, 20);
            this.toolStripMenuItemEdit.Text = "Edit";
            // 
            // contextMenuEdit
            // 
            this.contextMenuEdit.Items.AddRange(new ToolStripItem[] { this.toolStripMenuItemEditConnections, this.toolStripMenuItemEditViews, this.toolStripMenuItemEditDatabase });
            this.contextMenuEdit.Name = "contextMenuEdit";
            this.contextMenuEdit.OwnerItem = this.toolStripMenuItemEdit;
            this.contextMenuEdit.Size = new Size(191, 70);
            // 
            // toolStripMenuItemEditConnections
            // 
            this.toolStripMenuItemEditConnections.Name = "toolStripMenuItemEditConnections";
            this.toolStripMenuItemEditConnections.Size = new Size(190, 22);
            this.toolStripMenuItemEditConnections.Text = "Connection Settings...";
            this.toolStripMenuItemEditConnections.Click += this.toolStripMenuItemEditConnections_Click;
            // 
            // toolStripMenuItemEditViews
            // 
            this.toolStripMenuItemEditViews.Name = "toolStripMenuItemEditViews";
            this.toolStripMenuItemEditViews.Size = new Size(190, 22);
            this.toolStripMenuItemEditViews.Text = "View Settings...";
            this.toolStripMenuItemEditViews.Click += this.toolStripMenuItemEditViews_Click;
            // 
            // toolStripMenuItemEditDatabase
            // 
            this.toolStripMenuItemEditDatabase.Name = "toolStripMenuItemEditDatabase";
            this.toolStripMenuItemEditDatabase.Size = new Size(190, 22);
            this.toolStripMenuItemEditDatabase.Text = "Database Settings...";
            this.toolStripMenuItemEditDatabase.Click += this.toolStripMenuItemEditDatabase_Click;
            // 
            // toolStripMenuItemConnections
            // 
            this.toolStripMenuItemConnections.DropDown = this.contextMenuConnections;
            this.toolStripMenuItemConnections.Name = "toolStripMenuItemConnections";
            this.toolStripMenuItemConnections.Size = new Size(86, 20);
            this.toolStripMenuItemConnections.Text = "Connections";
            // 
            // contextMenuConnections
            // 
            this.contextMenuConnections.Name = "contextMenuConnections";
            this.contextMenuConnections.OwnerItem = this.toolStripMenuItemConnections;
            this.contextMenuConnections.Size = new Size(61, 4);
            // 
            // toolStripMenuItemAbout
            // 
            this.toolStripMenuItemAbout.DropDown = this.contextMenuAbout;
            this.toolStripMenuItemAbout.Name = "toolStripMenuItemAbout";
            this.toolStripMenuItemAbout.Size = new Size(52, 20);
            this.toolStripMenuItemAbout.Text = "About";
            // 
            // contextMenuAbout
            // 
            this.contextMenuAbout.Items.AddRange(new ToolStripItem[] { this.toolStripMenuItemAboutAbout });
            this.contextMenuAbout.Name = "contextMenuAbout";
            this.contextMenuAbout.OwnerItem = this.toolStripMenuItemAbout;
            this.contextMenuAbout.Size = new Size(181, 48);
            // 
            // toolStripMenuItemAboutAbout
            // 
            this.toolStripMenuItemAboutAbout.Name = "toolStripMenuItemAboutAbout";
            this.toolStripMenuItemAboutAbout.Size = new Size(180, 22);
            this.toolStripMenuItemAboutAbout.Text = "About {this.Text}";
            this.toolStripMenuItemAboutAbout.Click += this.toolStripMenuItemAboutAbout_Click;
            // 
            // openFileDialogConfig
            // 
            this.openFileDialogConfig.DefaultExt = "json";
            this.openFileDialogConfig.FileName = "config.json";
            this.openFileDialogConfig.Filter = "JSON Files (*.JSON)|*.json|All Files (*.*)|*.*";
            // 
            // saveFileDialogConfig
            // 
            this.saveFileDialogConfig.DefaultExt = "json";
            this.saveFileDialogConfig.FileName = "config.json";
            this.saveFileDialogConfig.Filter = "JSON Files (*.JSON)|*.json|All Files (*.*)|*.*";
            // 
            // MainWindow
            // 
            this.AcceptButton = this.buttonMeasure;
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.labelMessagesCollapse;
            this.ClientSize = new Size(784, 361);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Controls.Add(this.menuStripMain);
            this.Icon = (Icon)resources.GetObject("$this.Icon");
            this.MainMenuStrip = this.menuStripMain;
            this.MinimumSize = new Size(500, 350);
            this.Name = "MainWindow";
            this.Text = "ILInspect";
            this.FormClosed += this.MainWindow_FormClosed;
            this.Shown += this.MainWindow_Shown;
            this.groupBox_Sidebar.ResumeLayout(false);
            this.groupBox_Sidebar.PerformLayout();
            this.splitContainerMessages.Panel1.ResumeLayout(false);
            this.splitContainerMessages.Panel2.ResumeLayout(false);
            this.splitContainerMessages.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.splitContainerMessages).EndInit();
            this.splitContainerMessages.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainerMainView.Panel1.ResumeLayout(false);
            this.splitContainerMainView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.splitContainerMainView).EndInit();
            this.splitContainerMainView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxViewImage).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.dataGridViewDB).EndInit();
            this.tableLayoutPanelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.databaseBindingSource).EndInit();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.contextMenuFile.ResumeLayout(false);
            this.contextMenuEdit.ResumeLayout(false);
            this.contextMenuAbout.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion


        private Button buttonMeasure;
        private System.ComponentModel.BackgroundWorker backgroundWorkerInitSensors;
        private GroupBox groupBox_Sidebar;
        private PictureBox pictureBoxViewImage;
        private TableLayoutPanel tableLayoutPanelMain;
        private Label labelView;
        private ComboBox comboBoxView;
        public DataGridView dataGridViewDB;
        private BindingSource databaseBindingSource;
        private SplitContainer splitContainerMainView;
        private TextBox textBoxUnit;
        private Label labelUnit;
        private Label labelStatus;
        private Label labelStatusLabel;
        private System.ComponentModel.BackgroundWorker backgroundWorkerMeasure;
        private Button buttonInit;
        private Button buttonFilter;
        private Button buttonRemoveFilter;
        private SplitContainer splitContainerMessages;
        private LinkLabel labelMessagesCollapse;
        public TextBox textBoxMessages;
        private Panel panel1;
        private MenuStrip menuStripMain;
        private ToolStripMenuItem toolStripMenuItemFile;
        private ContextMenuStrip contextMenuFile;
        private ToolStripMenuItem toolStripMenuItemFileLoadConfig;
        private ToolStripSeparator toolStripSeparatorMain;
        private ToolStripMenuItem toolStripMenuItemFileExit;
        private ToolStripMenuItem toolStripMenuItemConnections;
        private ContextMenuStrip contextMenuConnections;
        private OpenFileDialog openFileDialogConfig;
        private ToolStripMenuItem toolStripMenuItemFileSaveConfig;
        private SaveFileDialog saveFileDialogConfig;
        private ToolStripMenuItem toolStripMenuItemAbout;
        private ContextMenuStrip contextMenuAbout;
        private ToolStripMenuItem toolStripMenuItemAboutAbout;
        private ToolStripMenuItem toolStripMenuItemEdit;
        private ContextMenuStrip contextMenuEdit;
        private ToolStripMenuItem toolStripMenuItemEditConnections;
        private ToolStripMenuItem toolStripMenuItemEditViews;
        private ToolStripMenuItem toolStripMenuItemEditDatabase;
        private ToolStripMenuItem toolStripMenuItemFileNewConfig;
    }
}