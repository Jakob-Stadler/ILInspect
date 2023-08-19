using System.ComponentModel;
using System.Net.Sockets;

namespace ILInspect {

    public partial class ConnectionContextMenu : UserControl {
        private MessageCollection messageCollection;
        private Connection connection;

        public ConnectionContextMenu(Connection connection, MessageCollection messageCollection) {
            this.connection = connection;
            this.messageCollection = messageCollection;
            this.InitializeComponent();
            this.createAllSubmenus();
        }

        private void createAllSubmenus() {
            this.createAllCommIDValuesSubmenus();
        }

        private void createCommIDValuesSubmenu(ToolStripMenuItem item, int id) {
            string postfix = $"_{item.Name}";

            ContextMenuStrip ctxMenu;
            ToolStripMenuItem itemValuesCurrent;
            ToolStripTextBox textboxValuesCurrent;
            ToolStripSeparator separator;
            ToolStripMenuItem itemValuesError;
            ToolStripTextBox textboxValuesError;

            Action<Action<int?>?, Action<Exception>?> proxyCall;

            ctxMenu = new ContextMenuStrip(this.components);
            itemValuesCurrent = new();
            textboxValuesCurrent = new();
            separator = new();
            itemValuesError = new();
            textboxValuesError = new();
            ctxMenu.SuspendLayout();

            List<ToolStripMenuItem> list = new() {
                itemValuesCurrent,
                itemValuesError,
            };

            Func<int, string> formatFunc = (value) => {
                switch (value) {
                    case 99999999:
                        return "Over Range";
                    case -99999999:
                        return "Under Range";
                    case -99999998:
                        return "Invalid!";
                    case 100000000:
                        return "Error!";
                    default:
                        SensorProxy? relevantProxy = this.connection.getProxyByID(id);
                        double convFactor;
                        if (relevantProxy == null) {
                            convFactor = 1;
                        } else {
                            convFactor = relevantProxy.config.ConversionFactor;
                        }
                        int numberZeros = (int)Math.Round(Math.Log10(1 / convFactor), MidpointRounding.ToPositiveInfinity);
                        string valueFormat;
                        if (numberZeros == 0) {
                            valueFormat = $"{{0:+#0;-#0;0}}";
                        } else {
                            string zeros = new('0', numberZeros);
                            valueFormat = $"{{0:+#0.{zeros};-#0.{zeros};0}}";
                        }
                        return string.Format(valueFormat, (double)value * convFactor);
                }
            };

            proxyCall = (callback, excHandler) => { this.connection.sendGetCurrentValue0ForID(id, callback, excHandler); };
            EventHandler clickCurrentValueHandler = this.createClickHandlerGet(textboxValuesCurrent, proxyCall, list, formatFunc);

            formatFunc = (value) => { return $"{value}"; };
            proxyCall = (callback, excHandler) => { this.connection.sendGetErrorCodeForID(id, callback, excHandler); };
            EventHandler clickErrorCodeHandler = this.createClickHandlerGet(textboxValuesError, proxyCall, list, formatFunc);

            ctxMenu.Items.AddRange(new ToolStripItem[] {
                itemValuesCurrent,
                textboxValuesCurrent,
                separator,
                itemValuesError,
                textboxValuesError
            });
            ctxMenu.Name = ($"contextMenuCommIDValues{postfix}");
            ctxMenu.Size = this.contextMenuCommIDValues.Size;
            ctxMenu.Closed += (this.contextMenu_HideImagesClearText!);
            ctxMenu.Closing += (this.contextMenu_AbortClosing!);
            ctxMenu.Opening += (this.contextMenu_HideImagesClearText!);

            itemValuesCurrent.Name = ($"toolStripMenuItemCommIDValuesCurrent{postfix}");
            itemValuesCurrent.Size = this.toolStripMenuItemCommIDValuesCurrent.Size;
            itemValuesCurrent.Text = this.toolStripMenuItemCommIDValuesCurrent.Text;
            itemValuesCurrent.Click += clickCurrentValueHandler;

            textboxValuesCurrent.Name = ($"toolStripTextBoxCommIDValuesCurrent{postfix}");
            textboxValuesCurrent.BorderStyle = this.toolStripTextBoxCommIDValuesCurrent.BorderStyle;
            textboxValuesCurrent.ReadOnly = this.toolStripTextBoxCommIDValuesCurrent.ReadOnly;
            textboxValuesCurrent.Size = this.toolStripTextBoxCommIDValuesCurrent.Size;

            separator.Name = ($"toolStripSeparatorCommIDValues{postfix}");
            separator.Size = this.toolStripSeparatorCommIDValues.Size;

            itemValuesError.Name = ($"toolStripMenuItemCommIDValuesError{postfix}");
            itemValuesError.Size = this.toolStripMenuItemCommIDValuesError.Size;
            itemValuesError.Text = this.toolStripMenuItemCommIDValuesError.Text;
            itemValuesError.Click += clickErrorCodeHandler;

            textboxValuesError.Name = ($"toolStripTextBoxCommIDValuesError{postfix}");
            textboxValuesError.BorderStyle = this.toolStripTextBoxCommIDValuesError.BorderStyle;
            textboxValuesError.ReadOnly = this.toolStripTextBoxCommIDValuesError.ReadOnly;
            textboxValuesError.Size = this.toolStripTextBoxCommIDValuesError.Size;

            item.DropDown = ctxMenu;

            ctxMenu.ResumeLayout(false);
        }

