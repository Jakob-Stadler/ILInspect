namespace ILInspect {

    public partial class DatabaseEditor : Form {
        private JSONConfig config;

        public DatabaseEditor(JSONConfig config) {
            this.InitializeComponent();
            this.config = config;
            this.fillFields();
        }

        private void fillFields() {
            string source = this.config.Database.Source;
            if (source != ":memory:") {
                source = Path.GetFullPath(source, this.config.ConfigDirectory ?? AppDomain.CurrentDomain.BaseDirectory);
            }
            this.textBoxDatabaseSource.Text = source;
        }

        private void buttonDatabaseBrowse_Click(object sender, EventArgs e) {
            DialogResult dialogResult = this.saveFileDialogSource.ShowDialog();
            if (dialogResult == DialogResult.OK) {
                this.textBoxDatabaseSource.Text = this.saveFileDialogSource.FileName;
            }
        }

        private void buttonDatabaseMemory_Click(object sender, EventArgs e) {
            this.textBoxDatabaseSource.Text = ":memory:";
        }

        private void buttonOK_Click(object sender, EventArgs e) {
            this.config.Database.Source = this.textBoxDatabaseSource.Text;
            this.config.ConfigDirectory = null;  // Remove Path to loaded config file, since the current config is not loaded from a file anymore.
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
