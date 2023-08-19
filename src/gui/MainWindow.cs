using System.Collections.Concurrent;
using System.Data;
using System.Net.Sockets;
using System.Text.Json;

namespace ILInspect {

    public partial class MainWindow : Form {
        private JSONConfig config = JSONConfig.createDefault();
        private Dictionary<string, SensorBox> sensorBoxes = new();
        private List<Connection> connections = new();
        private Dictionary<string, SensorProxy> proxies = new();
        private Dictionary<string, ConnectionContextMenu> connectionContextMenus = new();
        private Database? database = null;
        private string filter = string.Empty;
        private MessageCollection messageCollection;
        private bool connectionInitialized = false;

        public MainWindow(string configPath, MessageCollection messageCollection) {
            this.InitializeComponent();
            this.toolStripMenuItemAboutAbout.Text = $"About {this.Text}";

            this.messageCollection = messageCollection;
            messageCollection.control = this.textBoxMessages; // Invokation Synchronization
            Binding binding = new("Text", messageCollection, "Text", false, DataSourceUpdateMode.OnPropertyChanged);
            this.textBoxMessages.DataBindings.Add(binding);

            // Hide Messages by default
            this.splitContainerMessages.Panel2Collapsed = true;

            this.loadConfig(configPath);
        }

        public void saveConfig(string configPath) {
            string jsonContents;
            JsonSerializerOptions options = new() {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            try {
                jsonContents = JsonSerializer.Serialize(this.config, options);
            }
            catch (JsonException ex) {
                string message = (
                    "Failed to serialize config to JSON!\r\n" +
                    $"{ex.Message}"
                );
                this.messageCollection.addLine(message);
                MessageBox.Show(
                    message,
                    "Configuration Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            try {
                File.WriteAllText(configPath, jsonContents);
            }
            catch (Exception ex) {
                string message = (
                    $"Failed to save config to file {configPath}\r\n" +
                    $"{ex.Message}"
                );
                this.messageCollection.addLine(message);
                MessageBox.Show(
                    message,
                    "Configuration Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
        }

        public void loadConfig(string configPath) {
            string jsonContents;
            try {
                jsonContents = File.ReadAllText(configPath);
            }
            catch (Exception ex) {
                string message = (
                    $"Failed to load file {configPath}\r\n" +
                    $"{ex.Message}\r\n" +
                    "Keeping old configuration!"
                );
                this.messageCollection.addLine(message);
                MessageBox.Show(
                    message,
                    "File Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            JsonSerializerOptions options = new() {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            try {
                this.config = JsonSerializer.Deserialize<JSONConfig>(jsonContents, options)!;
            }
            catch (JsonException ex) {
                string message = (
                    $"Failed to load config from file {configPath}\r\n" +
                    $"{ex.Message}\r\n" +
                    "Keeping old configuration!"
                );
                this.messageCollection.addLine(message);
                MessageBox.Show(
                    message,
                    "Configuration Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            this.config.ConfigDirectory = Path.GetDirectoryName(configPath);

            this.applyConfig();
        }

        private void applyConfig() {
            bool encounteredError = false;

            // Replace relative image paths
            foreach (ViewConfig viewConfig in this.config.Views) {
                string location = viewConfig.ImageLocation;
                location = Path.GetFullPath(location, this.config.ConfigDirectory ?? AppDomain.CurrentDomain.BaseDirectory);
                viewConfig.ImageLocation = location;
            }

            // Replace relative database path
            string source = this.config.Database.Source;
            if (source != ":memory:") {
                source = Path.GetFullPath(source, this.config.ConfigDirectory ?? AppDomain.CurrentDomain.BaseDirectory);
            }

            // Establish connections and create proxy objects for sensors
            this.connections = new();
            this.proxies = new();
            for (int connIndex = 0; connIndex < this.config.SensorConnections.Count; connIndex++) {
                ConnectionConfig conConfig = this.config.SensorConnections[connIndex];
                Connection conn = new(conConfig, this.messageCollection);
                this.connections.Add(conn);
                for (int sensorIndex = 0; sensorIndex < conConfig.Sensors.Count; sensorIndex++) {
                    SensorConfig sensorConfig = conConfig.Sensors[sensorIndex];
                    string boxId = sensorConfig.ID;
                    SensorProxy proxy = new(sensorConfig, sensorIndex + 1, conn, this.messageCollection);
                    if (!this.proxies.TryAdd(boxId, proxy)) {
                        // Failed to add proxy, possibly duplicate boxId
                        string message = (
                            $"Failed to add sensor_connections[{connIndex}]/sensors/id:\"{boxId}\"\r\n" +
                            $"IDs have to be unique, check the configuration to make sure no other sensor uses the same ID!"
                        );
                        this.messageCollection.addLine(message);
                        MessageBox.Show(
                            message,
                            "Sensor Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        encounteredError = true;
                        continue;
                    }
                    conn.addSensor(boxId, proxy);
                    if (conn.StopLaser) {
                        proxy.sendSetLaserEmissionStop(1);
                    }
                }
            }
            this.createSubmenus();

            // Populate Views
            this.comboBoxView.Items.Clear();
            this.comboBoxView.SelectedIndex = -1;
            foreach (ViewConfig view in this.config.Views ?? Enumerable.Empty<ViewConfig>()) {
                this.comboBoxView.Items.Add(view.Name);
            }
            if (this.comboBoxView.Items.Count > 0) {
                if (this.Visible)
                    this.comboBoxView.SelectedIndex = 0;
            } else {
                this.changeView(-1);
            }

            // Connect to Database
            this.database = new(source, this.messageCollection);
            try {
                this.database.initTable();
            }
            catch (Exception ex) {
                string message = (
                    "Encountered exception when trying to access database!\r\n" +
                    $"Check on status of Read/Write/Create permissions of file {source}\r\n" +
                    $"{ex.Message}\r\n" +
                    "Execution will continue without database!"
                );
                this.messageCollection.addLine(message);
                MessageBox.Show(
                    message,
                    "Database Error!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                encounteredError = true;
                this.database = null;
            }

            // Make sure database columns are populated
            foreach (string sensorId in this.proxies.Keys) {
                this.database?.addColumn(sensorId);
            }

            // Display Database contents in DataGridView
            if (this.database != null) {
                this.loadDataTable();
            } else {
                this.dataGridViewDB.DataSource = null;
            }

            if (encounteredError) {
                this.labelStatus.Text = "Error while loading config, check messages!";
            } else {
                this.labelStatus.Text = "Config loaded successfully.";
            }
        }

        private void createSubmenus() {
            this.contextMenuConnections.Items.Clear();
            this.contextMenuConnections.SuspendLayout();
            for (int i = 0; i < this.connections.Count; i++) {
                Connection? connection = this.connections[i];

                ConnectionContextMenu connCtxMenuContainer;
                connCtxMenuContainer = new(connection, this.messageCollection);
                connCtxMenuContainer.Parent = this;

                ContextMenuStrip connCtxMenu;
                connCtxMenu = connCtxMenuContainer.contextMenuCommunication;

                ToolStripMenuItem connItem;
                connItem = new();
                string address = $" ({this.config.SensorConnections[i].Host}:{this.config.SensorConnections[i].Port})";
                connItem.Text = $"Connection {i + 1}{address}";
                connItem.DropDown = connCtxMenu;
                this.contextMenuConnections.Items.Add(connItem);

                ContextMenuStrip contextMenuSensors;
                contextMenuSensors = connCtxMenuContainer.contextMenuCommSensors;
                contextMenuSensors.SuspendLayout();
                foreach (SensorProxy proxy in connection.proxies.Values) {

                    SensorContextMenu sensorContextMenuContainer;
                    sensorContextMenuContainer = new(null, proxy, this.messageCollection);
                    sensorContextMenuContainer.Parent = this;

                    this.connectionContextMenus[proxy.config.ID] = connCtxMenuContainer;

                    ToolStripMenuItem sensorItem;
                    sensorItem = new();
                    sensorItem.Text = $"Sensor {proxy.config.ID}";
                    sensorItem.DropDown = sensorContextMenuContainer.contextMenu;
                    contextMenuSensors.Items.Add(sensorItem);
                }
                contextMenuSensors.ResumeLayout(false);
                contextMenuSensors.PerformLayout();
            }
            this.contextMenuConnections.ResumeLayout(false);
            this.contextMenuConnections.PerformLayout();
        }


        public void insertData(string view, string unitId, IDictionary<string, string> data) {
            this.database?.insertData(view, unitId, data);
        }

        private void updateAllBoxes(Action<Exception> excHandler) {
            ConcurrentDictionary<string, string> updateDict = new();
            CountdownEvent countdown = new(1);
            foreach (Connection connection in this.connections) {
                countdown.AddCount();
                Action<Exception> modifiedExcHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        connection.printSocketException((SocketException)ex);
                    }
                    excHandler(ex);
                    countdown.Signal();
                };

                Action afterUpdateCallback = () => {
                    foreach (KeyValuePair<string, SensorProxy> proxypair in connection.proxies) {
                        string boxId = proxypair.Key;
                        SensorProxy proxy = proxypair.Value;
                        int status = proxy.status;
                        double? value = proxy.value;
                        string strValue;
                        SensorBox? sensorBox;
                        if (this.sensorBoxes.TryGetValue(boxId, out sensorBox)) {
                            if (value == null) {
                                value = 99999.999;
                            } else {
                                value = value * proxy.config.ConversionFactor;
                            }
                            string valueFormat;
                            int numberZeros = (int)Math.Round(Math.Log10(1 / proxy.config.ConversionFactor), MidpointRounding.ToPositiveInfinity);
                            if (numberZeros == 0) {
                                valueFormat = $"{{0:+#0;-#0;0}}";
                            } else {
                                string zeros = new('0', numberZeros);
                                valueFormat = $"{{0:+#0.{zeros};-#0.{zeros};0}}";
                            }
                            switch (status) {
                                case 4:  // GO
                                    sensorBox.makeGreen();
                                    strValue = string.Format(valueFormat, value);
                                    break;
                                case 1:  // HIGH
                                case 2:  // LOW
                                    sensorBox.makeRed();
                                    strValue = string.Format(valueFormat, value);
                                    break;
                                case 3:  // Error
                                    sensorBox.makeGrey();
                                    strValue = "ERROR";
                                    break;
                                case 0:  // Off
                                default:
                                    sensorBox.makeGrey();
                                    strValue = "- - - -";
                                    break;
                            }
                            sensorBox.setMeasurementText(strValue);
                            updateDict.TryAdd(boxId, strValue);
                        }
                    }
                    countdown.Signal();
                };

                connection.updateAllMS(afterUpdateCallback, modifiedExcHandler);
            }
            countdown.Signal();
            countdown.Wait();
            this.Invoke(new Action(() => {
                this.insertData(this.comboBoxView.Text, this.textBoxUnit.Text, updateDict);
                this.loadDataTable();
            }));
        }

        private void buttonMeasure_Click(object sender, EventArgs e) {
            if (!this.backgroundWorkerMeasure.IsBusy) {
                this.backgroundWorkerMeasure.RunWorkerAsync();
            }
        }

        private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e) {
            this.changeView(this.comboBoxView.SelectedIndex);
        }

        private void changeView(int index) {
            Image? image;
            if (index == -1) {
                // Clear View
                image = null;
            } else {
                if (index >= (this.config.Views.Count)) {
                    this.printDebugMessage($"View Index {index} does not exist!");
                    return;
                }
                ViewConfig viewConfig = this.config.Views[index];
                image = viewConfig.Image;
                if (image == null) {
                    string relativePath = viewConfig.ImageLocation;
                    string fullPath = Path.GetFullPath(relativePath, this.config.ConfigDirectory ?? AppDomain.CurrentDomain.BaseDirectory);
                    try {
                        image = Image.FromFile(fullPath);
                        // Cache image
                        viewConfig.Image = image;
                    }
                    catch (Exception ex) {
                        string message = (
                            $"Failed to load image from file {fullPath}\r\n" +
                            $"{ex}\r\n" +
                            "Execution will continue without image!"
                        );
                        this.messageCollection.addLine(message);
                        MessageBox.Show(
                            message,
                            "Image Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        image = Properties.Resources.Warning;
                    }
                }
            }
            this.pictureBoxViewImage.Image = image;
            if (this.Visible) {
                if (!this.backgroundWorkerInitSensors.IsBusy) {
                    this.backgroundWorkerInitSensors.RunWorkerAsync(index);
                }
            }
            this.adjustFilter();
        }

        private void createSensorBoxes(int index) {
            this.pictureBoxViewImage.Invoke(new Action(() => {
                if (index == -1) {
                    // Remove old sensorBox controls
                    foreach (SensorBox sensorbox in this.sensorBoxes.Values) {
                        sensorbox.contextMenus.config = null;
                        sensorbox.Dispose();
                    }
                    this.sensorBoxes.Clear();
                    return;
                }

                if (index >= (this.config.Views.Count)) {
                    this.printDebugMessage($"View Index {index} does not exist!");
                    return;
                }
                ViewConfig? view = this.config.Views[index];
                // Remove old sensorBox controls
                foreach (SensorBox sensorbox in this.sensorBoxes.Values) {
                    sensorbox.Dispose();
                }
                this.sensorBoxes.Clear();

                for (int sensorIndex = 0; sensorIndex < view.Sensors.Count; sensorIndex++) {
                    SensorUsageConfig sensorConfig = view.Sensors[sensorIndex];

                    SensorProxy proxy;
                    if (!this.proxies.TryGetValue(sensorConfig.ID, out proxy!)) {
                        // KeyNotFoundException
                        string message = (
                            $"View \"{view.Name}\" attempts to use sensor with unkown ID: \"{sensorConfig.ID}\"\r\n" +
                            $"in config views[{index}]/sensors[{sensorIndex}]/id:\"{sensorConfig.ID}\"\r\n" +
                            $"Skipping creation of Sensor Box UI Element!"
                        );
                        this.messageCollection.addLine(message);
                        MessageBox.Show(
                            message,
                            "Sensor Box Error!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        continue;
                    }
                    SensorContextMenu contextMenuContainer = new(sensorConfig, proxy, this.messageCollection);
                    contextMenuContainer.Parent = this;
                    ConnectionContextMenu connectionContextMenu = this.connectionContextMenus[proxy.config.ID];
                    contextMenuContainer.toolStripMenuItemCommUnit.DropDown = connectionContextMenu.contextMenuCommunication;
                    contextMenuContainer.makeCommunicationSubmenuVisible();
                    SensorBox sensorBox = new(sensorConfig, proxy, contextMenuContainer, this.messageCollection);
                    sensorBox.Hide();
                    sensorBox.Name = $"sensorBox_{sensorConfig.ID}";
                    sensorBox.Parent = this.pictureBoxViewImage;
                    sensorBox.TabStop = false;
                    this.sensorBoxes.Add(sensorConfig.ID, sensorBox);
                    this.adjustSensorBoxPosition(sensorBox);
                    sensorBox.Show();
                }
            }));
        }

        public void connectSensorBoxes() {
            this.connectionInitialized = false;
            this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Initializing Sensors..."; }));
            foreach (SensorBox sensorBox in this.sensorBoxes.Values) {
                try {
                    sensorBox.initSensor();
                }
                catch (SocketException ex) {
                    sensorBox.proxy.printSocketException(ex);
                    this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Connection-Error during Initialization!"; }));
                    return;
                }
            }
            this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Initialization finished!"; }));
            this.connectionInitialized = true;
        }

        public void printDebugMessage(string message) {
            this.messageCollection.addLine($"{message}");
        }

        private void adjustSensorBoxPosition(SensorBox sensorBox) {
            PictureBox pb = this.pictureBoxViewImage;
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

            sensorBox.Location = new Point(
                (int)(offsetX + sensorBox.config.PositionX * imageSize.Width - (sensorBox.Width / 2)),
                (int)(offsetY + sensorBox.config.PositionY * imageSize.Height - (sensorBox.Height / 2))
            );
        }

        public void loadDataTable() {
            if (this.database == null) {
                return;
            }
            DataGridView dgv = this.dataGridViewDB;
            DataTable table = this.database.provideDataTable();
            dgv.DataSource = table;
            foreach (DataGridViewColumn column in dgv.Columns) {
                string? displayName = this.database.getDisplayNameFromHeader(column.HeaderText);
                if (displayName != null) {
                    column.HeaderText = displayName;
                }
            }
            table.DefaultView.RowFilter = this.filter;
        }

        private void pictureBoxViewImage_SizeChanged(object sender, EventArgs e) {
            foreach (SensorBox sensorbox in this.sensorBoxes.Values) {
                this.adjustSensorBoxPosition(sensorbox);
            }

        }

        private void MainWindow_Shown(object sender, EventArgs e) {
            this.comboBoxView.SelectedIndex = 0;
        }

        private void backgroundWorkerInitSensors_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            int index = (int)e.Argument!;
            this.createSensorBoxes(index);
            this.connectSensorBoxes();
        }

        private void backgroundWorkerMeasure_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            if (!this.connectionInitialized) {
                this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Unable to perform measurement before Initialization!"; }));
                return;
            }
            this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Contacting Sensors..."; }));

            bool hadConnectionError = false;
            Action<Exception> excHandler = (e) => {
                this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Connection-Error during Measurment!"; }));
                hadConnectionError = true;
            };

            this.updateAllBoxes(excHandler);

            if (!hadConnectionError) {
                this.labelStatus.Invoke(new Action(() => { this.labelStatus.Text = "Measurement finished!"; }));
            }
        }

        private void buttonInit_Click(object sender, EventArgs e) {
            if (!this.backgroundWorkerInitSensors.IsBusy) {
                this.backgroundWorkerInitSensors.RunWorkerAsync(this.comboBoxView.SelectedIndex);
            }
        }

        private void adjustFilter() {
            if (this.filter != string.Empty) {
                const string headerView = "view";
                const string headerUnit = "unit_id";
                string valueView = this.comboBoxView.Text;
                string valueUnit = this.textBoxUnit.Text;
                this.filter = $"[{headerView}] = '{valueView}' AND [{headerUnit}] = '{valueUnit}'";
            }
        }

        private void filterDataGrid() {
            const string headerView = "view";
            const string headerUnit = "unit_id";
            string valueView = this.comboBoxView.Text;
            string valueUnit = this.textBoxUnit.Text;
            this.filter = $"[{headerView}] = '{valueView}' AND [{headerUnit}] = '{valueUnit}'";
            DataTable table = (this.dataGridViewDB.DataSource as DataTable)!;
            table.DefaultView.RowFilter = this.filter;
        }

        private void removeFilter() {
            this.filter = string.Empty;
            DataTable table = (this.dataGridViewDB.DataSource as DataTable)!;
            table.DefaultView.RowFilter = this.filter;
        }

        private void buttonFilter_Click(object sender, EventArgs e) {
            this.filterDataGrid();
        }

        private void textBoxUnit_TextChanged(object sender, EventArgs e) {
            this.adjustFilter();
        }

        private void buttonRemoveFilter_Click(object sender, EventArgs e) {
            this.removeFilter();
        }

        private void labelMessagesCollapse_Click(object sender, LinkLabelLinkClickedEventArgs e) {
            this.splitContainerMessages.Panel2Collapsed = !this.splitContainerMessages.Panel2Collapsed;
            this.textBoxMessages.SelectionStart = this.textBoxMessages.Text.Length;
            this.textBoxMessages.ScrollToCaret();

        }

        private void textBoxMessages_TextChanged(object sender, EventArgs e) {
            this.textBoxMessages.SelectionStart = this.textBoxMessages.Text.Length;
            this.textBoxMessages.ScrollToCaret();
        }

        private void toolStripMenuItemFileNewConfig_Click(object sender, EventArgs e) {
            this.config = JSONConfig.createDefault();
            this.applyConfig();
        }

        private void toolStripMenuItemFileLoadConfig_Click(object sender, EventArgs e) {
            DialogResult result = this.openFileDialogConfig.ShowDialog();
            if (result == DialogResult.OK) {
                this.loadConfig(this.openFileDialogConfig.FileName);
            }
        }

        private void toolStripMenuItemFileSaveConfig_Click(object sender, EventArgs e) {
            DialogResult result = this.saveFileDialogConfig.ShowDialog();
            if (result == DialogResult.OK) {
                this.saveConfig(this.saveFileDialogConfig.FileName);
            }
        }

        private void toolStripMenuItemFileExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void ShowAboutPopup() {
            string version = Properties.Resources.VERSION.Trim();
            string date = Properties.Resources.DATE.Trim();
            bool dirtyBuild = (Properties.Resources.UNCOMMITED_CHANGES.Trim() != string.Empty);
            string versionExtra = version + (dirtyBuild ? " *(Work in Progress)" : string.Empty);

            string popupText = $"{this.Text}\r\n" +
                $"Version {versionExtra}\r\n" +
                $"Build Date {date}\r\n" +
                $"\r\n" +
                $"© Jakob Stadler 2023\r\n" +
                $"\r\n" +
                $"See LICENSE file for licensing information";

            MessageBox.Show(popupText, $"About {this.Text}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItemAboutAbout_Click(object sender, EventArgs e) {
            this.ShowAboutPopup();
        }

        private void toolStripMenuItemEditConnections_Click(object sender, EventArgs e) {
            ConnectionEditor editor = new(this.config);
            DialogResult result = editor.ShowDialog();
            if (result == DialogResult.OK) {
                this.applyConfig();
            }
        }

        private void toolStripMenuItemEditViews_Click(object sender, EventArgs e) {
            ViewEditor editor = new(this.config);
            DialogResult result = editor.ShowDialog();
            if (result == DialogResult.OK) {
                this.applyConfig();
            }
        }

        private void toolStripMenuItemEditDatabase_Click(object sender, EventArgs e) {
            DatabaseEditor editor = new(this.config);
            DialogResult result = editor.ShowDialog();
            if (result == DialogResult.OK) {
                this.applyConfig();
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e) {
            // Cleanup: Disconnect before closing
            foreach (Connection conn in this.connections) {
                conn.disconnect();
            }
        }
    }
}