using System.Data;

namespace ILInspect {
    public partial class ViewEditor : Form {
        private enum EditState {
            None,
            Add,
            Edit,
            Remove
        }

        private JSONConfig config;
        private IList<SensorUsageConfig> sensors;
        private EditState viewEditState;
        private EditState sensorEditState;
        private bool configChanged = false;

        public ViewEditor(JSONConfig config) {
            this.InitializeComponent();
            this.pictureBoxSensorLocation.Parent = this.pictureBoxPreview;
            this.config = config;
            this.sensors = new List<SensorUsageConfig>();
            this.viewEditState = EditState.None;
            this.sensorEditState = EditState.None;
            this.populateIDDropdown();
            this.fillFields();
        }

        private void populateIDDropdown() {
            List<string> list = new();
            foreach (ConnectionConfig connection in this.config.SensorConnections) {
                list.AddRange(from sensor in connection.Sensors select sensor.ID);
            }
            this.comboBoxSensorID.Items.Clear();
            this.comboBoxSensorID.Items.AddRange(list.ToArray());
        }

        private void fillFields() {
            this.listBoxSelectView.Items.AddRange(
                (from view in this.config.Views select view.Name).ToArray()
            );
        }
        private void fillView() {
            int index = this.listBoxSelectView.SelectedIndex;
            if (index < 0) {
                // Clear fields
                this.textBoxViewName.Text = string.Empty;
                this.textBoxViewImageLocation.Text = string.Empty;
                this.pictureBoxPreview.Image = null;
                this.listBoxSelectSensor.Items.Clear();
                this.sensors = new List<SensorUsageConfig>();
                this.buttonViewAdd.Enabled = true;
                this.buttonViewEdit.Enabled = false;
                this.buttonViewRemove.Enabled = false;

            } else if (index < this.config.Views.Count) {
                ViewConfig viewConfig = this.config.Views[index];
                this.textBoxViewName.Text = viewConfig.Name;
                this.textBoxViewImageLocation.Text = viewConfig.ImageLocation;
                Image? image = viewConfig.Image;
                if (image == null) {
                    string relativePath = viewConfig.ImageLocation;
                    string fullPath = Path.GetFullPath(relativePath, this.config.ConfigDirectory ?? AppDomain.CurrentDomain.BaseDirectory);
                    try {
                        image = Image.FromFile(fullPath);
                        // Cache image
                        viewConfig.Image = image;
                    }
                    catch (Exception ex) {
                        MessageBox.Show(
                            $"Failed to load image file!\r\n{ex}",
                            "Loading failed!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        image = Properties.Resources.Warning;
                    }
                }
                this.pictureBoxPreview.Image = image;
                this.listBoxSelectSensor.Items.Clear();
                this.sensors = viewConfig.Sensors;
                this.listBoxSelectSensor.Items.AddRange(
                    (from sensor in this.sensors select $"{sensor.ID}").ToArray()
                );
                this.buttonViewAdd.Enabled = true;
                this.buttonViewEdit.Enabled = true;
                this.buttonViewRemove.Enabled = true;
            } else {
                // View not currently in config
            }
        }

        private void fillSensor() {
            int index = this.listBoxSelectSensor.SelectedIndex;
            if (index < 0) {
                // Clear fields
                this.comboBoxSensorID.Text = string.Empty;
                this.textBoxSensorPosX.Text = string.Empty;
                this.textBoxSensorPosY.Text = string.Empty;
                this.comboBoxSensorBank.Text = string.Empty;
                this.textBoxSensorShift.Text = string.Empty;
                this.textBoxSensorHigh.Text = string.Empty;
                this.textBoxSensorLow.Text = string.Empty;
                this.textBoxSensorAnalogUpper.Text = string.Empty;
                this.textBoxSensorAnalogLower.Text = string.Empty;
                this.buttonSensorAdd.Enabled = true;
                this.buttonSensorEdit.Enabled = false;
                this.buttonSensorRemove.Enabled = false;

            } else if (index < this.sensors.Count) {
                SensorUsageConfig sensorConfig = this.sensors![index];

                this.comboBoxSensorID.Text = sensorConfig.ID;
                this.textBoxSensorPosX.Text = double.Round(sensorConfig.PositionX * 100, 3).ToString();
                this.textBoxSensorPosY.Text = double.Round(sensorConfig.PositionY * 100, 3).ToString();
                this.comboBoxSensorBank.Text = sensorConfig.Bank?.ToString() ?? string.Empty;
                this.textBoxSensorShift.Text = sensorConfig.ShiftTarget?.ToString() ?? string.Empty;
                this.textBoxSensorHigh.Text = sensorConfig.HighThreshold?.ToString() ?? string.Empty;
                this.textBoxSensorLow.Text = sensorConfig.LowThreshold?.ToString() ?? string.Empty;
                this.textBoxSensorAnalogUpper.Text = sensorConfig.AnalogUpperBound?.ToString() ?? string.Empty;
                this.textBoxSensorAnalogLower.Text = sensorConfig.AnalogLowerBound?.ToString() ?? string.Empty;

                this.adjustSensorPosition();

                if (this.viewEditState == EditState.Add || this.viewEditState == EditState.Edit) {
                    this.buttonSensorAdd.Enabled = true;
                    this.buttonSensorEdit.Enabled = true;
                    this.buttonSensorRemove.Enabled = true;
                }
            } else {
                // Sensor not currently in config
            }
        }

        private void adjustSensorPosition() {
            double posX;
            double posY;
            if (!double.TryParse(this.textBoxSensorPosX.Text, out posX)) {
                this.pictureBoxSensorLocation.Visible = false;
                return;
            }
            if (!double.TryParse(this.textBoxSensorPosY.Text, out posY)) {
                this.pictureBoxSensorLocation.Visible = false;
                return;
            }


            PictureBox pb = this.pictureBoxPreview;
            Image img = pb.Image;

            if (img == null)
                return;

            double wfactor = (double)img.Width / pb.ClientSize.Width;
            double hfactor = (double)img.Height / pb.ClientSize.Height;

            double resizeFactor = Math.Max(wfactor, hfactor);
            Size imageSize = new(
                (int)(img.Width / resizeFactor),
                (int)(img.Height / resizeFactor)
            );
            int offsetX = (pb.Width - imageSize.Width) / 2;
            int offsetY = (pb.Height - imageSize.Height) / 2;

            this.pictureBoxSensorLocation.Location = new Point(
                (int)(offsetX + posX * 0.01 * imageSize.Width - (this.pictureBoxSensorLocation.Width / 2)),
                (int)(offsetY + posY * 0.01 * imageSize.Height - (this.pictureBoxSensorLocation.Height / 2))
            );
            this.pictureBoxSensorLocation.Visible = true;
        }

        private void buttonViewAdd_Click(object sender, EventArgs e) {
            int index = this.listBoxSelectView.Items.Add("<New Entry>");
            this.listBoxSelectView.SelectedIndex = index;
            this.listBoxSelectView.Enabled = false;

            this.textBoxViewName.Text = string.Empty;
            this.textBoxViewImageLocation.Text = string.Empty;
            this.pictureBoxPreview.Image = null;
            this.listBoxSelectSensor.Items.Clear();
            this.sensors = new List<SensorUsageConfig>();

            this.buttonViewAdd.Enabled = false;
            this.buttonViewEdit.Enabled = false;
            this.buttonViewRemove.Enabled = false;
            this.buttonViewOK.Enabled = true;
            this.buttonViewCancel.Enabled = true;

            this.buttonLoadPreview.Enabled = true;
            this.buttonViewLocateImage.Enabled = true;

            this.labelViewName.Enabled = true;
            this.labelViewImageLocation.Enabled = true;
            this.labelViewPreview.Enabled = true;
            this.textBoxViewName.Enabled = true;
            this.textBoxViewImageLocation.Enabled = true;
            this.pictureBoxPreview.Enabled = true;

            this.buttonSensorAdd.Enabled = true;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;

            this.buttonClose.Enabled = false;

            this.viewEditState = EditState.Add;
            this.sensorEditState = EditState.None;
        }

        private void buttonViewEdit_Click(object sender, EventArgs e) {
            this.listBoxSelectView.Enabled = false;

            this.buttonViewAdd.Enabled = false;
            this.buttonViewEdit.Enabled = false;
            this.buttonViewRemove.Enabled = false;
            this.buttonViewOK.Enabled = true;
            this.buttonViewCancel.Enabled = true;

            // Clone sensor list, don't work with direct reference
            this.sensors = (
                from sensor in this.sensors select (SensorUsageConfig)sensor.Clone()
            ).ToList();

            this.buttonLoadPreview.Enabled = true;
            this.buttonViewLocateImage.Enabled = true;

            this.labelViewName.Enabled = true;
            this.labelViewImageLocation.Enabled = true;
            this.labelViewPreview.Enabled = true;
            this.textBoxViewName.Enabled = true;
            this.textBoxViewImageLocation.Enabled = true;
            this.pictureBoxPreview.Enabled = true;

            this.buttonSensorAdd.Enabled = true;
            this.buttonSensorEdit.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);
            this.buttonSensorRemove.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);

