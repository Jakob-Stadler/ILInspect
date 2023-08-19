using System.Data;

namespace ILInspect {
    public partial class ConnectionEditor : Form {
        private enum EditState {
            None,
            Add,
            Edit,
            Remove
        }

        private JSONConfig config;
        private IList<SensorConfig> sensors;
        private EditState connectionEditState;
        private EditState sensorEditState;
        private bool configChanged = false;

        public ConnectionEditor(JSONConfig config) {
            this.InitializeComponent();
            this.comboBoxSensorDecimalPlaces.Items.AddRange(new object[] {
                (0.001).ToString(),
                (0.01).ToString(),
                (0.1).ToString(),
                (1.0).ToString(),
            });
            this.config = config;
            this.sensors = new List<SensorConfig>();
            this.connectionEditState = EditState.None;
            this.sensorEditState = EditState.None;
            this.fillFields();
        }

        private void fillFields() {
            this.listBoxSelectConnection.Items.AddRange(
                (from conn in this.config.SensorConnections select $"{conn.Host}:{conn.Port}").ToArray()
            );
        }

        private void fillConnection() {
            int index = this.listBoxSelectConnection.SelectedIndex;
            if (index < 0) {
                // Clear fields
                this.textBoxConnectionHost.Text = string.Empty;
                this.textBoxConnectionPort.Text = string.Empty;
                this.checkBoxConnectionLaserStop.Checked = false;
                this.textBoxConnectionLaserDelay.Text = string.Empty;
                this.listBoxSelectSensor.Items.Clear();
                this.sensors = new List<SensorConfig>();
                this.buttonConnectionAdd.Enabled = true;
                this.buttonConnectionEdit.Enabled = false;
                this.buttonConnectionRemove.Enabled = false;

            } else if (index < this.config.SensorConnections.Count) {
                ConnectionConfig connectionConfig = this.config.SensorConnections[index];
                this.textBoxConnectionHost.Text = connectionConfig.Host;
                this.textBoxConnectionPort.Text = connectionConfig.Port.ToString();
                this.checkBoxConnectionLaserStop.Checked = connectionConfig.StopLasers;
                this.textBoxConnectionLaserDelay.Text = connectionConfig.LaserTimeout.ToString();
                this.listBoxSelectSensor.Items.Clear();
                this.sensors = connectionConfig.Sensors;
                this.listBoxSelectSensor.Items.AddRange(
                    (from sensor in this.sensors select $"{sensor.ID}").ToArray()
                );
                this.buttonConnectionAdd.Enabled = true;
                this.buttonConnectionEdit.Enabled = true;
                this.buttonConnectionRemove.Enabled = true;
            } else {
                // Connection not currently in config
            }
        }

        private void fillSensor() {
            int index = this.listBoxSelectSensor.SelectedIndex;
            if (index < 0) {
                // Clear fields
                this.textBoxSensorID.Text = string.Empty;
                this.comboBoxSensorDecimalPlaces.Text = string.Empty;
                this.buttonSensorAdd.Enabled = true;
                this.buttonSensorEdit.Enabled = false;
                this.buttonSensorRemove.Enabled = false;

            } else if (index < this.sensors.Count) {
                SensorConfig sensorConfig = this.sensors![index];
                this.textBoxSensorID.Text = sensorConfig.ID;
                this.comboBoxSensorDecimalPlaces.Text = sensorConfig.ConversionFactor.ToString();

                if (this.connectionEditState == EditState.Add || this.connectionEditState == EditState.Edit) {
                    this.buttonSensorAdd.Enabled = true;
                    this.buttonSensorEdit.Enabled = true;
                    this.buttonSensorRemove.Enabled = true;
                }
            } else {
                // Sensor not currently in config
            }
        }

        private void buttonConnectionAdd_Click(object sender, EventArgs e) {
            int index = this.listBoxSelectConnection.Items.Add("<New Entry>");
            this.listBoxSelectConnection.SelectedIndex = index;
            this.listBoxSelectConnection.Enabled = false;

            this.textBoxConnectionHost.Text = string.Empty;
            this.textBoxConnectionPort.Text = string.Empty;
            this.checkBoxConnectionLaserStop.Checked = false;
            this.textBoxConnectionLaserDelay.Text = string.Empty;
            this.listBoxSelectSensor.Items.Clear();
            this.sensors = new List<SensorConfig>();

            this.buttonConnectionAdd.Enabled = false;
            this.buttonConnectionEdit.Enabled = false;
            this.buttonConnectionRemove.Enabled = false;
            this.buttonConnectionOK.Enabled = true;
            this.buttonConnectionCancel.Enabled = true;

            this.labelConnectionHost.Enabled = true;
            this.labelConnectionPort.Enabled = true;
            this.labelConnectionLaserDelay.Enabled = true;
            this.textBoxConnectionHost.Enabled = true;
            this.textBoxConnectionPort.Enabled = true;
            this.textBoxConnectionLaserDelay.Enabled = true;
            this.checkBoxConnectionLaserStop.Enabled = true;

            this.buttonSensorAdd.Enabled = true;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;

            this.buttonClose.Enabled = false;

            this.connectionEditState = EditState.Add;
            this.sensorEditState = EditState.None;
        }

