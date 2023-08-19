namespace ILInspect {

    partial class SensorBox {
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.textBoxMeasurement = new TextBox();
            this.pictureBoxState = new PictureBox();
            this.labelId = new Label();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxState).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxMeasurement
            // 
            this.textBoxMeasurement.Location = new Point(0, 15);
            this.textBoxMeasurement.Margin = new Padding(0);
            this.textBoxMeasurement.Name = "textBoxMeasurement";
            this.textBoxMeasurement.ReadOnly = true;
            this.textBoxMeasurement.Size = new Size(47, 23);
            this.textBoxMeasurement.TabIndex = 0;
            this.textBoxMeasurement.Text = "- - - - -";
            // 
            // pictureBoxState
            // 
            this.pictureBoxState.Image = Properties.Resources.grey_circle;
            this.pictureBoxState.Location = new Point(31, 0);
            this.pictureBoxState.Margin = new Padding(0);
            this.pictureBoxState.Name = "pictureBoxState";
            this.pictureBoxState.Size = new Size(15, 15);
            this.pictureBoxState.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxState.TabIndex = 1;
            this.pictureBoxState.TabStop = false;
            this.pictureBoxState.MouseClick += this.SensorBox_MouseClick;
            // 
            // labelId
            // 
            this.labelId.AutoSize = true;
            this.labelId.BackColor = SystemColors.Control;
            this.labelId.Dock = DockStyle.Left;
            this.labelId.Location = new Point(0, 0);
            this.labelId.Margin = new Padding(0);
            this.labelId.Name = "labelId";
            this.labelId.Size = new Size(31, 15);
            this.labelId.TabIndex = 2;
            this.labelId.Text = "1234";
            this.labelId.MouseClick += this.SensorBox_MouseClick;
            // 
            // SensorBox
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(this.textBoxMeasurement);
            this.Controls.Add(this.labelId);
            this.Controls.Add(this.pictureBoxState);
            this.Margin = new Padding(0);
            this.Name = "SensorBox";
            this.Size = new Size(47, 38);
            this.MouseClick += this.SensorBox_MouseClick;
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxState).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        public TextBox textBoxMeasurement;
        public PictureBox pictureBoxState;
        public Label labelId;
    }
}
