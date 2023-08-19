using System.ComponentModel;
using System.Net.Sockets;

namespace ILInspect {

    public partial class SensorContextMenu : UserControl {
        public SensorUsageConfig? config;
        public SensorProxy proxy;
        private MessageCollection messageCollection;
        private bool layoutAdjustedComboboxes = false;

        public SensorContextMenu(SensorUsageConfig? config, SensorProxy proxy, MessageCollection messageCollection) {
            this.config = config;
            this.proxy = proxy;
            this.messageCollection = messageCollection;
            this.InitializeComponent();
            this.createAllSubmenus();
            this.adjustSize();
        }

        public void makeCommunicationSubmenuVisible() {
            this.toolStripSeparatorMain2.Visible = true;
            this.toolStripMenuItemCommUnit.Visible = true;
        }

        private void createAllSubmenus() {
            this.createBank0Submenus();
            this.createBank1Submenus();
            this.createBank2Submenus();
            this.createBank3Submenus();
            this.createAnalogFreeRangeSubmenus();
            this.createDiffCountOneShotSubmenus();
            this.createAutoTriggerSubmenus();
            this.createAlarmCountSubmenus();
            this.createDelayDurationSubmenus();
            this.createTuningRangeSubmenus();
            this.createCalibrationTarget1Submenus();
            this.createCalibrationTarget2Submenus();
            this.createTwoPointCalculationCalibrationTarget1Submenus();
            this.createTwoPointCalculationCalibrationTarget2Submenus();
            this.createThreePointCalculationCalibrationTarget1Submenus();
            this.createThreePointCalculationCalibrationTarget3Submenus();
        }

        private string valueFormat {
            get {
                int numberZeros = (int)Math.Round(Math.Log10(1 / this.proxy.config.ConversionFactor), MidpointRounding.ToPositiveInfinity);
                if (numberZeros == 0) {
                    return $"{{0:+#0;-#0;0}}";
                } else {
                    string zeros = new('0', numberZeros);
                    return $"{{0:+#0.{zeros};-#0.{zeros};0}}";
                }
            }
        }

        private string formatValue(int value) {
            switch (value) {
                case 99999:
                    return "Over Range!";
                case -99999:
                    return "Under Range!";
                case -99998:
                    return "- - - -";
                case 100000:
                    return "Error!";
                default:
                    return string.Format(this.valueFormat, (double)value * this.proxy.config.ConversionFactor);
            }
        }

        private string formatValueNoFactor(int value) {
            return string.Format("{0}", value);
        }


        private int? convertText(string text) {
            double decimalValue;
            if (double.TryParse(text, out decimalValue)) {
                return (int)(decimalValue / this.proxy.config.ConversionFactor);
            }
            return null;
        }


        private int? convertTextNoFactor(string text) {
            int value;
            if (int.TryParse(text, out value)) {
                return value;
            }
            return null;
        }

        private void createGetSetSubmenu(
            ToolStripMenuItem item,
            ToolStripTextBox textbox,
            Action<Action<int?>?, Action<Exception>?> getter,
            Action<int, Action<string, string>?, Action<Exception>?> setter,
            Func<int, string>? formatFunc = null,
            Func<string, int?>? conversionFunc = null
        ) {
            string postfix = $"_{item.Name}";

            ContextMenuStrip contextMenuGetSet;
            ToolStripMenuItem toolStripMenuItemGet;
            ToolStripMenuItem toolStripMenuItemSet;

            contextMenuGetSet = new ContextMenuStrip(this.components);
            toolStripMenuItemGet = new ToolStripMenuItem();
            toolStripMenuItemSet = new ToolStripMenuItem();
            contextMenuGetSet.SuspendLayout();


            List<ToolStripMenuItem> list = new() { toolStripMenuItemGet, toolStripMenuItemSet, };

            EventHandler clickHandlerGet = this.createClickHandlerGet(textbox, getter, list, formatFunc);
            EventHandler clickHandlerSet = this.createClickHandlerSet(textbox, setter, list, conversionFunc);

            //
            // contextMenuGetSet
            //
            contextMenuGetSet.Items.AddRange(new ToolStripItem[] { toolStripMenuItemGet, toolStripMenuItemSet });
            contextMenuGetSet.Name = $"contextMenuGetSet{postfix}";
            contextMenuGetSet.Size = this.contextMenuGetSet.Size;
            contextMenuGetSet.OwnerItem = item;
            contextMenuGetSet.Closed += this.contextMenu_HideImages!;
            contextMenuGetSet.Closing += this.contextMenu_AbortClosing!;
            contextMenuGetSet.Opening += this.contextMenu_HideImages!;
            //
            // toolStripMenuItemGet
            //
            toolStripMenuItemGet.Name = $"toolStripMenuItemGet{postfix}";
            toolStripMenuItemGet.Size = this.toolStripMenuItemGet.Size;
            toolStripMenuItemGet.Text = this.toolStripMenuItemGet.Text;
            toolStripMenuItemGet.Click += clickHandlerGet;
            //
            // toolStripMenuItemSet
            //
            toolStripMenuItemSet.Name = $"toolStripMenuItemSet{postfix}";
            toolStripMenuItemSet.Size = this.toolStripMenuItemSet.Size;
            toolStripMenuItemSet.Text = this.toolStripMenuItemSet.Text;
            toolStripMenuItemSet.Click += clickHandlerSet;

            item.DropDown = contextMenuGetSet;

            contextMenuGetSet.ResumeLayout(false);
        }


