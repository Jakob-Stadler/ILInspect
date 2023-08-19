namespace ILInspect {
    partial class DatabaseEditor {
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
            this.panelCenter = new Panel();
            this.labelHorizontalDivider = new Label();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.buttonDatabaseMemory = new Button();
            this.buttonDatabaseBrowse = new Button();
            this.textBoxDatabaseSource = new TextBox();
            this.labelDatabaseSource = new Label();
            this.saveFileDialogSource = new SaveFileDialog();
            this.tableLayoutPanelMain.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tableLayoutPanelMain.Controls.Add(this.panelCenter, 0, 0);
            this.tableLayoutPanelMain.Dock = DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 1;
            this.tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            this.tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.Size = new Size(534, 131);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // panelCenter
            // 
            this.panelCenter.Controls.Add(this.labelHorizontalDivider);
            this.panelCenter.Controls.Add(this.buttonCancel);
            this.panelCenter.Controls.Add(this.buttonOK);
            this.panelCenter.Controls.Add(this.buttonDatabaseMemory);
            this.panelCenter.Controls.Add(this.buttonDatabaseBrowse);
            this.panelCenter.Controls.Add(this.textBoxDatabaseSource);
            this.panelCenter.Controls.Add(this.labelDatabaseSource);
            this.panelCenter.Dock = DockStyle.Fill;
            this.panelCenter.Location = new Point(3, 3);
            this.panelCenter.Name = "panelCenter";
            this.panelCenter.Size = new Size(528, 125);
            this.panelCenter.TabIndex = 0;
            // 
            // labelHorizontalDivider
            // 
            this.labelHorizontalDivider.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.labelHorizontalDivider.BorderStyle = BorderStyle.Fixed3D;
            this.labelHorizontalDivider.Location = new Point(12, 82);
            this.labelHorizontalDivider.Margin = new Padding(6);
            this.labelHorizontalDivider.Name = "labelHorizontalDivider";
            this.labelHorizontalDivider.Size = new Size(504, 2);
            this.labelHorizontalDivider.TabIndex = 4;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.buttonCancel.Location = new Point(215, 93);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(200, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.buttonOK.Location = new Point(9, 93);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new Size(200, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonDatabaseMemory
            // 
            this.buttonDatabaseMemory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.buttonDatabaseMemory.Location = new Point(319, 38);
            this.buttonDatabaseMemory.Name = "buttonDatabaseMemory";
            this.buttonDatabaseMemory.Size = new Size(200, 23);
            this.buttonDatabaseMemory.TabIndex = 3;
            this.buttonDatabaseMemory.Text = "No File - Temporary Memory only";
            this.buttonDatabaseMemory.UseVisualStyleBackColor = true;
            this.buttonDatabaseMemory.Click += this.buttonDatabaseMemory_Click;
            // 
            // buttonDatabaseBrowse
            // 
            this.buttonDatabaseBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.buttonDatabaseBrowse.Location = new Point(113, 38);
            this.buttonDatabaseBrowse.Name = "buttonDatabaseBrowse";
            this.buttonDatabaseBrowse.Size = new Size(200, 23);
            this.buttonDatabaseBrowse.TabIndex = 2;
            this.buttonDatabaseBrowse.Text = "Select File...";
            this.buttonDatabaseBrowse.UseVisualStyleBackColor = true;
            this.buttonDatabaseBrowse.Click += this.buttonDatabaseBrowse_Click;
            // 
            // textBoxDatabaseSource
            // 
            this.textBoxDatabaseSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.textBoxDatabaseSource.Location = new Point(165, 9);
            this.textBoxDatabaseSource.Name = "textBoxDatabaseSource";
            this.textBoxDatabaseSource.Size = new Size(354, 23);
            this.textBoxDatabaseSource.TabIndex = 1;
            // 
            // labelDatabaseSource
            // 
            this.labelDatabaseSource.Location = new Point(9, 9);
            this.labelDatabaseSource.Name = "labelDatabaseSource";
            this.labelDatabaseSource.Size = new Size(150, 23);
            this.labelDatabaseSource.TabIndex = 0;
            this.labelDatabaseSource.Text = "Database Source (File):";
            this.labelDatabaseSource.TextAlign = ContentAlignment.MiddleRight;
            // 
            // saveFileDialogSource
            // 
            this.saveFileDialogSource.FileName = "database.db";
            this.saveFileDialogSource.Filter = "All Files (*.*)|*.*";
            this.saveFileDialogSource.Title = "Select SQLite Database File";
            // 
            // DatabaseEditor
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new Size(534, 131);
            this.Controls.Add(this.tableLayoutPanelMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new Size(450, 160);
            this.Name = "DatabaseEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Config - Edit Database Settings";
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.panelCenter.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanelMain;
        private Panel panelCenter;
        private Button buttonCancel;
        private Button buttonOK;
        private Button buttonDatabaseMemory;
        private Button buttonDatabaseBrowse;
        private TextBox textBoxDatabaseSource;
        private Label labelDatabaseSource;
        private Label labelHorizontalDivider;
        private SaveFileDialog saveFileDialogSource;
    }
}