        private void buttonConnectionEdit_Click(object sender, EventArgs e) {
            this.listBoxSelectConnection.Enabled = false;

            this.buttonConnectionAdd.Enabled = false;
            this.buttonConnectionEdit.Enabled = false;
            this.buttonConnectionRemove.Enabled = false;
            this.buttonConnectionOK.Enabled = true;
            this.buttonConnectionCancel.Enabled = true;

            // Clone sensor list, don't work with direct reference
            this.sensors = (
                from sensor in this.sensors select (SensorConfig)sensor.Clone()
            ).ToList();

            this.labelConnectionHost.Enabled = true;
            this.labelConnectionPort.Enabled = true;
            this.labelConnectionLaserDelay.Enabled = true;
            this.textBoxConnectionHost.Enabled = true;
            this.textBoxConnectionPort.Enabled = true;
            this.textBoxConnectionLaserDelay.Enabled = true;
            this.checkBoxConnectionLaserStop.Enabled = true;

            this.buttonSensorAdd.Enabled = true;
            this.buttonSensorEdit.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);
            this.buttonSensorRemove.Enabled = (this.listBoxSelectSensor.SelectedIndex >= 0);

            this.buttonClose.Enabled = false;

            this.connectionEditState = EditState.Edit;
            this.sensorEditState = EditState.None;
        }