            this.buttonClose.Enabled = false;

            this.viewEditState = EditState.Edit;
            this.sensorEditState = EditState.None;
        }

        private void buttonViewRemove_Click(object sender, EventArgs e) {
            this.buttonViewAdd.Enabled = false;
            this.buttonViewEdit.Enabled = false;
            this.buttonViewRemove.Enabled = false;
            this.buttonViewOK.Enabled = true;
            this.buttonViewCancel.Enabled = true;

            this.buttonClose.Enabled = false;

            this.viewEditState = EditState.Remove;
            this.sensorEditState = EditState.None;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void buttonViewLocateImage_Click(object sender, EventArgs e) {
            DialogResult dialogResult = this.openFileDialogLocateImage.ShowDialog();
            if (dialogResult == DialogResult.OK) {
                this.textBoxViewImageLocation.Text = this.openFileDialogLocateImage.FileName;
            }
        }

        private void buttonLoadPreview_Click(object sender, EventArgs e) {
            try {
                this.pictureBoxPreview.Image = Image.FromFile(this.textBoxViewImageLocation.Text);
                return;
            }
            catch (Exception ex) {
                MessageBox.Show(
                    $"Failed to load image file!\r\n{ex}",
                    "Loading failed!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                this.pictureBoxPreview.Image = Properties.Resources.Warning;
                return;
            }
        }

        private void buttonViewOK_Click(object sender, EventArgs e) {

            string name = this.textBoxViewName.Text;
            if (string.IsNullOrEmpty(name)) {
                MessageBox.Show(
                    $"Missing field Name!",
                    "Missing field!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            string imageLocation = this.textBoxViewImageLocation.Text;
            if (string.IsNullOrEmpty(imageLocation)) {
                MessageBox.Show(
                    $"Missing field Image Location!",
                    "Missing field!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            Image? image = this.pictureBoxPreview.Image;
            if (image == Properties.Resources.Warning) {
                image = null;
            }

            ViewConfig view;
            int index = this.listBoxSelectView.SelectedIndex;

            switch (this.viewEditState) {
                case EditState.Add:
                    // Update Listbox and add to Config
                    view = new() {
                        Name = name,
                        ImageLocation = imageLocation,
                        Image = image,
                        Sensors = this.sensors
                    };
                    this.config.Views.Add(view);
                    this.listBoxSelectView.Items[index] = view.Name;
                    break;

                case EditState.Edit:
                    // Update Listbox and change in config
                    view = this.config.Views[index];
                    view.Name = name;
                    view.ImageLocation = imageLocation;
                    view.Image = image;
                    view.Sensors = this.sensors;

                    this.listBoxSelectView.Items[index] = view.Name;
                    break;

                case EditState.Remove:
                    // Update Listbox and remove from config
                    this.config.Views.RemoveAt(index);
                    this.listBoxSelectView.Items.RemoveAt(index);
                    break;

                default:
                    break;
            }

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorCancel.Enabled = false;

            this.comboBoxSensorID.Enabled = false;
            this.textBoxSensorPosX.Enabled = false;
            this.textBoxSensorPosY.Enabled = false;
            this.comboBoxSensorBank.Enabled = false;
            this.textBoxSensorShift.Enabled = false;
            this.textBoxSensorHigh.Enabled = false;
            this.textBoxSensorLow.Enabled = false;
            this.textBoxSensorAnalogUpper.Enabled = false;
            this.textBoxSensorAnalogLower.Enabled = false;
            this.labelSensorID.Enabled = false;
            this.labelSensorPosX.Enabled = false;
            this.labelSensorPosY.Enabled = false;
            this.labelSensorBank.Enabled = false;
            this.labelSensorShift.Enabled = false;
            this.labelSensorHigh.Enabled = false;
            this.labelSensorLow.Enabled = false;
            this.labelSensorAnalogUpper.Enabled = false;
            this.labelSensorAnalogLower.Enabled = false;

            this.buttonViewAdd.Enabled = true;
            this.buttonViewEdit.Enabled = (this.listBoxSelectView.SelectedIndex >= 0);
            this.buttonViewRemove.Enabled = (this.listBoxSelectView.SelectedIndex >= 0);
            this.buttonViewOK.Enabled = false;
            this.buttonViewCancel.Enabled = false;

            this.labelViewName.Enabled = false;
            this.labelViewImageLocation.Enabled = false;
            this.labelViewPreview.Enabled = false;
            this.textBoxViewName.Enabled = false;
            this.textBoxViewImageLocation.Enabled = false;
            this.pictureBoxPreview.Enabled = false;

            this.buttonLoadPreview.Enabled = false;
            this.buttonViewLocateImage.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.listBoxSelectView.Enabled = true;

            this.buttonClose.Enabled = true;

            this.viewEditState = EditState.None;
            this.sensorEditState = EditState.None;

            this.configChanged = true;
        }

        private void buttonViewCancel_Click(object sender, EventArgs e) {
            if (this.sensorEditState == EditState.Add) {
                this.listBoxSelectSensor.Items.RemoveAt(this.listBoxSelectSensor.Items.Count - 1);
            }
            if (this.viewEditState == EditState.Add) {
                this.listBoxSelectView.Items.RemoveAt(this.listBoxSelectView.Items.Count - 1);
            }

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorCancel.Enabled = false;

            this.comboBoxSensorID.Enabled = false;
            this.textBoxSensorPosX.Enabled = false;
            this.textBoxSensorPosY.Enabled = false;
            this.comboBoxSensorBank.Enabled = false;
            this.textBoxSensorShift.Enabled = false;
            this.textBoxSensorHigh.Enabled = false;
            this.textBoxSensorLow.Enabled = false;
            this.textBoxSensorAnalogUpper.Enabled = false;
            this.textBoxSensorAnalogLower.Enabled = false;
            this.labelSensorID.Enabled = false;
            this.labelSensorPosX.Enabled = false;
            this.labelSensorPosY.Enabled = false;
            this.labelSensorBank.Enabled = false;
            this.labelSensorShift.Enabled = false;
            this.labelSensorHigh.Enabled = false;
            this.labelSensorLow.Enabled = false;
            this.labelSensorAnalogUpper.Enabled = false;
            this.labelSensorAnalogLower.Enabled = false;

            this.buttonViewAdd.Enabled = true;
            this.buttonViewEdit.Enabled = (this.listBoxSelectView.SelectedIndex >= 0);
            this.buttonViewRemove.Enabled = (this.listBoxSelectView.SelectedIndex >= 0);
            this.buttonViewOK.Enabled = false;
            this.buttonViewCancel.Enabled = false;

            this.labelViewName.Enabled = false;
            this.labelViewImageLocation.Enabled = false;
            this.labelViewPreview.Enabled = false;
            this.textBoxViewName.Enabled = false;
            this.textBoxViewImageLocation.Enabled = false;
            this.pictureBoxPreview.Enabled = false;

            this.buttonLoadPreview.Enabled = false;
            this.buttonViewLocateImage.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.listBoxSelectView.Enabled = true;

            this.buttonClose.Enabled = true;

            this.viewEditState = EditState.None;
            this.sensorEditState = EditState.None;
        }

        private void buttonSensorAdd_Click(object sender, EventArgs e) {
            int index = this.listBoxSelectSensor.Items.Add("<New Entry>");
            this.listBoxSelectSensor.SelectedIndex = index;
            this.listBoxSelectSensor.Enabled = false;

            this.comboBoxSensorID.Text = string.Empty;
            this.textBoxSensorPosX.Text = string.Empty;
            this.textBoxSensorPosY.Text = string.Empty;
            this.comboBoxSensorBank.Text = string.Empty;
            this.textBoxSensorShift.Text = string.Empty;
            this.textBoxSensorHigh.Text = string.Empty;
            this.textBoxSensorLow.Text = string.Empty;
            this.textBoxSensorAnalogUpper.Text = string.Empty;
            this.textBoxSensorAnalogLower.Text = string.Empty;

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = true;
            this.buttonSensorCancel.Enabled = true;

            this.comboBoxSensorID.Enabled = true;
            this.textBoxSensorPosX.Enabled = true;
            this.textBoxSensorPosY.Enabled = true;
            this.comboBoxSensorBank.Enabled = true;
            this.textBoxSensorShift.Enabled = true;
            this.textBoxSensorHigh.Enabled = true;
            this.textBoxSensorLow.Enabled = true;
            this.textBoxSensorAnalogUpper.Enabled = true;
            this.textBoxSensorAnalogLower.Enabled = true;
            this.labelSensorID.Enabled = true;
            this.labelSensorPosX.Enabled = true;
            this.labelSensorPosY.Enabled = true;
            this.labelSensorBank.Enabled = true;
            this.labelSensorShift.Enabled = true;
            this.labelSensorHigh.Enabled = true;
            this.labelSensorLow.Enabled = true;
            this.labelSensorAnalogUpper.Enabled = true;
            this.labelSensorAnalogLower.Enabled = true;

            this.buttonClose.Enabled = false;

            this.sensorEditState = EditState.Add;
        }

        private void buttonSensorEdit_Click(object sender, EventArgs e) {
            this.listBoxSelectSensor.Enabled = false;

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = true;
            this.buttonSensorCancel.Enabled = true;

            this.comboBoxSensorID.Enabled = true;
            this.textBoxSensorPosX.Enabled = true;
            this.textBoxSensorPosY.Enabled = true;
            this.comboBoxSensorBank.Enabled = true;
            this.textBoxSensorShift.Enabled = true;
            this.textBoxSensorHigh.Enabled = true;
            this.textBoxSensorLow.Enabled = true;
            this.textBoxSensorAnalogUpper.Enabled = true;
            this.textBoxSensorAnalogLower.Enabled = true;
            this.labelSensorID.Enabled = true;
            this.labelSensorPosX.Enabled = true;
            this.labelSensorPosY.Enabled = true;
            this.labelSensorBank.Enabled = true;
            this.labelSensorShift.Enabled = true;
            this.labelSensorHigh.Enabled = true;
            this.labelSensorLow.Enabled = true;
            this.labelSensorAnalogUpper.Enabled = true;
            this.labelSensorAnalogLower.Enabled = true;

            this.buttonClose.Enabled = false;

            this.sensorEditState = EditState.Edit;
        }

        private void buttonSensorRemove_Click(object sender, EventArgs e) {
            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = true;
            this.buttonSensorCancel.Enabled = true;

            this.buttonClose.Enabled = false;

            this.sensorEditState = EditState.Remove;
        }

        private void buttonSensorOK_Click(object sender, EventArgs e) {

            string id = this.comboBoxSensorID.Text;
            if (string.IsNullOrEmpty(id)) {
                MessageBox.Show(
                    $"Missing field ID!",
                    "Missing field!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            if (!double.TryParse(this.textBoxSensorPosX.Text, out double posX)) {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Position X to numeric value: {this.textBoxSensorPosX.Text}",
                    "Conversion failed!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            // Percent -> decimal value
            posX = posX * 0.01;

            if (!double.TryParse(this.textBoxSensorPosY.Text, out double posY)) {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Position Y to numeric value: {this.textBoxSensorPosY.Text}",
                    "Conversion failed!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            // Percent -> decimal value
            posY = posY * 0.01;

            int? bank;
            if (this.comboBoxSensorBank.Text == string.Empty) {
                bank = null;
            } else if (int.TryParse(this.comboBoxSensorBank.Text, out int temp)) {
                bank = temp;
            } else {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Bank to numeric value: {this.comboBoxSensorBank.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            double? shift;
            if (this.textBoxSensorShift.Text == string.Empty) {
                shift = null;
            } else if (double.TryParse(this.textBoxSensorShift.Text, out double temp)) {
                shift = temp;
            } else {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Zero Shift Target to numeric value: {this.textBoxSensorShift.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            double? high;
            if (this.textBoxSensorHigh.Text == string.Empty) {
                high = null;
            } else if (double.TryParse(this.textBoxSensorHigh.Text, out double temp)) {
                high = temp;
            } else {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert HIGH threshold to numeric value: {this.textBoxSensorHigh.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            double? low;
            if (this.textBoxSensorLow.Text == string.Empty) {
                low = null;
            } else if (double.TryParse(this.textBoxSensorLow.Text, out double temp)) {
                low = temp;
            } else {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert LOW threshold to numeric value: {this.textBoxSensorLow.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            double? analog_upper;
            if (this.textBoxSensorAnalogUpper.Text == string.Empty) {
                analog_upper = null;
            } else if (double.TryParse(this.textBoxSensorAnalogUpper.Text, out double temp)) {
                analog_upper = temp;
            } else {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Analog Upper Limit to numeric value: {this.textBoxSensorAnalogUpper.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            double? analog_lower;
            if (this.textBoxSensorAnalogLower.Text == string.Empty) {
                analog_lower = null;
            } else if (double.TryParse(this.textBoxSensorAnalogLower.Text, out double temp)) {
                analog_lower = temp;
            } else {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Analog Lower Limit to numeric value: {this.textBoxSensorAnalogLower.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            SensorUsageConfig sensor;
            int index = this.listBoxSelectSensor.SelectedIndex;

            switch (this.sensorEditState) {
                case EditState.Add:
                    // Update Listbox and add to Config
                    sensor = new() {
                        ID = id,
                        PositionX = posX,
                        PositionY = posY,
                        Bank = bank,
                        ShiftTarget = shift,
                        HighThreshold = high,
                        LowThreshold = low,
                        AnalogUpperBound = analog_upper,
                        AnalogLowerBound = analog_lower
                    };
                    this.sensors.Add(sensor);
                    this.listBoxSelectSensor.Items[index] = id;
                    break;

                case EditState.Edit:
                    // Update Listbox and change in config
                    sensor = this.sensors[index];
                    sensor.ID = id;
                    sensor.PositionX = posX;
                    sensor.PositionY = posY;
                    sensor.Bank = bank;
                    sensor.ShiftTarget = shift;
                    sensor.HighThreshold = high;
                    sensor.LowThreshold = low;
                    sensor.AnalogUpperBound = analog_upper;
                    sensor.AnalogLowerBound = analog_lower;
                    this.listBoxSelectSensor.Items[index] = id;
                    break;

                case EditState.Remove:
                    // Update Listbox and remove from config
                    this.sensors.RemoveAt(index);
                    this.listBoxSelectSensor.Items.RemoveAt(index);
                    break;

                default:
                    break;
            }

            this.buttonSensorAdd.Enabled = true;
            this.buttonSensorEdit.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);
            this.buttonSensorRemove.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorCancel.Enabled = false;

            this.comboBoxSensorID.Enabled = false;
            this.textBoxSensorPosX.Enabled = false;
            this.textBoxSensorPosY.Enabled = false;
            this.comboBoxSensorBank.Enabled = false;
            this.textBoxSensorShift.Enabled = false;
            this.textBoxSensorHigh.Enabled = false;
            this.textBoxSensorLow.Enabled = false;
            this.textBoxSensorAnalogUpper.Enabled = false;
            this.textBoxSensorAnalogLower.Enabled = false;
            this.labelSensorID.Enabled = false;
            this.labelSensorPosX.Enabled = false;
            this.labelSensorPosY.Enabled = false;
            this.labelSensorBank.Enabled = false;
            this.labelSensorShift.Enabled = false;
            this.labelSensorHigh.Enabled = false;
            this.labelSensorLow.Enabled = false;
            this.labelSensorAnalogUpper.Enabled = false;
            this.labelSensorAnalogLower.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.sensorEditState = EditState.None;
        }

        private void buttonSensorCancel_Click(object sender, EventArgs e) {
            if (this.sensorEditState == EditState.Add) {
                this.listBoxSelectSensor.Items.RemoveAt(this.listBoxSelectSensor.Items.Count - 1);
            }

            this.buttonSensorAdd.Enabled = true;
            this.buttonSensorEdit.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);
            this.buttonSensorRemove.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorCancel.Enabled = false;

            this.comboBoxSensorID.Enabled = false;
            this.textBoxSensorPosX.Enabled = false;
            this.textBoxSensorPosY.Enabled = false;
            this.comboBoxSensorBank.Enabled = false;
            this.textBoxSensorShift.Enabled = false;
            this.textBoxSensorHigh.Enabled = false;
            this.textBoxSensorLow.Enabled = false;
            this.textBoxSensorAnalogUpper.Enabled = false;
            this.textBoxSensorAnalogLower.Enabled = false;
            this.labelSensorID.Enabled = false;
            this.labelSensorPosX.Enabled = false;
            this.labelSensorPosY.Enabled = false;
            this.labelSensorBank.Enabled = false;
            this.labelSensorShift.Enabled = false;
            this.labelSensorHigh.Enabled = false;
            this.labelSensorLow.Enabled = false;
            this.labelSensorAnalogUpper.Enabled = false;
            this.labelSensorAnalogLower.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.sensorEditState = EditState.None;
        }

        private void listBoxSelectView_SelectedIndexChanged(object sender, EventArgs e) {
            this.fillView();
        }

        private void listBoxSelectSensor_SelectedIndexChanged(object sender, EventArgs e) {
            this.fillSensor();
        }

        private void ViewEditor_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.configChanged) {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void textBoxSensorPosition_TextChanged(object sender, EventArgs e) {
            if (((TextBox)sender).Enabled) {
                this.adjustSensorPosition();
            }
        }

        private void moveSensorLocation(int X, int Y) {
            double posX;
            double posY;

            PictureBox pb = this.pictureBoxPreview;
            Image img = pb.Image;

            if (img == null)
                return;

            double wfactor = (double)img.Width / pb.ClientSize.Width;
            double hfactor = (double)img.Height / pb.ClientSize.Height;

            double resizeFactor = Math.Max(wfactor, hfactor);
            Size imageSize = new(
                (int)(img.Width / resizeFactor),
                (int)(img.Height / resizeFactor)
            );
            int offsetX = (pb.Width - imageSize.Width) / 2;
            int offsetY = (pb.Height - imageSize.Height) / 2;

            posX = double.Round((double)(X - offsetX) * 100.0 / imageSize.Width, 3);
            posY = double.Round((double)(Y - offsetY) * 100.0 / imageSize.Height, 3);

            this.pictureBoxSensorLocation.Location = new Point(
                (int)(offsetX + posX * 0.01 * imageSize.Width - (this.pictureBoxSensorLocation.Width / 2)),
                (int)(offsetY + posY * 0.01 * imageSize.Height - (this.pictureBoxSensorLocation.Height / 2))
            );

            this.textBoxSensorPosX.Text = posX.ToString();
            this.textBoxSensorPosY.Text = posY.ToString();
        }

        private void pictureBoxPreview_MouseDown(object sender, MouseEventArgs e) {
            if (
                (e.Button == MouseButtons.Left)
                && this.pictureBoxSensorLocation.Visible
                && this.textBoxSensorPosX.Enabled
                && this.textBoxSensorPosY.Enabled
            ) {
                this.moveSensorLocation(e.X, e.Y);
            }
        }

        private void pictureBoxSensorLocation_MouseDown(object sender, MouseEventArgs e) {
            if (
                (e.Button == MouseButtons.Left)
                && this.pictureBoxSensorLocation.Visible
                && this.textBoxSensorPosX.Enabled
                && this.textBoxSensorPosY.Enabled
            ) {
                Point parent = (sender as Control)!.Location;
                this.moveSensorLocation(e.X + parent.X, e.Y + parent.Y);
            }
        }

        private void pictureBoxPreview_SizeChanged(object sender, EventArgs e) {
            this.adjustSensorPosition();
        }
    }
}