        private void createAllCommIDValuesSubmenus() {
            List<ToolStripMenuItem> items = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };

            for (int id = 1; id <= items.Count; id++) {
                this.createCommIDValuesSubmenu(items[id - 1], id);
            }
        }

        private EventHandler createClickHandlerGet(
            ToolStripTextBox textbox,
            Action<Action<int?>?, Action<Exception>?> proxyCall,
            IList<ToolStripMenuItem>? list,
            Func<int, string> formatFunc
        ) {
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
                        this.connection.printSocketException((SocketException)ex);
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

        private void backgroundWorkerCommunication_DoWork(object sender, DoWorkEventArgs e) {
            Action action = (Action)e.Argument!;
            action();
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
                        this.connection.printSocketException((SocketException)ex);
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
                        this.connection.printSocketException((SocketException)ex);
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
                    this.connection.printSocketException((SocketException)ex);
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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommSettingsMaskShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetSensorStatusMaskSetting;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommSettingsMaskDont,
                this.toolStripMenuItemCommSettingsMaskDo,
            };
            this.highlightMenuItem(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemCommSettingsMaskDont_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommSettingsMaskShow,
                this.toolStripMenuItemCommSettingsMaskDont,
                this.toolStripMenuItemCommSettingsMaskDo,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.connection.sendSetSensorStatusMaskSetting;
            int value = 0;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCommSettingsMaskDo_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommSettingsMaskShow,
                this.toolStripMenuItemCommSettingsMaskDont,
                this.toolStripMenuItemCommSettingsMaskDo,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            Action<int, Action<string, string>?, Action<Exception>?> proxyCall = this.connection.sendSetSensorStatusMaskSetting;
            int value = 1;
            Action<string, string>? receiveFunc = null;
            this.contextMenuEntryClickWrite(menuItem, proxyCall, value, receiveFunc);
        }

        private void toolStripMenuItemCommCombinedStatusShow_Click(object sender, EventArgs e) {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedStatusDLEN1,
                this.toolStripMenuItemCommCombinedStatusBit1,
                this.toolStripMenuItemCommCombinedStatusBit2,
                this.toolStripMenuItemCommCombinedStatusBit3,
                this.toolStripMenuItemCommCombinedStatusBit4,
                this.toolStripMenuItemCommCombinedStatusBit5,
                this.toolStripMenuItemCommCombinedStatusBit6,
                this.toolStripMenuItemCommCombinedStatusBit7,
                this.toolStripMenuItemCommCombinedStatusBit8,
                this.toolStripMenuItemCommCombinedStatusBit9,
                this.toolStripMenuItemCommCombinedStatusBit10,
                this.toolStripMenuItemCommCombinedStatusBit11,
                this.toolStripMenuItemCommCombinedStatusBit12,
                this.toolStripMenuItemCommCombinedStatusBit13,
                this.toolStripMenuItemCommCombinedStatusWarning,
                this.toolStripMenuItemCommCombinedStatusError,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetStatus;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommCombinedValuesErrorID_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedValuesErrorID,
                this.toolStripMenuItemCommCombinedValuesErrorCode,
                this.toolStripMenuItemCommCombinedValuesWarningID,
                this.toolStripMenuItemCommCombinedValuesWarningCode,
                this.toolStripMenuItemCommCombinedValuesID00,
                this.toolStripMenuItemCommCombinedValuesNumber,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCommCombinedValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetErrorIDNumber;

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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommCombinedValuesErrorCode_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedValuesErrorID,
                this.toolStripMenuItemCommCombinedValuesErrorCode,
                this.toolStripMenuItemCommCombinedValuesWarningID,
                this.toolStripMenuItemCommCombinedValuesWarningCode,
                this.toolStripMenuItemCommCombinedValuesID00,
                this.toolStripMenuItemCommCombinedValuesNumber,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCommCombinedValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetErrorCode;

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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommCombinedValuesWarningID_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedValuesErrorID,
                this.toolStripMenuItemCommCombinedValuesErrorCode,
                this.toolStripMenuItemCommCombinedValuesWarningID,
                this.toolStripMenuItemCommCombinedValuesWarningCode,
                this.toolStripMenuItemCommCombinedValuesID00,
                this.toolStripMenuItemCommCombinedValuesNumber,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCommCombinedValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetWarningIDNumber;

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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommCombinedValuesWarningCode_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedValuesErrorID,
                this.toolStripMenuItemCommCombinedValuesErrorCode,
                this.toolStripMenuItemCommCombinedValuesWarningID,
                this.toolStripMenuItemCommCombinedValuesWarningCode,
                this.toolStripMenuItemCommCombinedValuesID00,
                this.toolStripMenuItemCommCombinedValuesNumber,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCommCombinedValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetWarningCode;

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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommCombinedValuesID00_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedValuesErrorID,
                this.toolStripMenuItemCommCombinedValuesErrorCode,
                this.toolStripMenuItemCommCombinedValuesWarningID,
                this.toolStripMenuItemCommCombinedValuesWarningCode,
                this.toolStripMenuItemCommCombinedValuesID00,
                this.toolStripMenuItemCommCombinedValuesNumber,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCommCombinedValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetErrorCodeID00;

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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommCombinedValuesNumber_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommCombinedValuesErrorID,
                this.toolStripMenuItemCommCombinedValuesErrorCode,
                this.toolStripMenuItemCommCombinedValuesWarningID,
                this.toolStripMenuItemCommCombinedValuesWarningCode,
                this.toolStripMenuItemCommCombinedValuesID00,
                this.toolStripMenuItemCommCombinedValuesNumber,
            };
            foreach (ToolStripMenuItem item in list) {
                item.Image = null;
            }

            ToolStripTextBox textbox = this.toolStripTextBoxCommCombinedValues;
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetSensorConnectedNumber;

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
                    this.connection.printSocketException((SocketException)ex);
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

        private void toolStripMenuItemCommIDStatusError_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetSensorErrorStatus;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusWarning_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetWarningStatus;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusValueProperty_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetCurrentValue0Property;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusValueInvalid_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetCurrentValue0Invalid;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusValueUnderRange_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetCurrentValue0UnderRange;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusValueOverRange_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetCurrentValue0OverRange;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.red_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusOuput1_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetOuput1;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemCommIDStatusOuput2_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetOuput2;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemCommIDStatusOuput3_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetOuput3;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall);
        }

        private void toolStripMenuItemCommIDStatusOuput4_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetOuput4;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall, Properties.Resources.green_circle, Properties.Resources.grey_circle);
        }

        private void toolStripMenuItemCommIDStatusOuput5_Click(object sender, EventArgs e) {
            List<ToolStripMenuItem> extraList = new() {
                this.toolStripMenuItemCommIDStatusError,
                this.toolStripMenuItemCommIDStatusWarning,
                this.toolStripMenuItemCommIDStatusValueProperty,
                this.toolStripMenuItemCommIDStatusValueInvalid,
                this.toolStripMenuItemCommIDStatusValueOverRange,
                this.toolStripMenuItemCommIDStatusValueUnderRange,
                this.toolStripMenuItemCommIDStatusOuput1,
                this.toolStripMenuItemCommIDStatusOuput2,
                this.toolStripMenuItemCommIDStatusOuput3,
                this.toolStripMenuItemCommIDStatusOuput4,
                this.toolStripMenuItemCommIDStatusOuput5,
            };
            foreach (ToolStripMenuItem item in extraList) {
                item.Image = null;
            }

            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender!;
            List<ToolStripMenuItem> list = new() {
                this.toolStripMenuItemCommID01,
                this.toolStripMenuItemCommID02,
                this.toolStripMenuItemCommID03,
                this.toolStripMenuItemCommID04,
                this.toolStripMenuItemCommID05,
                this.toolStripMenuItemCommID06,
                this.toolStripMenuItemCommID07,
                this.toolStripMenuItemCommID08,
                this.toolStripMenuItemCommID09,
                this.toolStripMenuItemCommID10,
                this.toolStripMenuItemCommID11,
                this.toolStripMenuItemCommID12,
                this.toolStripMenuItemCommID13,
                this.toolStripMenuItemCommID14,
                this.toolStripMenuItemCommID15,
            };
            Action<Action<int?>?, Action<Exception>?> proxyCall = this.connection.sendGetOuput5;
            this.highlightMenuItemsBitfield(menuItem, list, proxyCall);
        }

    }
}