        private EventHandler createClickHandlerGet(
            ToolStripTextBox textbox,
            Action<Action<int?>?, Action<Exception>?> proxyCall,
            IList<ToolStripMenuItem>? list = null,
            Func<int, string>? formatFunc = null
        ) {
            if (formatFunc == null) {
                formatFunc = this.formatValue;
            }
            EventHandler getter = (sender, e) => {
                foreach (ToolStripMenuItem item in list ?? Enumerable.Empty<ToolStripItem>()) {
                    item.Image = null;
                }

                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
                Action<int?> updateTextbox = (int? value) => {
                    if (value != null) {
                        string text = formatFunc((int)value);
                        this.Invoke(() => {
                            textbox.Text = text;
                            menuItem.Image = Properties.Resources.green_circle;
                        });
                    } else {
                        this.Invoke(() => {
                            textbox.Text = "No Value!";
                            menuItem.Image = Properties.Resources.red_circle;
                        });
                    }
                };
                Action<Exception> excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.proxy.printSocketException((SocketException)ex);
                    } else {
                        this.messageCollection.addLine(ex.ToString());
                    }
                    this.Invoke(() => {
                        menuItem.Image = Properties.Resources.red_circle;
                        textbox.Text = "Error!";
                    });
                };
                textbox.Text = "Waiting for result...";
                this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
            };
            return getter;
        }

        private EventHandler createClickHandlerSet(
            ToolStripTextBox textbox,
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall,
            IList<ToolStripMenuItem>? list = null,
            Func<string, int?>? conversionFunc = null
        ) {
            if (conversionFunc == null) {
                conversionFunc = this.convertText;
            }
            EventHandler setter = (sender, e) => {
                foreach (ToolStripMenuItem item in list ?? Enumerable.Empty<ToolStripItem>()) {
                    item.Image = null;
                }

                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
                // get value (converted from textbox)
                int? value = conversionFunc(textbox.Text);
                if (value != null) {
                    Action<string, string>? receiveFunc = null;
                    this.contextMenuEntryClickWrite(menuItem, proxyCall, (int)value, receiveFunc);
                } else {
                    this.messageCollection.addLine($"Can't convert value to number: {textbox.Text}");
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                }
            };
            return setter;
        }

        private void createBank0Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxBank0Value;
            ToolStripMenuItem itemHigh = this.toolStripMenuItemBank0HIGH;
            ToolStripMenuItem itemLow = this.toolStripMenuItemBank0LOW;
            ToolStripMenuItem itemShift = this.toolStripMenuItemBank0Shift;
            ToolStripMenuItem itemAnalogUpper = this.toolStripMenuItemBank0AnalogUpper;
            ToolStripMenuItem itemAnalogLower = this.toolStripMenuItemBank0AnalogLower;

            this.createGetSetSubmenu(itemHigh, textbox, this.proxy.sendGetHIGHBank0, this.proxy.sendSetHIGHBank0);

            this.createGetSetSubmenu(itemLow, textbox, this.proxy.sendGetLOWBank0, this.proxy.sendSetLOWBank0);

            this.createGetSetSubmenu(itemShift, textbox, this.proxy.sendGetShiftBank0, this.proxy.sendSetShiftBank0);

            this.createGetSetSubmenu(itemAnalogUpper, textbox, this.proxy.sendGetAnalogOutputUpperLimitBank0, this.proxy.sendSetAnalogOutputUpperLimitBank0);

            this.createGetSetSubmenu(itemAnalogLower, textbox, this.proxy.sendGetAnalogOutputLowerLimitBank0, this.proxy.sendSetAnalogOutputLowerLimitBank0);
        }


        private void createBank1Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxBank1Value;
            ToolStripMenuItem itemHigh = this.toolStripMenuItemBank1HIGH;
            ToolStripMenuItem itemLow = this.toolStripMenuItemBank1LOW;
            ToolStripMenuItem itemShift = this.toolStripMenuItemBank1Shift;
            ToolStripMenuItem itemAnalogUpper = this.toolStripMenuItemBank1AnalogUpper;
            ToolStripMenuItem itemAnalogLower = this.toolStripMenuItemBank1AnalogLower;

            this.createGetSetSubmenu(itemHigh, textbox, this.proxy.sendGetHIGHBank1, this.proxy.sendSetHIGHBank1);

            this.createGetSetSubmenu(itemLow, textbox, this.proxy.sendGetLOWBank1, this.proxy.sendSetLOWBank1);

            this.createGetSetSubmenu(itemShift, textbox, this.proxy.sendGetShiftBank1, this.proxy.sendSetShiftBank1);

            this.createGetSetSubmenu(itemAnalogUpper, textbox, this.proxy.sendGetAnalogOutputUpperLimitBank1, this.proxy.sendSetAnalogOutputUpperLimitBank1);

            this.createGetSetSubmenu(itemAnalogLower, textbox, this.proxy.sendGetAnalogOutputLowerLimitBank1, this.proxy.sendSetAnalogOutputLowerLimitBank1);
        }

        private void createBank2Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxBank2Value;
            ToolStripMenuItem itemHigh = this.toolStripMenuItemBank2HIGH;
            ToolStripMenuItem itemLow = this.toolStripMenuItemBank2LOW;
            ToolStripMenuItem itemShift = this.toolStripMenuItemBank2Shift;
            ToolStripMenuItem itemAnalogUpper = this.toolStripMenuItemBank2AnalogUpper;
            ToolStripMenuItem itemAnalogLower = this.toolStripMenuItemBank2AnalogLower;

            this.createGetSetSubmenu(itemHigh, textbox, this.proxy.sendGetHIGHBank2, this.proxy.sendSetHIGHBank2);

            this.createGetSetSubmenu(itemLow, textbox, this.proxy.sendGetLOWBank2, this.proxy.sendSetLOWBank2);

            this.createGetSetSubmenu(itemShift, textbox, this.proxy.sendGetShiftBank2, this.proxy.sendSetShiftBank2);

            this.createGetSetSubmenu(itemAnalogUpper, textbox, this.proxy.sendGetAnalogOutputUpperLimitBank2, this.proxy.sendSetAnalogOutputUpperLimitBank2);

            this.createGetSetSubmenu(itemAnalogLower, textbox, this.proxy.sendGetAnalogOutputLowerLimitBank2, this.proxy.sendSetAnalogOutputLowerLimitBank2);
        }

        private void createBank3Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxBank3Value;
            ToolStripMenuItem itemHigh = this.toolStripMenuItemBank3HIGH;
            ToolStripMenuItem itemLow = this.toolStripMenuItemBank3LOW;
            ToolStripMenuItem itemShift = this.toolStripMenuItemBank3Shift;
            ToolStripMenuItem itemAnalogUpper = this.toolStripMenuItemBank3AnalogUpper;
            ToolStripMenuItem itemAnalogLower = this.toolStripMenuItemBank3AnalogLower;

            this.createGetSetSubmenu(itemHigh, textbox, this.proxy.sendGetHIGHBank3, this.proxy.sendSetHIGHBank3);

            this.createGetSetSubmenu(itemLow, textbox, this.proxy.sendGetLOWBank3, this.proxy.sendSetLOWBank3);

            this.createGetSetSubmenu(itemShift, textbox, this.proxy.sendGetShiftBank3, this.proxy.sendSetShiftBank3);

            this.createGetSetSubmenu(itemAnalogUpper, textbox, this.proxy.sendGetAnalogOutputUpperLimitBank3, this.proxy.sendSetAnalogOutputUpperLimitBank3);

            this.createGetSetSubmenu(itemAnalogLower, textbox, this.proxy.sendGetAnalogOutputLowerLimitBank3, this.proxy.sendSetAnalogOutputLowerLimitBank3);
        }

        private void createAnalogFreeRangeSubmenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxOutputAnalogLimitsFreeValue;
            ToolStripMenuItem itemAnalogUpper = this.toolStripMenuItemOutputAnalogLimitsFreeUpper;
            ToolStripMenuItem itemAnalogLower = this.toolStripMenuItemOutputAnalogLimitsFreeLower;

            this.createGetSetSubmenu(itemAnalogUpper, textbox, this.proxy.sendGetAnalogOutputUpperLimitValue, this.proxy.sendSetAnalogOutputUpperLimitValue);

            this.createGetSetSubmenu(itemAnalogLower, textbox, this.proxy.sendGetAnalogOutputLowerLimitValue, this.proxy.sendSetAnalogOutputLowerLimitValue);
        }

        private void createDiffCountOneShotSubmenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxFilterDiffCountOneShotTime;
            ToolStripMenuItem menuItem = this.toolStripMenuItemFilterDiffCountOneShotTime;

            this.createGetSetSubmenu(
              menuItem,
              textbox,
              this.proxy.sendGetTimerDurationDiffCountFilter,
              this.proxy.sendSetTimerDurationDiffCountFilter,
              this.formatValueNoFactor,
              this.convertTextNoFactor);
        }

        private void createAutoTriggerSubmenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxHoldingAutoTrigger;
            ToolStripMenuItem menuItem = this.toolStripMenuItemHoldingAutoTrigger;

            this.createGetSetSubmenu(menuItem, textbox, this.proxy.sendGetAutoHoldTriggerSetting, this.proxy.sendSetAutoHoldTriggerSetting);
        }

        private void createAlarmCountSubmenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxSamplingAlarmCount;
            ToolStripMenuItem menuItem = this.toolStripMenuItemSamplingAlarmCount;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetAlarmCount,
                this.proxy.sendSetAlarmCount,
                this.formatValueNoFactor,
                this.convertTextNoFactor
            );
        }

        private void createDelayDurationSubmenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxTimingDelayDuration;
            ToolStripMenuItem menuItem = this.toolStripMenuItemTimingDelayDuration;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetTimerDuration,
                this.proxy.sendSetTimerDuration,
                this.formatValueNoFactor,
                this.convertTextNoFactor
            );
        }

        private void createTuningRangeSubmenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxTuningRange;
            ToolStripMenuItem menuItem = this.toolStripMenuItemTuningRange;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetToleranceTuningRange,
                this.proxy.sendSetToleranceTuningRange
            );
        }

        private void createCalibrationTarget1Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxCalibrationTarget1;
            ToolStripMenuItem menuItem = this.toolStripMenuItemCalibrationTarget1;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetCalibrationFunctionSet1,
                this.proxy.sendSetCalibrationFunctionSet1
            );
        }

        private void createCalibrationTarget2Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxCalibrationTarget2;
            ToolStripMenuItem menuItem = this.toolStripMenuItemCalibrationTarget2;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetCalibrationFunctionSet2,
                this.proxy.sendSetCalibrationFunctionSet2
            );
        }

        private void createTwoPointCalculationCalibrationTarget1Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxCalculationTwoPointTarget1;
            ToolStripMenuItem menuItem = this.toolStripMenuItemCalculationTwoPointTarget1;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetCalculatedValueTwoPointCalibrationSet1,
                this.proxy.sendSetCalculatedValueTwoPointCalibrationSet1
            );
        }

        private void createTwoPointCalculationCalibrationTarget2Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxCalculationTwoPointTarget2;
            ToolStripMenuItem menuItem = this.toolStripMenuItemCalculationTwoPointTarget2;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetCalculatedValueTwoPointCalibrationSet2,
                this.proxy.sendSetCalculatedValueTwoPointCalibrationSet2
            );
        }

        private void createThreePointCalculationCalibrationTarget1Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxCalculationThreePointTarget1;
            ToolStripMenuItem menuItem = this.toolStripMenuItemCalculationThreePointTarget1;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetCalculatedValueThreePointCalibrationSet1,
                this.proxy.sendSetCalculatedValueThreePointCalibrationSet1
            );
        }

        private void createThreePointCalculationCalibrationTarget3Submenus() {
            ToolStripTextBox textbox = this.toolStripTextBoxCalculationThreePointTarget3;
            ToolStripMenuItem menuItem = this.toolStripMenuItemCalculationThreePointTarget3;

            this.createGetSetSubmenu(
                menuItem,
                textbox,
                this.proxy.sendGetCalculatedValueThreePointCalibrationSet3,
                this.proxy.sendSetCalculatedValueThreePointCalibrationSet3
            );
        }

        private void backgroundWorkerCommunication_DoWork(object sender, DoWorkEventArgs e) {
            Action action = (Action)e.Argument!;
            action();
        }

        public void initSensor() {
            if (this.config == null)
                return;

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
            if (this.proxy.connection.StopLaser) {
                this.proxy.sendSetLaserEmissionStop(1);
            }

        }

        public void adjustSize() {
            this.toolStripComboBoxSystemParametersAnalog.Width = this.contextMenuSystemParameters.DisplayRectangle.Width - 2 * this.toolStripComboBoxSystemParametersAnalog.Margin.Horizontal;
            this.toolStripComboBoxSystemParametersPolarity.Width = this.contextMenuSystemParameters.DisplayRectangle.Width - 2 * this.toolStripComboBoxSystemParametersPolarity.Margin.Horizontal;
        }

        public void ShowContextMenu(Control sender, int x, int y) {
            this.contextMenu.Show(sender, x, y);
        }

        private void contextMenu_AbortClosing(object sender, ToolStripDropDownClosingEventArgs e) {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) {
                e.Cancel = true;
            }
        }

        private void hideImages(ContextMenuStrip? menuStrip) {
            if (menuStrip != null) {
                foreach (ToolStripItem item in menuStrip.Items) {
                    item.Image = null;
                }
            }
        }

        private void hideImagesClearText(ContextMenuStrip? menuStrip) {
            if (menuStrip != null) {
                foreach (ToolStripItem item in menuStrip.Items) {
                    item.Image = null;
                    if (typeof(ToolStripTextBox).IsAssignableFrom(item.GetType())) {
                        item.Text = string.Empty;
                    }
                }
            }
        }

        private void hideImagesClearDropdown(ContextMenuStrip? menuStrip) {
            if (menuStrip != null) {
                foreach (ToolStripItem item in menuStrip.Items) {
                    item.Image = null;
                    if (typeof(ToolStripComboBox).IsAssignableFrom(item.GetType())) {
                        ((ToolStripComboBox)item).SelectedIndex = -1;
                    }
                }
            }
        }

        private void contextMenu_HideImages(object sender, ToolStripDropDownClosedEventArgs e) {
            this.hideImages(sender as ContextMenuStrip);
        }

        private void contextMenu_HideImages(object sender, CancelEventArgs e) {
            this.hideImages(sender as ContextMenuStrip);
        }

        private void contextMenu_HideImagesClearText(object sender, ToolStripDropDownClosedEventArgs e) {
            this.hideImagesClearText(sender as ContextMenuStrip);
        }

        private void contextMenu_HideImagesClearText(object sender, CancelEventArgs e) {
            this.hideImagesClearText(sender as ContextMenuStrip);
        }

        private void contextMenu_HideImagesClearDropdown(object sender, ToolStripDropDownClosedEventArgs e) {
            this.hideImagesClearDropdown(sender as ContextMenuStrip);
        }

        private void contextMenu_HideImagesClearDropdown(object sender, CancelEventArgs e) {
            this.hideImagesClearDropdown(sender as ContextMenuStrip);
        }

        private void contextMenuEntryClickWrite(
            ToolStripMenuItem menuItem,
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall,
            int value,
            Action<string, string>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            if (receiveFunc == null) {
                receiveFunc = (string response, string expectedResponse) => {
                    if (response != expectedResponse) {
                        this.messageCollection.addLine(
                            $"Response does not match expectation!\r\n" +
                            $"Expected: {expectedResponse}\r\n" +
                            $"Received: {response}"
                         );
                        this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                    } else {
                        this.Invoke(() => { menuItem.Image = Properties.Resources.green_circle; });
                    }
                };
            }

            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.proxy.printSocketException((SocketException)ex);
                    } else {
                        this.messageCollection.addLine(ex.ToString());
                    }
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                };
            }

            Action action = () => {
                this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                proxyCall(value, receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }

        private void contextMenuEntryClickRead(
            ToolStripMenuItem menuItem,
            Action<Action<int?>?, Action<Exception>?> proxyCall,
            Action<int?>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            if (receiveFunc == null) {
                receiveFunc = (int? value) => {
                    if (value == null) {
                        this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                    } else {
                        this.messageCollection.addLine($"Received: {value}");
                        this.Invoke(() => { menuItem.Image = Properties.Resources.green_circle; });
                    }
                };
            }

            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.proxy.printSocketException((SocketException)ex);
                    } else {
                        this.messageCollection.addLine(ex.ToString());
                    }
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                };
            }

            Action action = () => {
                this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }

        private void toolStripMenuItemReInit_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action action = () => {
                try {
                    this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                    this.initSensor();
                    this.Invoke(() => { menuItem.Image = Properties.Resources.green_circle; });
                }
                catch (SocketException ex) {
                    this.proxy.printSocketException(ex);
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                }
                catch (Exception ex) {
                    this.messageCollection.addLine(ex.ToString());
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                }
            };
            this.backgroundWorkerCommunication.RunWorkerAsync(action);
        }

        private void hideActiveBankState() {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItembank0,
                this.toolStripMenuItembank1,
                this.toolStripMenuItembank2,
                this.toolStripMenuItembank3,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }
        }

        private void toolStripMenuItemBank0Switch_Click(object sender, EventArgs e) {
            this.hideActiveBankState();
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetBank;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemBank1Switch_Click(object sender, EventArgs e) {
            this.hideActiveBankState();
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetBank;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemBank2Switch_Click(object sender, EventArgs e) {
            this.hideActiveBankState();
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetBank;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemBank3Switch_Click(object sender, EventArgs e) {
            this.hideActiveBankState();
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetBank;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void highlightMenuItem(
            ToolStripMenuItem menuItem,
            List<ToolStripMenuItem> list,
            Action<Action<int?>?, Action<Exception>?> proxyCall,
            Bitmap? bitmapActive = null,
            Bitmap? bitmapInactive = null
        ) {
            if (bitmapActive == null) {
                bitmapActive = Properties.Resources.blue_circle;
            }
            if (bitmapInactive == null) {
                bitmapInactive = Properties.Resources.grey_circle;
            }
            Action<int?>? receiveFunc = (int? value) => {
                if (value == null) {
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                } else {
                    this.Invoke(() => {
                        menuItem.Image = Properties.Resources.green_circle;
                        for (int i = 0; i < list.Count; i++) {
                            if (i == value) {
                                list[i].Image = Properties.Resources.blue_circle;
                            } else {
                                list[i].Image = Properties.Resources.grey_circle;
                            }
                        }
                    });
                }
            };

            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
            };

            Action action = () => {
                this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }

        private void highlightMenuItemsBitfield(
            ToolStripMenuItem menuItem,
            List<ToolStripMenuItem> list,
            Action<Action<int?>?, Action<Exception>?> proxyCall,
            Bitmap? bitmapActive = null,
            Bitmap? bitmapInactive = null
        ) {
            if (bitmapActive == null) {
                bitmapActive = Properties.Resources.blue_circle;
            }
            if (bitmapInactive == null) {
                bitmapInactive = Properties.Resources.grey_circle;
            }
            Action<int?>? receiveFunc = (int? value) => {
                if (value == null) {
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                } else {
                    this.Invoke(() => {
                        menuItem.Image = Properties.Resources.green_circle;
                        for (int i = 0; i < list.Count; i++) {
                            if (((value >> i) & 1) != 0) {
                                list[i].Image = bitmapActive;
                            } else {
                                list[i].Image = bitmapInactive;
                            }
                        }
                    });
                }
            };

            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
            };

            Action action = () => {
                this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }


        private void highlightBank(ToolStripMenuItem menuItem, Action<Action<int?>?, Action<Exception>?> proxyCall) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItembank0,
                this.toolStripMenuItembank1,
                this.toolStripMenuItembank2,
                this.toolStripMenuItembank3,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void highlightStatus(
            ToolStripMenuItem menuItem,
            Action<Action<int?>?, Action<Exception>?> proxyCall,
            int offset = 0,
            Bitmap? bitmapInProgress = null,
            Bitmap? bitmapSuccess = null,
            Bitmap? bitmapFail = null,
            Bitmap? bitmapInactive = null
        ) {
            if (bitmapInProgress == null) {
                bitmapInProgress = Properties.Resources.yellow_circle;
            }
            if (bitmapSuccess == null) {
                bitmapSuccess = Properties.Resources.green_circle;
            }
            if (bitmapFail == null) {
                bitmapFail = Properties.Resources.red_circle;
            }
            if (bitmapInactive == null) {
                bitmapInactive = Properties.Resources.grey_circle;
            }
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemStatusInProgress,
                this.toolStripMenuItemStatusSuccess,
                this.toolStripMenuItemStatusFail,
                this.toolStripMenuItemStatusSettings,
                this.toolStripMenuItemStatusEEPROM,
                this.toolStripMenuItemStatusZeroShift,
                this.toolStripMenuItemStatusReset,
                this.toolStripMenuItemStatusTuning,
                this.toolStripMenuItemStatusCalibration,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            Action<int?>? receiveFunc = (int? value) => {
                if (value == null) {
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                } else {
                    this.Invoke(() => {
                        menuItem.Image = Properties.Resources.green_circle;
                        this.toolStripMenuItemStatusInProgress.Image = bitmapInactive;
                        this.toolStripMenuItemStatusSuccess.Image = bitmapInactive;
                        this.toolStripMenuItemStatusFail.Image = bitmapInactive;
                        switch (value + offset) {
                            case 0:
                                this.toolStripMenuItemStatusInProgress.Image = bitmapInProgress;
                                break;
                            case 1:
                                this.toolStripMenuItemStatusSuccess.Image = bitmapSuccess;
                                break;
                            case 2:
                                this.toolStripMenuItemStatusFail.Image = bitmapFail;
                                break;
                            default:
                                menuItem.Image = Properties.Resources.red_circle;
                                this.messageCollection.addLine($"Invalid Status value: {value} + {offset}");
                                break;
                        }
                    });
                }
            };

            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
            };

            Action action = () => {
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.yellow_circle;
                    this.toolStripMenuItemStatusInProgress.Image = null;
                    this.toolStripMenuItemStatusSuccess.Image = null;
                    this.toolStripMenuItemStatusFail.Image = null;
                });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }

        private void toolStripMenuItemActiveBank_Click(object sender, EventArgs e) {
            this.toolStripMenuItemActiveBank.Image = null;
            this.toolStripMenuItemSelectedBank.Image = null;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetBankStatus;
            this.highlightBank(menuItem, proxyCall);
        }


        private void toolStripMenuItemSelectedBank_Click(object sender, EventArgs e) {
            this.toolStripMenuItemActiveBank.Image = null;
            this.toolStripMenuItemSelectedBank.Image = null;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetBank;
            this.highlightBank(menuItem, proxyCall);
        }

        private void toolStripMenuItemBankSwitchingShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemBankSwitchingButtonNetwork,
                this.toolStripMenuItemBankSwitchingExternalWireSignal,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetBankShiftMethod;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemBankSwitchingButtonNetwork_Click(object sender, EventArgs e) {
            this.toolStripMenuItemBankSwitchingShow.Image = null;
            this.toolStripMenuItemBankSwitchingButtonNetwork.Image = null;
            this.toolStripMenuItemBankSwitchingExternalWireSignal.Image = null;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetBankShiftMethod;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemBankSwitchingExternalWireSignal_Click(object sender, EventArgs e) {
            this.toolStripMenuItemBankSwitchingShow.Image = null;
            this.toolStripMenuItemBankSwitchingButtonNetwork.Image = null;
            this.toolStripMenuItemBankSwitchingExternalWireSignal.Image = null;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetBankShiftMethod;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemErrorsShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemErrorsBit0,
                this.toolStripMenuItemErrorsBit1,
                this.toolStripMenuItemErrorsBit2,
                this.toolStripMenuItemErrorsBit3,
                this.toolStripMenuItemErrorsBit4,
                this.toolStripMenuItemErrorsBit5,
                this.toolStripMenuItemErrorsBit6,
                this.toolStripMenuItemErrorsBit7,
                this.toolStripMenuItemErrorsBit8,
                this.toolStripMenuItemErrorsBit9,
                this.toolStripMenuItemErrorsBit10,
                this.toolStripMenuItemErrorsBit11,
                this.toolStripMenuItemErrorsBit12,
                this.toolStripMenuItemErrorsBit13,
                this.toolStripMenuItemErrorsBit14,
                this.toolStripMenuItemErrorsBit15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetSensorAmplifierError;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);

        }

        private void contextMenuSystemParameters_Layout(object sender, LayoutEventArgs e) {
            if (!this.layoutAdjustedComboboxes) {
                this.layoutAdjustedComboboxes = true;
                this.toolStripComboBoxSystemParametersAnalog.Width =
                    this.contextMenuSystemParameters.DisplayRectangle.Width
                    - 2 * this.toolStripComboBoxSystemParametersAnalog.Margin.Horizontal;
                this.toolStripComboBoxSystemParametersPolarity.Width =
                    this.contextMenuSystemParameters.DisplayRectangle.Width
                    - 2 * this.toolStripComboBoxSystemParametersPolarity.Margin.Horizontal;
            }
        }

        private void toolStripMenuItemExtInputShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1,
                this.toolStripMenuItemExtInput2,
                this.toolStripMenuItemExtInput3,
                this.toolStripMenuItemExtInput4,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetExternalInputStatus;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemInputSettingsShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInputSettingsInitial,
                this.toolStripMenuItemInputSettingsUser,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetExternalInputSetting;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemInputSettingsInitial_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInputSettingsShow,
                this.toolStripMenuItemInputSettingsInitial,
                this.toolStripMenuItemInputSettingsUser,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInputSetting;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemInputSettingsUser_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInputSettingsShow,
                this.toolStripMenuItemInputSettingsInitial,
                this.toolStripMenuItemInputSettingsUser,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInputSetting;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput1Show_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1ZeroShift,
                this.toolStripMenuItemExtInput1BankA,
                this.toolStripMenuItemExtInput1BankB,
                this.toolStripMenuItemExtInput1LaserStop,
                this.toolStripMenuItemExtInput1Unused,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetExternalInput1;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemExtInput1ZeroShift_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1Show,
                this.toolStripMenuItemExtInput1ZeroShift,
                this.toolStripMenuItemExtInput1BankA,
                this.toolStripMenuItemExtInput1BankB,
                this.toolStripMenuItemExtInput1LaserStop,
                this.toolStripMenuItemExtInput1Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput1;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput1BankA_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1Show,
                this.toolStripMenuItemExtInput1ZeroShift,
                this.toolStripMenuItemExtInput1BankA,
                this.toolStripMenuItemExtInput1BankB,
                this.toolStripMenuItemExtInput1LaserStop,
                this.toolStripMenuItemExtInput1Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput1;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput1BankB_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1Show,
                this.toolStripMenuItemExtInput1ZeroShift,
                this.toolStripMenuItemExtInput1BankA,
                this.toolStripMenuItemExtInput1BankB,
                this.toolStripMenuItemExtInput1LaserStop,
                this.toolStripMenuItemExtInput1Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput1;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput1LaserStop_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1Show,
                this.toolStripMenuItemExtInput1ZeroShift,
                this.toolStripMenuItemExtInput1BankA,
                this.toolStripMenuItemExtInput1BankB,
                this.toolStripMenuItemExtInput1LaserStop,
                this.toolStripMenuItemExtInput1Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput1;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput1Unused_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput1Show,
                this.toolStripMenuItemExtInput1ZeroShift,
                this.toolStripMenuItemExtInput1BankA,
                this.toolStripMenuItemExtInput1BankB,
                this.toolStripMenuItemExtInput1LaserStop,
                this.toolStripMenuItemExtInput1Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput1;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput2Show_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput2Reset,
                this.toolStripMenuItemExtInput2BankA,
                this.toolStripMenuItemExtInput2BankB,
                this.toolStripMenuItemExtInput2LaserStop,
                this.toolStripMenuItemExtInput2Unused,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetExternalInput2;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemExtInput2Reset_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput2Show,
                this.toolStripMenuItemExtInput2Reset,
                this.toolStripMenuItemExtInput2BankA,
                this.toolStripMenuItemExtInput2BankB,
                this.toolStripMenuItemExtInput2LaserStop,
                this.toolStripMenuItemExtInput2Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput2;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput2BankA_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput2Show,
                this.toolStripMenuItemExtInput2Reset,
                this.toolStripMenuItemExtInput2BankA,
                this.toolStripMenuItemExtInput2BankB,
                this.toolStripMenuItemExtInput2LaserStop,
                this.toolStripMenuItemExtInput2Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput2;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput2BankB_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput2Show,
                this.toolStripMenuItemExtInput2Reset,
                this.toolStripMenuItemExtInput2BankA,
                this.toolStripMenuItemExtInput2BankB,
                this.toolStripMenuItemExtInput2LaserStop,
                this.toolStripMenuItemExtInput2Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput2;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput2LaserStop_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput2Show,
                this.toolStripMenuItemExtInput2Reset,
                this.toolStripMenuItemExtInput2BankA,
                this.toolStripMenuItemExtInput2BankB,
                this.toolStripMenuItemExtInput2LaserStop,
                this.toolStripMenuItemExtInput2Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput2;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput2Unused_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput2Show,
                this.toolStripMenuItemExtInput2Reset,
                this.toolStripMenuItemExtInput2BankA,
                this.toolStripMenuItemExtInput2BankB,
                this.toolStripMenuItemExtInput2LaserStop,
                this.toolStripMenuItemExtInput2Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput2;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput3Show_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput3Timing,
                this.toolStripMenuItemExtInput3BankA,
                this.toolStripMenuItemExtInput3BankB,
                this.toolStripMenuItemExtInput3LaserStop,
                this.toolStripMenuItemExtInput3Unused,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetExternalInput3;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemExtInput3Timing_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput3Show,
                this.toolStripMenuItemExtInput3Timing,
                this.toolStripMenuItemExtInput3BankA,
                this.toolStripMenuItemExtInput3BankB,
                this.toolStripMenuItemExtInput3LaserStop,
                this.toolStripMenuItemExtInput3Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput3;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput3BankA_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput3Show,
                this.toolStripMenuItemExtInput3Timing,
                this.toolStripMenuItemExtInput3BankA,
                this.toolStripMenuItemExtInput3BankB,
                this.toolStripMenuItemExtInput3LaserStop,
                this.toolStripMenuItemExtInput3Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput3;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput3BankB_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput3Show,
                this.toolStripMenuItemExtInput3Timing,
                this.toolStripMenuItemExtInput3BankA,
                this.toolStripMenuItemExtInput3BankB,
                this.toolStripMenuItemExtInput3LaserStop,
                this.toolStripMenuItemExtInput3Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput3;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput3LaserStop_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput3Show,
                this.toolStripMenuItemExtInput3Timing,
                this.toolStripMenuItemExtInput3BankA,
                this.toolStripMenuItemExtInput3BankB,
                this.toolStripMenuItemExtInput3LaserStop,
                this.toolStripMenuItemExtInput3Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput3;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput3Unused_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput3Show,
                this.toolStripMenuItemExtInput3Timing,
                this.toolStripMenuItemExtInput3BankA,
                this.toolStripMenuItemExtInput3BankB,
                this.toolStripMenuItemExtInput3LaserStop,
                this.toolStripMenuItemExtInput3Unused,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput3;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput4Show_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput4Unused,
                this.toolStripMenuItemExtInput4BankA,
                this.toolStripMenuItemExtInput4BankB,
                this.toolStripMenuItemExtInput4LaserStop,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetExternalInput4;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemExtInput4Unused_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput4Show,
                this.toolStripMenuItemExtInput4Unused,
                this.toolStripMenuItemExtInput4BankA,
                this.toolStripMenuItemExtInput4BankB,
                this.toolStripMenuItemExtInput4LaserStop,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput4;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput4BankA_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput4Show,
                this.toolStripMenuItemExtInput4Unused,
                this.toolStripMenuItemExtInput4BankA,
                this.toolStripMenuItemExtInput4BankB,
                this.toolStripMenuItemExtInput4LaserStop,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput4;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput4BankB_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput4Show,
                this.toolStripMenuItemExtInput4Unused,
                this.toolStripMenuItemExtInput4BankA,
                this.toolStripMenuItemExtInput4BankB,
                this.toolStripMenuItemExtInput4LaserStop,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput4;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemExtInput4LaserStop_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemExtInput4Show,
                this.toolStripMenuItemExtInput4Unused,
                this.toolStripMenuItemExtInput4BankA,
                this.toolStripMenuItemExtInput4BankB,
                this.toolStripMenuItemExtInput4LaserStop,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetExternalInput4;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemOutputDigitalShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputDigitalHigh,
                this.toolStripMenuItemOutputDigitalLow,
                this.toolStripMenuItemOutputDigitalGo,
                this.toolStripMenuItemOutputDigitalAlarm,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetJudgementAlarmOutput;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemOutputModeShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputModeNO,
                this.toolStripMenuItemOutputModeNC,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetOutputMode;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemOutputModeNO_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputModeShow,
                this.toolStripMenuItemOutputModeNO,
                this.toolStripMenuItemOutputModeNC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetOutputMode;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemOutputModeNC_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputModeShow,
                this.toolStripMenuItemOutputModeNO,
                this.toolStripMenuItemOutputModeNC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetOutputMode;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemOutputAnalogRead_Click(object sender, EventArgs e) {
            ToolStripTextBox textbox = this.toolStripTextBoxOutputAnalogValue;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetAnalogOutputValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0:+#0;-#0;0}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemOutputAnalogScalingShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputAnalogScalingInitial,
                this.toolStripMenuItemOutputAnalogScalingFreeRange,
                this.toolStripMenuItemOutputAnalogScalingBank,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetAnalogOutputScaling;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemOutputAnalogScalingInitial_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputAnalogScalingShow,
                this.toolStripMenuItemOutputAnalogScalingInitial,
                this.toolStripMenuItemOutputAnalogScalingFreeRange,
                this.toolStripMenuItemOutputAnalogScalingBank,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetAnalogOutputScaling;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemOutputAnalogScalingFreeRange_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputAnalogScalingShow,
                this.toolStripMenuItemOutputAnalogScalingInitial,
                this.toolStripMenuItemOutputAnalogScalingFreeRange,
                this.toolStripMenuItemOutputAnalogScalingBank,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetAnalogOutputScaling;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemOutputAnalogScalingBank_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemOutputAnalogScalingShow,
                this.toolStripMenuItemOutputAnalogScalingInitial,
                this.toolStripMenuItemOutputAnalogScalingFreeRange,
                this.toolStripMenuItemOutputAnalogScalingBank,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetAnalogOutputScaling;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void displaySystemParameters(
            ToolStripMenuItem menuItem,
            Action<Action<int?>?, Action<Exception>?> proxyCall
        ) {
            Action<int?>? receiveFunc = (int? value) => {
                if (value == null) {
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                } else {
                    this.Invoke(() => {
                        int bit0 = (int)value & 0b0001;
                        int bit123 = ((int)value & 0b1110) >> 1;
                        this.toolStripComboBoxSystemParametersPolarity.SelectedIndex = bit0;
                        this.toolStripComboBoxSystemParametersAnalog.SelectedIndex = bit123;

                        menuItem.Image = Properties.Resources.green_circle;
                    });
                }
            };

            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
            };

            Action action = () => {
                this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }

        private void toolStripMenuItemSystemParametersCurrent_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSystemParametersCurrent,
                this.toolStripMenuItemSystemParametersFutureRead,
                this.toolStripMenuItemSystemParametersFutureWrite,
                this.toolStripMenuItemSystemParametersFutureActivate,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCurrentSystemParameters;
            this.displaySystemParameters(menuItem, proxyCall);
        }

        private void toolStripMenuItemSystemParametersFutureRead_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSystemParametersCurrent,
                this.toolStripMenuItemSystemParametersFutureRead,
                this.toolStripMenuItemSystemParametersFutureWrite,
                this.toolStripMenuItemSystemParametersFutureActivate,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetSystemParameterSettings;
            this.displaySystemParameters(menuItem, proxyCall);
        }

        private void toolStripMenuItemSystemParametersFutureWrite_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSystemParametersCurrent,
                this.toolStripMenuItemSystemParametersFutureRead,
                this.toolStripMenuItemSystemParametersFutureWrite,
                this.toolStripMenuItemSystemParametersFutureActivate,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }
            int bit0 = this.toolStripComboBoxSystemParametersPolarity.SelectedIndex;
            int bit123 = this.toolStripComboBoxSystemParametersAnalog.SelectedIndex;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            if ((bit0 < 0) || (bit123 < 0)) {
                menuItem.Image = Properties.Resources.red_circle;
                return;
            }
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSystemParameterSettings;
            int value = (bit123 << 1) + bit0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSystemParametersFutureActivate_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show(
                "Changing this setting may damage the sensor amplifier and/or connected equipment! Are you sure you want to proceed?",
                "Potentially dangerous action detected!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );
            if (result == DialogResult.No) {
                return;
            }

            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSystemParametersCurrent,
                this.toolStripMenuItemSystemParametersFutureRead,
                this.toolStripMenuItemSystemParametersFutureWrite,
                this.toolStripMenuItemSystemParametersFutureActivate,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSystemParametersSetRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemValuesRV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemValuesRV,
                this.toolStripMenuItemValuesPV,
                this.toolStripMenuItemValuesCALC,
                this.toolStripMenuItemValuesPeakHold,
                this.toolStripMenuItemValuesBottomHold,
                this.toolStripMenuItemValuesAnalog,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetInternalMeasurementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemValuesPV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemValuesRV,
                this.toolStripMenuItemValuesPV,
                this.toolStripMenuItemValuesCALC,
                this.toolStripMenuItemValuesPeakHold,
                this.toolStripMenuItemValuesBottomHold,
                this.toolStripMenuItemValuesAnalog,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetJudgementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemValuesCALC_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemValuesRV,
                this.toolStripMenuItemValuesPV,
                this.toolStripMenuItemValuesCALC,
                this.toolStripMenuItemValuesPeakHold,
                this.toolStripMenuItemValuesBottomHold,
                this.toolStripMenuItemValuesAnalog,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalculationValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemValuesPeakHold_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemValuesRV,
                this.toolStripMenuItemValuesPV,
                this.toolStripMenuItemValuesCALC,
                this.toolStripMenuItemValuesPeakHold,
                this.toolStripMenuItemValuesBottomHold,
                this.toolStripMenuItemValuesAnalog,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetPeakHoldValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemValuesBottomHold_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemValuesRV,
                this.toolStripMenuItemValuesPV,
                this.toolStripMenuItemValuesCALC,
                this.toolStripMenuItemValuesPeakHold,
                this.toolStripMenuItemValuesBottomHold,
                this.toolStripMenuItemValuesAnalog,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetBottomHoldValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemValuesAnalog_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemValuesRV,
                this.toolStripMenuItemValuesPV,
                this.toolStripMenuItemValuesCALC,
                this.toolStripMenuItemValuesPeakHold,
                this.toolStripMenuItemValuesBottomHold,
                this.toolStripMenuItemValuesAnalog,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetAnalogOutputValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0:+#0;-#0;0}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemFilterShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetFilterSetting;

            Bitmap bitmapActive = Properties.Resources.blue_circle;
            Bitmap bitmapInactive = Properties.Resources.grey_circle;
            Action<int?>? receiveFunc = (int? value) => {
                if (value == null) {
                    this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
                } else {
                    this.Invoke(() => {
                        menuItem.Image = Properties.Resources.green_circle;
                        if (value <= 12) {
                            this.toolStripMenuItemFilterAverage.ShowDropDown();
                            this.toolStripMenuItemFilterAverage.Image = bitmapActive;
                        } else {
                            this.toolStripMenuItemFilterAverage.Image = bitmapInactive;
                        }
                        for (int i = 0; i < list.Count; i++) {
                            if (i == value) {
                                list[i].Image = bitmapActive;
                            } else {
                                list[i].Image = bitmapInactive;
                            }
                        }
                    });
                }
            };

            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => { menuItem.Image = Properties.Resources.red_circle; });
            };

            Action action = () => {
                this.Invoke(() => { menuItem.Image = Properties.Resources.yellow_circle; });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }
        }

        private void toolStripMenuItemAvg1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg4_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg8_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg16_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg32_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 5;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg64_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 6;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg128_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 7;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg256_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 8;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg512_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 9;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg1024_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 10;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg2048_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 11;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemAvg4096_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 12;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterDiffCountSet_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 13;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterHighPassSet_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemAvg1,
                this.toolStripMenuItemAvg2,
                this.toolStripMenuItemAvg4,
                this.toolStripMenuItemAvg8,
                this.toolStripMenuItemAvg16,
                this.toolStripMenuItemAvg32,
                this.toolStripMenuItemAvg64,
                this.toolStripMenuItemAvg128,
                this.toolStripMenuItemAvg256,
                this.toolStripMenuItemAvg512,
                this.toolStripMenuItemAvg1024,
                this.toolStripMenuItemAvg2048,
                this.toolStripMenuItemAvg4096,
                this.toolStripMenuItemFilterDiffCount,
                this.toolStripMenuItemFilterHighPass,
                this.toolStripMenuItemFilterAverage,
                this.toolStripMenuItemFilterDiffCountSet,
                this.toolStripMenuItemFilterHighPassSet,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetFilterSetting;
            int value = 14;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOffShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCutoffFrequencyHighPassFilter;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemFilterCutOff0p1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff0p2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff0p5_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff5_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 5;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff10_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 6;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff20_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 7;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff50_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 8;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemFilterCutOff100_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemFilterCutOffShow,
                this.toolStripMenuItemFilterCutOff0p1,
                this.toolStripMenuItemFilterCutOff0p2,
                this.toolStripMenuItemFilterCutOff0p5,
                this.toolStripMenuItemFilterCutOff1,
                this.toolStripMenuItemFilterCutOff2,
                this.toolStripMenuItemFilterCutOff5,
                this.toolStripMenuItemFilterCutOff10,
                this.toolStripMenuItemFilterCutOff20,
                this.toolStripMenuItemFilterCutOff50,
                this.toolStripMenuItemFilterCutOff100,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCutoffFrequencyHighPassFilter;
            int value = 9;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHoldingShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetHoldFunctionSetting;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemHoldingSample_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingShow,
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHoldFunctionSetting;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHoldingPeak_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingShow,
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHoldFunctionSetting;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHoldingBottom_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingShow,
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHoldFunctionSetting;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHoldingPeakToPeak_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingShow,
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHoldFunctionSetting;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHoldingAutoPeak_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingShow,
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHoldFunctionSetting;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHoldingAutoBottom_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHoldingShow,
                this.toolStripMenuItemHoldingSample,
                this.toolStripMenuItemHoldingPeak,
                this.toolStripMenuItemHoldingBottom,
                this.toolStripMenuItemHoldingPeakToPeak,
                this.toolStripMenuItemHoldingAutoPeak,
                this.toolStripMenuItemHoldingAutoBottom,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHoldFunctionSetting;
            int value = 5;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingRateShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingRateDefault,
                this.toolStripMenuItemSamplingRate0p33,
                this.toolStripMenuItemSamplingRate1,
                this.toolStripMenuItemSamplingRate2,
                this.toolStripMenuItemSamplingRate5,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetSamplingCycle;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemSamplingRateDefault_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingRateShow,
                this.toolStripMenuItemSamplingRateDefault,
                this.toolStripMenuItemSamplingRate0p33,
                this.toolStripMenuItemSamplingRate1,
                this.toolStripMenuItemSamplingRate2,
                this.toolStripMenuItemSamplingRate5,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSamplingCycle;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingRate0p33_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingRateShow,
                this.toolStripMenuItemSamplingRateDefault,
                this.toolStripMenuItemSamplingRate0p33,
                this.toolStripMenuItemSamplingRate1,
                this.toolStripMenuItemSamplingRate2,
                this.toolStripMenuItemSamplingRate5,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSamplingCycle;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingRate1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingRateShow,
                this.toolStripMenuItemSamplingRateDefault,
                this.toolStripMenuItemSamplingRate0p33,
                this.toolStripMenuItemSamplingRate1,
                this.toolStripMenuItemSamplingRate2,
                this.toolStripMenuItemSamplingRate5,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSamplingCycle;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingRate2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingRateShow,
                this.toolStripMenuItemSamplingRateDefault,
                this.toolStripMenuItemSamplingRate0p33,
                this.toolStripMenuItemSamplingRate1,
                this.toolStripMenuItemSamplingRate2,
                this.toolStripMenuItemSamplingRate5,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSamplingCycle;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingRate5_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingRateShow,
                this.toolStripMenuItemSamplingRateDefault,
                this.toolStripMenuItemSamplingRate0p33,
                this.toolStripMenuItemSamplingRate1,
                this.toolStripMenuItemSamplingRate2,
                this.toolStripMenuItemSamplingRate5,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSamplingCycle;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingAlarmShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingAlarmDefault,
                this.toolStripMenuItemSamplingAlarmClamp,
                this.toolStripMenuItemSamplingAlarmUser,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetAlarmSetting;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemSamplingAlarmDefault_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingAlarmShow,
                this.toolStripMenuItemSamplingAlarmDefault,
                this.toolStripMenuItemSamplingAlarmClamp,
                this.toolStripMenuItemSamplingAlarmUser,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetAlarmSetting;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingAlarmClamp_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingAlarmShow,
                this.toolStripMenuItemSamplingAlarmDefault,
                this.toolStripMenuItemSamplingAlarmClamp,
                this.toolStripMenuItemSamplingAlarmUser,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetAlarmSetting;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSamplingAlarmUser_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSamplingAlarmShow,
                this.toolStripMenuItemSamplingAlarmDefault,
                this.toolStripMenuItemSamplingAlarmClamp,
                this.toolStripMenuItemSamplingAlarmUser,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetAlarmSetting;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingSettingShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingSettingLevel,
                this.toolStripMenuItemTimingSettingEdge,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetTimingInputSetting;
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemTimingSettingLevel_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingSettingShow,
                this.toolStripMenuItemTimingSettingLevel,
                this.toolStripMenuItemTimingSettingEdge,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetTimingInputSetting;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingSettingEdge_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingSettingShow,
                this.toolStripMenuItemTimingSettingLevel,
                this.toolStripMenuItemTimingSettingEdge,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetTimingInputSetting;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingInputCurrent_Click(object sender, EventArgs e) {
            this.toolStripMenuItemTimingInputCurrent.Image = null;
            this.toolStripMenuItem2TimingInputSelected.Image = null;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetTimingStatus;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingInputOff,
                this.toolStripMenuItemTimingInputOn,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItem2TimingInputSelected_Click(object sender, EventArgs e) {
            this.toolStripMenuItemTimingInputCurrent.Image = null;
            this.toolStripMenuItem2TimingInputSelected.Image = null;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetTimingInput;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingInputOff,
                this.toolStripMenuItemTimingInputOn,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemTimingInputOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingInputCurrent,
                this.toolStripMenuItem2TimingInputSelected,
                this.toolStripMenuItemTimingInputOff,
                this.toolStripMenuItemTimingInputOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetTimingInput;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingInputOn_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingInputCurrent,
                this.toolStripMenuItem2TimingInputSelected,
                this.toolStripMenuItemTimingInputOff,
                this.toolStripMenuItemTimingInputOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetTimingInput;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingDelayShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetDelayTimer;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingDelayOff,
                this.toolStripMenuItemTimingDelayOnDelay,
                this.toolStripMenuItemTimingDelayOffDelay,
                this.toolStripMenuItemTimingDelayOneShot,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemTimingDelayOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingDelayShow,
                this.toolStripMenuItemTimingDelayOff,
                this.toolStripMenuItemTimingDelayOnDelay,
                this.toolStripMenuItemTimingDelayOffDelay,
                this.toolStripMenuItemTimingDelayOneShot,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDelayTimer;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingDelayOnDelay_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingDelayShow,
                this.toolStripMenuItemTimingDelayOff,
                this.toolStripMenuItemTimingDelayOnDelay,
                this.toolStripMenuItemTimingDelayOffDelay,
                this.toolStripMenuItemTimingDelayOneShot,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDelayTimer;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingDelayOffDelay_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingDelayShow,
                this.toolStripMenuItemTimingDelayOff,
                this.toolStripMenuItemTimingDelayOnDelay,
                this.toolStripMenuItemTimingDelayOffDelay,
                this.toolStripMenuItemTimingDelayOneShot,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDelayTimer;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTimingDelayOneShot_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTimingDelayShow,
                this.toolStripMenuItemTimingDelayOff,
                this.toolStripMenuItemTimingDelayOnDelay,
                this.toolStripMenuItemTimingDelayOffDelay,
                this.toolStripMenuItemTimingDelayOneShot,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDelayTimer;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuZeroShiftMemoryShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetZeroShiftMemoryFunction;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemZeroShiftMemoryOff,
                this.toolStripMenuItemZeroShiftMemoryOn,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemZeroShiftMemoryOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuZeroShiftMemoryShow,
                this.toolStripMenuItemZeroShiftMemoryOff,
                this.toolStripMenuItemZeroShiftMemoryOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetZeroShiftMemoryFunction;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemZeroShiftMemoryOn_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuZeroShiftMemoryShow,
                this.toolStripMenuItemZeroShiftMemoryOff,
                this.toolStripMenuItemZeroShiftMemoryOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetZeroShiftMemoryFunction;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemLaserCurrent_Click(object sender, EventArgs e) {
            this.toolStripMenuItemLaserCurrent.Image = null;
            this.toolStripMenuItemLaserSelected.Image = null;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetLaserEmissionStopStatus;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemLaserOff,
                this.toolStripMenuItemLaserOn,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemLaserSelected_Click(object sender, EventArgs e) {
            this.toolStripMenuItemLaserCurrent.Image = null;
            this.toolStripMenuItemLaserSelected.Image = null;
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetLaserEmissionStop;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemLaserOff,
                this.toolStripMenuItemLaserOn,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemLaserOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemLaserCurrent,
                this.toolStripMenuItemLaserSelected,
                this.toolStripMenuItemLaserOff,
                this.toolStripMenuItemLaserOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetLaserEmissionStop;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemLaserOn_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemLaserCurrent,
                this.toolStripMenuItemLaserSelected,
                this.toolStripMenuItemLaserOff,
                this.toolStripMenuItemLaserOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetLaserEmissionStop;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }
        private void toolStripMenuItemZeroShiftExecution_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendZeroShiftExecutionRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemZeroShiftReset_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendZeroShiftResetRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemReset_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendResetRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemInitialReset_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendInitialResetRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }


        private void toolStripMenuItemStatusSettings_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetAbnormalSetting;
            this.highlightStatus(menuItem, proxyCall, 1);
        }

        private void toolStripMenuItemStatusEEPROM_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetEEPROMWriteResult;
            this.highlightStatus(menuItem, proxyCall);
        }

        private void toolStripMenuItemStatusZeroShift_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetZeroShiftResult;
            this.highlightStatus(menuItem, proxyCall);
        }

        private void toolStripMenuItemStatusReset_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetResetRequestResult;
            this.highlightStatus(menuItem, proxyCall);
        }

        private void toolStripMenuItemStatusTuning_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetTuningResult;
            this.highlightStatus(menuItem, proxyCall);
        }

        private void toolStripMenuItemStatusCalibration_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalibrationResult;
            this.highlightStatus(menuItem, proxyCall);
        }

        private void toolStripMenuItemInfoProductCode_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetProductCode;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemInfoRevision_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetRevision;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0:X}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);

        }

        private void toolStripMenuItemInfoConnectedSensorHead_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetConnectedSensorHead;

            Dictionary<int, string> sensorHeads = new() {
                [0] = "Not connected",
                [1] = "IL-030",
                [2] = "IL-065",
                [3] = "IL-100",
                [4] = "IL-300",
                [5] = "IL-600",
                [106] = "IL-S025",
                [107] = "IL-S065",
                [208] = "IL-S100",
                [311] = "IL-2000",
            };

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = sensorHeads.GetValueOrDefault<int, string>((int)value, $"Unknown Value: {value}");
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemInfoProductName_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<string?>?, Action<Exception>?> proxyCall = this.proxy.sendGetProductName;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;

            Action<string?> receiveFunc = (string? text) => {
                if (text != null) {
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };

            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };

            Action action = () => {
                this.Invoke(() => {
                    textbox.Text = "Waiting for result...";
                    menuItem.Image = Properties.Resources.yellow_circle;
                });
                proxyCall(receiveFunc, excHandler);
            };
            if (!this.backgroundWorkerCommunication.IsBusy) {
                this.backgroundWorkerCommunication.RunWorkerAsync(action);
            }

        }

        private void toolStripMenuItemInfoSeriesCode_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetSeriesCode;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemInfoSeriesVersion_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetSeriesVersion;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemInfoDeviceType_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInfoProductCode,
                this.toolStripMenuItemInfoRevision,
                this.toolStripMenuItemInfoConnectedSensorHead,
                this.toolStripMenuItemInfoProductName,
                this.toolStripMenuItemInfoSeriesCode,
                this.toolStripMenuItemInfoSeriesVersion,
                this.toolStripMenuItemInfoDeviceType,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxInfo;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetDeviceType;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = string.Format("{0}", value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemKeyLockShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetKeyLock;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemKeyLockUnlocked,
                this.toolStripMenuItemKeyLockLocked,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemKeyLockUnlocked_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemKeyLockShow,
                this.toolStripMenuItemKeyLockUnlocked,
                this.toolStripMenuItemKeyLockLocked,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetKeyLock;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemKeyLockLocked_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemKeyLockShow,
                this.toolStripMenuItemKeyLockUnlocked,
                this.toolStripMenuItemKeyLockLocked,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetKeyLock;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSubDisplayShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetSubDisplayScreen;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemSubDisplayRV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSubDisplayScreen;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSubDisplayAnalog_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSubDisplayScreen;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSubDisplayHIGH_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSubDisplayScreen;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSubDisplayLOW_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSubDisplayScreen;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSubDisplayZeroShift_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSubDisplayScreen;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSubDisplayCALC_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSubDisplayRV,
                this.toolStripMenuItemSubDisplayAnalog,
                this.toolStripMenuItemSubDisplayHIGH,
                this.toolStripMenuItemSubDisplayLOW,
                this.toolStripMenuItemSubDisplayZeroShift,
                this.toolStripMenuItemSubDisplayCALC,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetSubDisplayScreen;
            int value = 5;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDirectionShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetMeasurementDirection;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDirectionNormal,
                this.toolStripMenuItemDirectionReversed,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemDirectionNormal_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDirectionShow,
                this.toolStripMenuItemDirectionNormal,
                this.toolStripMenuItemDirectionReversed,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetMeasurementDirection;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDirectionReversed_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDirectionShow,
                this.toolStripMenuItemDirectionNormal,
                this.toolStripMenuItemDirectionReversed,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetMeasurementDirection;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemInterferenceShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetMutualInterferencePreventionFunction;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInterferenceOff,
                this.toolStripMenuItemInterferenceOn,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemInterferenceOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInterferenceShow,
                this.toolStripMenuItemInterferenceOff,
                this.toolStripMenuItemInterferenceOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetMutualInterferencePreventionFunction;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemInterferenceOn_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemInterferenceShow,
                this.toolStripMenuItemInterferenceOff,
                this.toolStripMenuItemInterferenceOn,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetMutualInterferencePreventionFunction;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayDigitShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetDisplayDigit;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayDigitDefault,
                this.toolStripMenuItemDisplayDigit0p001,
                this.toolStripMenuItemDisplayDigit0p01,
                this.toolStripMenuItemDisplayDigit0p1,
                this.toolStripMenuItemDisplayDigit1,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemDisplayDigitDefault_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayDigitShow,
                this.toolStripMenuItemDisplayDigitDefault,
                this.toolStripMenuItemDisplayDigit0p001,
                this.toolStripMenuItemDisplayDigit0p01,
                this.toolStripMenuItemDisplayDigit0p1,
                this.toolStripMenuItemDisplayDigit1,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayDigit;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayDigit0p001_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayDigitShow,
                this.toolStripMenuItemDisplayDigitDefault,
                this.toolStripMenuItemDisplayDigit0p001,
                this.toolStripMenuItemDisplayDigit0p01,
                this.toolStripMenuItemDisplayDigit0p1,
                this.toolStripMenuItemDisplayDigit1,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayDigit;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayDigit0p01_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayDigitShow,
                this.toolStripMenuItemDisplayDigitDefault,
                this.toolStripMenuItemDisplayDigit0p001,
                this.toolStripMenuItemDisplayDigit0p01,
                this.toolStripMenuItemDisplayDigit0p1,
                this.toolStripMenuItemDisplayDigit1,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayDigit;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayDigit0p1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayDigitShow,
                this.toolStripMenuItemDisplayDigitDefault,
                this.toolStripMenuItemDisplayDigit0p001,
                this.toolStripMenuItemDisplayDigit0p01,
                this.toolStripMenuItemDisplayDigit0p1,
                this.toolStripMenuItemDisplayDigit1,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayDigit;
            int value = 3;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayDigit1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayDigitShow,
                this.toolStripMenuItemDisplayDigitDefault,
                this.toolStripMenuItemDisplayDigit0p001,
                this.toolStripMenuItemDisplayDigit0p01,
                this.toolStripMenuItemDisplayDigit0p1,
                this.toolStripMenuItemDisplayDigit1,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayDigit;
            int value = 4;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemPowerSavingShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetPowerSaving;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemPowerSavingOff,
                this.toolStripMenuItemPowerSavingHalf,
                this.toolStripMenuItemPowerSavingAll,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemPowerSavingOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemPowerSavingShow,
                this.toolStripMenuItemPowerSavingOff,
                this.toolStripMenuItemPowerSavingHalf,
                this.toolStripMenuItemPowerSavingAll,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetPowerSaving;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemPowerSavingHalf_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemPowerSavingShow,
                this.toolStripMenuItemPowerSavingOff,
                this.toolStripMenuItemPowerSavingHalf,
                this.toolStripMenuItemPowerSavingAll,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetPowerSaving;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemPowerSavingAll_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemPowerSavingShow,
                this.toolStripMenuItemPowerSavingOff,
                this.toolStripMenuItemPowerSavingHalf,
                this.toolStripMenuItemPowerSavingAll,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetPowerSaving;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHeadDisplayShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetHeadDisplayMode;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHeadDisplayDefault,
                this.toolStripMenuItemHeadDisplayOKNG,
                this.toolStripMenuItemHeadDisplayOff,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemHeadDisplayDefault_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHeadDisplayShow,
                this.toolStripMenuItemHeadDisplayDefault,
                this.toolStripMenuItemHeadDisplayOKNG,
                this.toolStripMenuItemHeadDisplayOff,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHeadDisplayMode;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHeadDisplayOKNG_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHeadDisplayShow,
                this.toolStripMenuItemHeadDisplayDefault,
                this.toolStripMenuItemHeadDisplayOKNG,
                this.toolStripMenuItemHeadDisplayOff,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHeadDisplayMode;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemHeadDisplayOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemHeadDisplayShow,
                this.toolStripMenuItemHeadDisplayDefault,
                this.toolStripMenuItemHeadDisplayOKNG,
                this.toolStripMenuItemHeadDisplayOff,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetHeadDisplayMode;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayColorShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetDisplayColor;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayColorGoGreen,
                this.toolStripMenuItemDisplayColorGoRed,
                this.toolStripMenuItemDisplayColorAlwaysRed,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemDisplayColorGoGreen_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayColorShow,
                this.toolStripMenuItemDisplayColorGoGreen,
                this.toolStripMenuItemDisplayColorGoRed,
                this.toolStripMenuItemDisplayColorAlwaysRed,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayColor;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayColorGoRed_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayColorShow,
                this.toolStripMenuItemDisplayColorGoGreen,
                this.toolStripMenuItemDisplayColorGoRed,
                this.toolStripMenuItemDisplayColorAlwaysRed,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayColor;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemDisplayColorAlwaysRed_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemDisplayColorShow,
                this.toolStripMenuItemDisplayColorGoGreen,
                this.toolStripMenuItemDisplayColorGoRed,
                this.toolStripMenuItemDisplayColorAlwaysRed,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetDisplayColor;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningOnePointPV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningOnePointPV,
                this.toolStripMenuItemTuningOnePointRequest,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxTuningOnePointPV;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetJudgementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemTuningOnePointRequest_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningOnePointPV,
                this.toolStripMenuItemTuningOnePointRequest,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendToleranceTuningRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningTwoPointRV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningTwoPointRV,
                this.toolStripMenuItemTuningTwoPointHIGH1,
                this.toolStripMenuItemTuningTwoPointHIGH2,
                this.toolStripMenuItemTuningTwoPointLOW1,
                this.toolStripMenuItemTuningTwoPointLOW2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxTuningTwoPointRV;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetInternalMeasurementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemTuningTwoPointHIGH1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningTwoPointRV,
                this.toolStripMenuItemTuningTwoPointHIGH1,
                this.toolStripMenuItemTuningTwoPointHIGH2,
                this.toolStripMenuItemTuningTwoPointLOW1,
                this.toolStripMenuItemTuningTwoPointLOW2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendTwoPointHIGHFirstPointRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningTwoPointHIGH2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningTwoPointRV,
                this.toolStripMenuItemTuningTwoPointHIGH1,
                this.toolStripMenuItemTuningTwoPointHIGH2,
                this.toolStripMenuItemTuningTwoPointLOW1,
                this.toolStripMenuItemTuningTwoPointLOW2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendTwoPointHIGHSecondPointRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningTwoPointLOW1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningTwoPointRV,
                this.toolStripMenuItemTuningTwoPointHIGH1,
                this.toolStripMenuItemTuningTwoPointHIGH2,
                this.toolStripMenuItemTuningTwoPointLOW1,
                this.toolStripMenuItemTuningTwoPointLOW2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendTwoPointLOWFirstPointRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningTwoPointLOW2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningTwoPointRV,
                this.toolStripMenuItemTuningTwoPointHIGH1,
                this.toolStripMenuItemTuningTwoPointHIGH2,
                this.toolStripMenuItemTuningTwoPointLOW1,
                this.toolStripMenuItemTuningTwoPointLOW2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendTwoPointLOWSecondPointRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningStepCountOnePointPV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningStepCountOnePointPV,
                this.toolStripMenuItemTuningStepCountOnePointRequest,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxTuningStepCountOnePointPV;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetJudgementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemTuningStepCountOnePointRequest_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningStepCountOnePointPV,
                this.toolStripMenuItemTuningStepCountOnePointRequest,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendDiffCountFilterOnePointTuningRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningStepCountTwoPointRV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningStepCountTwoPointRV,
                this.toolStripMenuItemTuningStepCountTwoPointStep1,
                this.toolStripMenuItemTuningStepCountTwoPointStep2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxTuningStepCountTwoPointRV;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetInternalMeasurementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemTuningStepCountTwoPointStep1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningStepCountTwoPointRV,
                this.toolStripMenuItemTuningStepCountTwoPointStep1,
                this.toolStripMenuItemTuningStepCountTwoPointStep2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendDiffCountFilterTwoPointFirstPointRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemTuningStepCountTwoPointStep2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemTuningStepCountTwoPointRV,
                this.toolStripMenuItemTuningStepCountTwoPointStep1,
                this.toolStripMenuItemTuningStepCountTwoPointStep2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendDiffCountFilterTwoPointSecondPointRequest;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationFunctionShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalculationFunction;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationFunctionOff,
                this.toolStripMenuItemCalculationFunctionAddition,
                this.toolStripMenuItemCalculationFunctionSubtraction,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemCalculationFunctionOff_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationFunctionShow,
                this.toolStripMenuItemCalculationFunctionOff,
                this.toolStripMenuItemCalculationFunctionAddition,
                this.toolStripMenuItemCalculationFunctionSubtraction,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalculationFunction;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationFunctionAddition_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationFunctionShow,
                this.toolStripMenuItemCalculationFunctionOff,
                this.toolStripMenuItemCalculationFunctionAddition,
                this.toolStripMenuItemCalculationFunctionSubtraction,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalculationFunction;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationFunctionSubtraction_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationFunctionShow,
                this.toolStripMenuItemCalculationFunctionOff,
                this.toolStripMenuItemCalculationFunctionAddition,
                this.toolStripMenuItemCalculationFunctionSubtraction,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalculationFunction;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalibrationRV_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalibrationRV,
                this.toolStripMenuItemCalibrationReal1,
                this.toolStripMenuItemCalibrationReal2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCalibrationRV;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetInternalMeasurementValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemCalibrationReal1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalibrationRV,
                this.toolStripMenuItemCalibrationReal1,
                this.toolStripMenuItemCalibrationReal2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalibrationSet1Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);

        }

        private void toolStripMenuItemCalibrationReal2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalibrationRV,
                this.toolStripMenuItemCalibrationReal1,
                this.toolStripMenuItemCalibrationReal2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalibrationSet2Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationCalibrationFunctionShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalculatedValueCalibrationFunction;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationCalibrationFunctionDefault,
                this.toolStripMenuItemCalculationCalibrationFunctionTwoPoint,
                this.toolStripMenuItemCalculationCalibrationFunctionThreePoint,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemCalculationCalibrationFunctionDefault_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationCalibrationFunctionShow,
                this.toolStripMenuItemCalculationCalibrationFunctionDefault,
                this.toolStripMenuItemCalculationCalibrationFunctionTwoPoint,
                this.toolStripMenuItemCalculationCalibrationFunctionThreePoint,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalculatedValueCalibrationFunction;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationCalibrationFunctionTwoPoint_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationCalibrationFunctionShow,
                this.toolStripMenuItemCalculationCalibrationFunctionDefault,
                this.toolStripMenuItemCalculationCalibrationFunctionTwoPoint,
                this.toolStripMenuItemCalculationCalibrationFunctionThreePoint,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalculatedValueCalibrationFunction;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationCalibrationFunctionThreePoint_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationCalibrationFunctionShow,
                this.toolStripMenuItemCalculationCalibrationFunctionDefault,
                this.toolStripMenuItemCalculationCalibrationFunctionTwoPoint,
                this.toolStripMenuItemCalculationCalibrationFunctionThreePoint,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalculatedValueCalibrationFunction;
            int value = 2;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationTwoPointCALC_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationTwoPointCALC,
                this.toolStripMenuItemCalculationTwoPointReal1,
                this.toolStripMenuItemCalculationTwoPointReal2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCalculationTwoPointCALC;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalculationValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemCalculationTwoPointReal1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationTwoPointCALC,
                this.toolStripMenuItemCalculationTwoPointReal1,
                this.toolStripMenuItemCalculationTwoPointReal2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalculationTwoPointCalibrationSet1Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationTwoPointReal2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationTwoPointCALC,
                this.toolStripMenuItemCalculationTwoPointReal1,
                this.toolStripMenuItemCalculationTwoPointReal2,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalculationTwoPointCalibrationSet2Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationThreePointCALC_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationThreePointCALC,
                this.toolStripMenuItemCalculationThreePointSet1,
                this.toolStripMenuItemCalculationThreePointSet2,
                this.toolStripMenuItemCalculationThreePointSet3,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCalculationThreePointCALC;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalculationValue;

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int?> updateTextbox = (int? value) => {
                if (value != null) {
                    string text = this.formatValue((int)value);
                    this.Invoke(() => {
                        textbox.Text = text;
                        menuItem.Image = Properties.Resources.green_circle;
                    });
                } else {
                    this.Invoke(() => {
                        textbox.Text = "No Value!";
                        menuItem.Image = Properties.Resources.red_circle;
                    });
                }
            };
            Action<Exception> excHandler = (ex) => {
                if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                    this.proxy.printSocketException((SocketException)ex);
                } else {
                    this.messageCollection.addLine(ex.ToString());
                }
                this.Invoke(() => {
                    menuItem.Image = Properties.Resources.red_circle;
                    textbox.Text = "Error!";
                });
            };
            textbox.Text = "Waiting for result...";
            this.contextMenuEntryClickRead(menuItem, proxyCall, updateTextbox, excHandler);
        }

        private void toolStripMenuItemCalculationThreePointSet1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationThreePointCALC,
                this.toolStripMenuItemCalculationThreePointSet1,
                this.toolStripMenuItemCalculationThreePointSet2,
                this.toolStripMenuItemCalculationThreePointSet3,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalculationThreePointCalibrationSet1Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationThreePointSet2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationThreePointCALC,
                this.toolStripMenuItemCalculationThreePointSet1,
                this.toolStripMenuItemCalculationThreePointSet2,
                this.toolStripMenuItemCalculationThreePointSet3,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalculationThreePointCalibrationSet2Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCalculationThreePointSet3_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCalculationThreePointCALC,
                this.toolStripMenuItemCalculationThreePointSet1,
                this.toolStripMenuItemCalculationThreePointSet2,
                this.toolStripMenuItemCalculationThreePointSet3,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendCalculationThreePointCalibrationSet3Request;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSensorCalibrationFunctionShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.proxy.sendGetCalibrationFunction;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSensorCalibrationFunctionDefault,
                this.toolStripMenuItemSensorCalibrationFunctionUserSetting,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemSensorCalibrationFunctionDefault_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSensorCalibrationFunctionShow,
                this.toolStripMenuItemSensorCalibrationFunctionDefault,
                this.toolStripMenuItemSensorCalibrationFunctionUserSetting,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalibrationFunction;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemSensorCalibrationFunctionUserSetting_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemSensorCalibrationFunctionShow,
                this.toolStripMenuItemSensorCalibrationFunctionDefault,
                this.toolStripMenuItemSensorCalibrationFunctionUserSetting,
              };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.proxy.sendSetCalibrationFunction;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }
    }
}
