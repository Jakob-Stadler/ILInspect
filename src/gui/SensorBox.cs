namespace ILInspect {

    public partial class SensorBox : UserControl {
        public SensorUsageConfig config;
        public SensorProxy proxy;
        private MessageCollection messageCollection;
        public SensorContextMenu contextMenus;

        public SensorBox(SensorUsageConfig config, SensorProxy proxy, SensorContextMenu contextMenus, MessageCollection messageCollection) {
            this.config = config;
            this.proxy = proxy;
            this.messageCollection = messageCollection;
            this.InitializeComponent();
            this.contextMenus = contextMenus;
            this.labelId.Text = config.ID;
            this.adjustSize();
        }

        public void initSensor() {
            int? bank = this.config.Bank;
            if (bank == null || bank < 0 || bank > 3) {
                return;
            } else {
                this.proxy.sendSetBank((int)bank);
                if (this.config.HighThreshold != null) {
                    this.proxy.sendSetHIGH((int)bank, (int)(this.config.HighThreshold / this.proxy.config.ConversionFactor));
                }
                if (this.config.LowThreshold != null) {
                    this.proxy.sendSetLOW((int)bank, (int)(this.config.LowThreshold / this.proxy.config.ConversionFactor));
                }
                if (this.config.ShiftTarget != null) {
                    this.proxy.sendSetShiftTarget((int)bank, (int)(this.config.ShiftTarget / this.proxy.config.ConversionFactor));
                }
                if (this.config.AnalogUpperBound != null) {
                    this.proxy.sendSetAnalogOutputUpperLimit((int)bank, (int)(this.config.AnalogUpperBound / this.proxy.config.ConversionFactor));
                }
                if (this.config.AnalogLowerBound != null) {
                    this.proxy.sendSetAnalogOutputLowerLimit((int)bank, (int)(this.config.AnalogLowerBound / this.proxy.config.ConversionFactor));
                }
            }
        }

        public void adjustSize() {
            if (this.Width < this.labelId.Width + this.pictureBoxState.Width) {
                this.Width = this.labelId.Width + 1 + this.pictureBoxState.Width + 1;
                this.textBoxMeasurement.Width = this.Width;
            }
            this.pictureBoxState.Left = this.labelId.Width + 1;
            this.Height = this.textBoxMeasurement.Top + this.textBoxMeasurement.Height + 1;
        }

        public void makeRed() {
            this.pictureBoxState.Image = Properties.Resources.red_circle;
        }

        public void makeGreen() {
            this.pictureBoxState.Image = Properties.Resources.green_circle;
        }

        public void makeGrey() {
            this.pictureBoxState.Image = Properties.Resources.grey_circle;
        }

        public void setMeasurementText(string message) {
            this.textBoxMeasurement.Invoke(new Action(() => { this.textBoxMeasurement.Text = message; }));
        }

        private void SensorBox_MouseClick(object sender, MouseEventArgs e) {
            this.contextMenus.ShowContextMenu((Control)sender, e.X, e.Y);
        }
    }
}