        private void buttonConnectionRemove_Click(object sender, EventArgs e) {
            this.buttonConnectionAdd.Enabled = false;
            this.buttonConnectionEdit.Enabled = false;
            this.buttonConnectionRemove.Enabled = false;
            this.buttonConnectionOK.Enabled = true;
            this.buttonConnectionCancel.Enabled = true;

            this.buttonClose.Enabled = false;

            this.connectionEditState = EditState.Remove;
            this.sensorEditState = EditState.None;
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void buttonConnectionOK_Click(object sender, EventArgs e) {

            string host = this.textBoxConnectionHost.Text;
            if (string.IsNullOrEmpty(host)) {
                MessageBox.Show(
                    $"Missing field Host!",
                    "Missing field!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            bool laserStop = this.checkBoxConnectionLaserStop.Checked;
            if (!int.TryParse(this.textBoxConnectionPort.Text, out int port)) {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Port to numeric value: {this.textBoxConnectionPort.Text}",
                    "Conversion failed!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            int? laserDelay;
            if (this.textBoxConnectionLaserDelay.Text == string.Empty) {
                laserDelay = null;
            } else if (!int.TryParse(this.textBoxConnectionLaserDelay.Text, out int tempLaserDelay)) {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Laser Delay to numeric value: {this.textBoxConnectionLaserDelay.Text}",
                    "Conversion failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            } else {
                laserDelay = tempLaserDelay;
            }

            ConnectionConfig conn;
            int index = this.listBoxSelectConnection.SelectedIndex;

            switch (this.connectionEditState) {
                case EditState.Add:
                    // Update Listbox and add to Config
                    conn = new() {
                        Host = host,
                        Port = port,
                        StopLasers = laserStop,
                        Sensors = this.sensors
                    };
                    if (laserDelay.HasValue) {
                        conn.LaserTimeout = laserDelay.Value;
                    }
                    this.config.SensorConnections.Add(conn);
                    this.listBoxSelectConnection.Items[index] = $"{conn.Host}:{conn.Port}";
                    break;
                case EditState.Edit:
                    // Update Listbox and change in config
                    conn = this.config.SensorConnections[index];
                    conn.Host = host;
                    conn.Port = port;
                    conn.StopLasers = laserStop;
                    if (laserDelay.HasValue) {
                        conn.LaserTimeout = laserDelay.Value;
                    } else {
                        conn.LaserTimeout = 1000;
                    }
                    conn.Sensors = this.sensors;

                    this.listBoxSelectConnection.Items[index] = $"{conn.Host}:{conn.Port}";

                    break;
                case EditState.Remove:
                    // Update Listbox and remove from config
                    this.config.SensorConnections.RemoveAt(index);
                    this.listBoxSelectConnection.Items.RemoveAt(index);
                    break;
                default:
                    break;
            }

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorCancel.Enabled = false;

            this.labelSensorID.Enabled = false;
            this.labelSensorDecimals.Enabled = false;
            this.textBoxSensorID.Enabled = false;
            this.comboBoxSensorDecimalPlaces.Enabled = false;

            this.buttonConnectionAdd.Enabled = true;
            this.buttonConnectionEdit.Enabled = (this.listBoxSelectConnection.SelectedIndex >= 0);
            this.buttonConnectionRemove.Enabled = (this.listBoxSelectConnection.SelectedIndex >= 0);
            this.buttonConnectionOK.Enabled = false;
            this.buttonConnectionCancel.Enabled = false;

            this.labelConnectionHost.Enabled = false;
            this.labelConnectionPort.Enabled = false;
            this.labelConnectionLaserDelay.Enabled = false;
            this.textBoxConnectionHost.Enabled = false;
            this.textBoxConnectionPort.Enabled = false;
            this.textBoxConnectionLaserDelay.Enabled = false;
            this.checkBoxConnectionLaserStop.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.listBoxSelectConnection.Enabled = true;

            this.buttonClose.Enabled = true;

            this.connectionEditState = EditState.None;
            this.sensorEditState = EditState.None;

            this.configChanged = true;
        }

        private void buttonConnectionCancel_Click(object sender, EventArgs e) {
            if (this.sensorEditState == EditState.Add) {
                this.listBoxSelectSensor.Items.RemoveAt(this.listBoxSelectSensor.Items.Count - 1);
            }
            if (this.connectionEditState == EditState.Add) {
                this.listBoxSelectConnection.Items.RemoveAt(this.listBoxSelectConnection.Items.Count - 1);
            }

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = false;
            this.buttonSensorCancel.Enabled = false;

            this.labelSensorID.Enabled = false;
            this.labelSensorDecimals.Enabled = false;
            this.textBoxSensorID.Enabled = false;
            this.comboBoxSensorDecimalPlaces.Enabled = false;

            this.buttonConnectionAdd.Enabled = true;
            this.buttonConnectionEdit.Enabled = (this.listBoxSelectConnection.SelectedIndex >= 0);
            this.buttonConnectionRemove.Enabled = (this.listBoxSelectConnection.SelectedIndex >= 0);
            this.buttonConnectionOK.Enabled = false;
            this.buttonConnectionCancel.Enabled = false;

            this.labelConnectionHost.Enabled = false;
            this.labelConnectionPort.Enabled = false;
            this.labelConnectionLaserDelay.Enabled = false;
            this.textBoxConnectionHost.Enabled = false;
            this.textBoxConnectionPort.Enabled = false;
            this.textBoxConnectionLaserDelay.Enabled = false;
            this.checkBoxConnectionLaserStop.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.listBoxSelectConnection.Enabled = true;

            this.buttonClose.Enabled = true;

            this.connectionEditState = EditState.None;
            this.sensorEditState = EditState.None;
        }

        private void buttonSensorAdd_Click(object sender, EventArgs e) {
            int index = this.listBoxSelectSensor.Items.Add("<New Entry>");
            this.listBoxSelectSensor.SelectedIndex = index;
            this.listBoxSelectSensor.Enabled = false;

            this.textBoxSensorID.Text = string.Empty;
            this.comboBoxSensorDecimalPlaces.Text = string.Empty;

            this.buttonSensorAdd.Enabled = false;
            this.buttonSensorEdit.Enabled = false;
            this.buttonSensorRemove.Enabled = false;
            this.buttonSensorOK.Enabled = true;
            this.buttonSensorCancel.Enabled = true;

            this.labelSensorID.Enabled = true;
            this.labelSensorDecimals.Enabled = true;
            this.textBoxSensorID.Enabled = true;
            this.comboBoxSensorDecimalPlaces.Enabled = true;

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

            this.labelSensorID.Enabled = true;
            this.labelSensorDecimals.Enabled = true;
            this.textBoxSensorID.Enabled = true;
            this.comboBoxSensorDecimalPlaces.Enabled = true;

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

            string id = this.textBoxSensorID.Text;
            if (string.IsNullOrEmpty(id)) {
                MessageBox.Show(
                    $"Missing field ID!",
                    "Missing field!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            if (!double.TryParse(this.comboBoxSensorDecimalPlaces.Text, out double decimals)) {
                // Conversion failed
                MessageBox.Show(
                    $"Failed to convert Decimal Places to numeric value: {this.comboBoxSensorDecimalPlaces.Text}",
                    "Conversion failed!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            SensorConfig sensor;
            int index = this.listBoxSelectSensor.SelectedIndex;

            switch (this.sensorEditState) {
                case EditState.Add:
                    // Update Listbox and add to Config
                    sensor = new() {
                        ID = id,
                        ConversionFactor = decimals
                    };
                    this.sensors.Add(sensor);
                    this.listBoxSelectSensor.Items[index] = id;
                    break;

                case EditState.Edit:
                    // Update Listbox and change in config
                    sensor = this.sensors[index];
                    sensor.ID = id;
                    sensor.ConversionFactor = decimals;
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

            this.labelSensorID.Enabled = false;
            this.labelSensorDecimals.Enabled = false;
            this.textBoxSensorID.Enabled = false;
            this.comboBoxSensorDecimalPlaces.Enabled = false;

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

            this.labelSensorID.Enabled = false;
            this.labelSensorDecimals.Enabled = false;
            this.textBoxSensorID.Enabled = false;
            this.comboBoxSensorDecimalPlaces.Enabled = false;

            this.listBoxSelectSensor.Enabled = true;
            this.sensorEditState = EditState.None;
        }

        private void listBoxSelectConnection_SelectedIndexChanged(object sender, EventArgs e) {
            this.fillConnection();
        }

        private void listBoxSelectSensor_SelectedIndexChanged(object sender, EventArgs e) {
            this.fillSensor();
        }

        private void ConnectionEditor_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.configChanged) {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
