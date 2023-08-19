using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ILInspect {

    public class Connection {
        private string host;
        private int port;
        private MessageCollection messageCollection;
        private TcpClient client;
        public Dictionary<string, SensorProxy> proxies;
        private object _connectionLock = new();
        private BackgroundWorker sendingWorker;
        private Queue<(string, Action<string>, Action<Exception>)> sendingQueue;
        private IPEndPoint? cachedEndpoint = null;
        private ConnectionConfig config;

        public Connection(ConnectionConfig config, MessageCollection messageCollection) {
            this.config = config;
            this.host = config.Host;
            this.port = config.Port;
            this.messageCollection = messageCollection;
            this.client = new TcpClient();
            this.proxies = new Dictionary<string, SensorProxy>();
            this.sendingWorker = new BackgroundWorker();
            this.sendingWorker.DoWork += this.worker_DoWork!;
            this.sendingQueue = new Queue<(string, Action<string>, Action<Exception>)>();
        }

        ~Connection() {
            this.sendingWorker.CancelAsync();
            if (this.client.Connected) {
                this.disconnect();
            }
        }

        public bool StopLaser {
            get {
                return this.config.StopLasers;
            }
        }

        public string Address {
            get {
                if (!this.client.Connected) {
                    return string.Empty;
                }
                IPEndPoint endPoint = (this.client.Client.RemoteEndPoint as IPEndPoint)!;
                string address;
                if (endPoint.Address.IsIPv4MappedToIPv6) {
                    address = $"{endPoint.Address.MapToIPv4()}:{endPoint.Port}";
                } else {
                    address = $"{endPoint.Address}:{endPoint.Port}";
                }
                return address;
            }
        }

        public SensorProxy? getProxyByID(int id) {
            if (id <= 0 || id > this.proxies.Count) {
                return null;
            }
            return this.proxies.ElementAt(id - 1).Value;
        }

        public void printSocketException(SocketException ex) {
            if (ex.ErrorCode == 10061) {
                this.messageCollection.addLine($"{ex.Message}");
                return;
            }
            this.messageCollection.addLine(ex.ToString());
        }

        public void addSensor(string boxId, SensorProxy sensor) {
            this.proxies.Add(boxId, sensor);
        }

        public void connect() {
            // Use cached Enpoint if available to skip DNS roundtrip the second time.
            if (this.cachedEndpoint == null) {
                this.client = new TcpClient(this.host, this.port);
                this.cachedEndpoint = this.client.Client.RemoteEndPoint as IPEndPoint;
            } else {
                this.client = new TcpClient();
                this.client.Connect(this.cachedEndpoint);
            }
        }

        public void disconnect() {
            if (this.client.Connected) this.client.Close();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            while (!this.sendingWorker.CancellationPending) {
                if (this.sendingQueue.TryDequeue(out (string message, Action<string> callback, Action<Exception> excHandler) item)) {
                    this._sendMessage(item.message, item.callback, item.excHandler);
                } else {
                    Thread.Sleep(100);
                }
            }
        }

        public void backgroundSend(string message, Action<string> callback, Action<Exception> excHandler) {
            this.sendingQueue.Enqueue((message, callback, excHandler));
            if (!this.sendingWorker.IsBusy) {
                this.sendingWorker.RunWorkerAsync();
            }
        }

        private void _sendMessage(string message, Action<string> callback, Action<Exception> excHandler) {
            try {
                lock (this._connectionLock) // Only allow one caller at a time
                {
                    if (!this.client.Connected) {
                        this.connect();
                    }

                    // Translate the passed message into ASCII and store it as a Byte array.
                    byte[] data = Encoding.ASCII.GetBytes($"{message}\r\n");

                    // Get a client stream for reading and writing.
                    NetworkStream stream = this.client.GetStream();

                    // Send the message to the connected TcpServer.
                    stream.Write(data, 0, data.Length);

                    this.messageCollection.addLine($"{this.Address}>> {message}");

                    // Receive the server response.

                    // Buffer to store the response bytes.
                    data = new byte[256];

                    // String to store the response ASCII representation.
                    string responseData = string.Empty;

                    // Read the first batch of the TcpServer response bytes.
                    int bytes = stream.Read(data, 0, data.Length);
                    responseData = Encoding.ASCII.GetString(data, 0, bytes);

                    this.messageCollection.addMessage($"{this.Address}<< {responseData}");
                    // Execute callback
                    callback(responseData.TrimEnd());

                    if (!this.config.stayConnected) {
                        this.disconnect();
                    }
                }
            }
            catch (Exception ex) {
                excHandler(ex);
            }
        }

        public void sendMessage(string message, Action<string> callback, Action<Exception> excHandler) {
            this.backgroundSend(message, callback, excHandler);
        }

        private void receiveResponseMS(string response) {
            // Expected Format:
            // MS,<status>,<value>,<status>,<value>,...,<status>,<value>
            string[] subs = response.Split(',');
            if (subs.Length <= 1) {
                this.messageCollection.addLine($"Invalid response: {response}");
                return;
            }
            if (subs[0] != "MS") {
                this.messageCollection.addLine($"Invalid response: {response}");
                return;
            }
            for (int index = 0; 2 * index + 1 < subs.Length; index++) {
                if (index >= this.proxies.Count) {
                    break;
                }
                string strStatusCode = subs[2 * index + 1];
                string strValue = subs[2 * index + 2];
                int statusCode = int.Parse(strStatusCode);
                int value = int.Parse(strValue);
                KeyValuePair<string, SensorProxy> proxypair = this.proxies.ElementAt(index);
                SensorProxy proxy = proxypair.Value;
                proxy.status = statusCode;
                proxy.value = value;
            }
        }

        private void updateAllMSWithLaserSwitching(Action afterUpdateCallback, Action<Exception> excHandler) {
            DoWorkEventHandler work = (sender, e) => {
                this.unsetAllLaserStops();
                Thread.Sleep(this.config.LaserTimeout);
                this.updateAllMSNoLaserSwitching(afterUpdateCallback, excHandler);
                this.setAllLaserStops();
            };
            BackgroundWorker worker = new();
            worker.DoWork += work;
            worker.RunWorkerAsync();
        }

        private void updateAllMSNoLaserSwitching(Action afterUpdateCallback, Action<Exception> excHandler) {
            Action<string> callback = (response) => {
                this.receiveResponseMS(response);
                afterUpdateCallback();
            };
            this.sendMessage("MS", callback, excHandler);
        }

        public void updateAllMS(Action afterUpdateCallback, Action<Exception> excHandler) {
            if (this.config.StopLasers) {
                this.updateAllMSWithLaserSwitching(afterUpdateCallback, excHandler);
            } else {
                this.updateAllMSNoLaserSwitching(afterUpdateCallback, excHandler);
            }
        }

        public void unsetAllLaserStops(Action<string, string>? callback = null, Action<Exception>? excHandler = null) {
            CountdownEvent countdown = new(1);

            Action<string, string> modifiedCallback = (response, expectedResponse) => {
                if (callback != null) {
                    callback(response, expectedResponse);
                } else {
                    this.genericWriteReceive(response, expectedResponse);
                }
                countdown.Signal();
            };

            Action<Exception> modifiedExcHandler = (ex) => {
                if (excHandler != null) {
                    excHandler(ex);
                } else {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.printSocketException((SocketException)ex);
                    }
                }
                countdown.Signal();
            };

            foreach (KeyValuePair<string, SensorProxy> proxypair in this.proxies) {
                SensorProxy proxy = proxypair.Value;
                countdown.AddCount();
                proxy.sendSetLaserEmissionStop(0, modifiedCallback, modifiedExcHandler);
            }
            countdown.Signal();
            countdown.Wait();  // Synchronize
        }

        public void setAllLaserStops(Action<string, string>? callback = null, Action<Exception>? excHandler = null) {
            CountdownEvent countdown = new(1);

            Action<string, string> modifiedCallback = (response, expectedResponse) => {
                if (callback != null) {
                    callback(response, expectedResponse);
                } else {
                    this.genericWriteReceive(response, expectedResponse);
                }
                countdown.Signal();
            };

            Action<Exception> modifiedExcHandler = (ex) => {
                if (excHandler != null) {
                    excHandler(ex);
                } else {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.printSocketException((SocketException)ex);
                    }
                }
                countdown.Signal();
            };

            foreach (KeyValuePair<string, SensorProxy> proxypair in this.proxies) {
                SensorProxy proxy = proxypair.Value;
                countdown.AddCount();
                proxy.sendSetLaserEmissionStop(1, modifiedCallback, modifiedExcHandler);
            }
            countdown.Signal();
            countdown.Wait();  // Synchronize
        }



        public void genericSendWriteCommand(
            int command,
            int value,
            Action<string, string>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            string messagePrefix = $"SW,00,{command:000}";
            string message = $"{messagePrefix},{value:+000000000;-000000000}";
            if (receiveFunc == null) {
                receiveFunc = this.genericWriteReceive;
            }
            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.printSocketException((SocketException)ex);
                    }
                };
            }
            Action<string> responseFunc = (response) => { receiveFunc(response, messagePrefix); };
            this.sendMessage(message, responseFunc, excHandler);
        }

        public void genericWriteReceive(string response, string expectedResponse) {
            if (response != expectedResponse) {
                this.messageCollection.addLine(
                    $"Response does not match expectation!\r\n" +
                    $"Expected: {expectedResponse}\r\n" +
                    $"Received: {response}"
                 );
            }
        }

        public void genericSendReadCommand(
            int command,
            Action<int?>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            string messagePrefix = $"SR,00,{command:000}";
            string message = messagePrefix;
            if (receiveFunc == null) {
                receiveFunc = this.genericReadReceive;
            }
            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.printSocketException((SocketException)ex);
                    }
                };
            }
            Action<string> responseFunc = this.createReadReceiveCallback(messagePrefix, receiveFunc);
            this.sendMessage(message, responseFunc, excHandler);
        }

        public void genericReadReceive(int? value) {
            this.messageCollection.addLine($"Received: {value}");
        }

        public Action<string> createReadReceiveCallback(string expectedPrefix, Action<int?> action) {
            Action<string> callback = (response) => {
                int? value = this.parseReadReceive(response, expectedPrefix);
                action(value);
            };
            return callback;
        }

        private int? parseReadReceive(string response, string expectedPrefix) {
            if (!response.StartsWith(expectedPrefix)) {
                this.messageCollection.addLine(
                    $"Response does not match expectation!\r\n" +
                    $"Expected: {expectedPrefix}\r\n" +
                    $"Received: {response}"
                 );
                return null;
            }
            string strValue = response.Substring(expectedPrefix.Length + 1);
            if (int.TryParse(strValue, out int value)) {
                return value;
            } else {
                this.messageCollection.addLine($"Unable to parse response value: {response}");
                return null;
            }
        }

        public void sendGetStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(0, callback, excHandler);
        }

        public void sendGetSensorErrorStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(1, callback, excHandler);
        }

        public void sendGetWarningStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(2, callback, excHandler);
        }

        public void sendGetCurrentValue0Property(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(4, callback, excHandler);
        }

        public void sendGetErrorIDNumber(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(8, callback, excHandler);
        }

        public void sendGetErrorCode(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(9, callback, excHandler);
        }

        public void sendGetWarningIDNumber(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(10, callback, excHandler);
        }

        public void sendGetWarningCode(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(11, callback, excHandler);
        }

        public void sendGetOuput1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(16, callback, excHandler);
        }

        public void sendGetOuput2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(17, callback, excHandler);
        }

        public void sendGetOuput3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(18, callback, excHandler);
        }

        public void sendGetOuput4(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(19, callback, excHandler);
        }

        public void sendGetOuput5(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(20, callback, excHandler);
        }

        public void sendGetCurrentValue0Invalid(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(38, callback, excHandler);
        }

        public void sendGetCurrentValue0UnderRange(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(39, callback, excHandler);
        }

        public void sendGetCurrentValue0OverRange(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(40, callback, excHandler);
        }

        public void sendGetCurrentValue0ForID(int id, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (id < 1 || id > 15) {
                throw new ArgumentOutOfRangeException("id");
            }
            int command = 43 + id;
            this.genericSendReadCommand(command, callback, excHandler);
        }

        public void sendGetSensorStatusMaskSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(76, callback, excHandler);
        }

        public void sendSetSensorStatusMaskSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(76, value, receiveFunc, excHandler);
        }

        public void sendGetSensorConnectedNumber(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(77, callback, excHandler);
        }

        public void sendGetErrorCodeForID(int id, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (id < 0 || id > 15) {
                throw new ArgumentOutOfRangeException("id");
            }
            int command = 668 + id;
            this.genericSendReadCommand(command, callback, excHandler);
        }

        public void sendGetErrorCodeID00(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(668, callback, excHandler);
        }

    }

    public class SensorProxy {
        private int hardwareIndex;
        public SensorConfig config;
        public int status;
        public double? value;
        public Connection connection;
        private MessageCollection messageCollection;

        public SensorProxy(SensorConfig config, int hardwareIndex, Connection connection, MessageCollection messageCollection) {
            this.config = config;
            this.hardwareIndex = hardwareIndex;
            this.connection = connection;
            this.messageCollection = messageCollection;
            this.status = 0;
            this.value = null;
        }

        public void printSocketException(SocketException ex) {
            this.connection.printSocketException(ex);
        }

        public void genericSendWriteCommand(
            int command,
            int value,
            Action<string, string>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            string messagePrefix = $"SW,{this.hardwareIndex:00},{command:000}";
            string message = $"{messagePrefix},{value:+000000000;-000000000}";
            if (receiveFunc == null) {
                receiveFunc = this.genericWriteReceive;
            }
            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.connection.printSocketException((SocketException)ex);
                    }
                };
            }
            Action<string> responseFunc = (response) => { receiveFunc(response, messagePrefix); };
            this.connection.sendMessage(message, responseFunc, excHandler);
        }

        public void genericWriteReceive(string response, string expectedResponse) {
            if (response != expectedResponse) {
                this.messageCollection.addLine(
                    $"Response does not match expectation!\r\n" +
                    $"Expected: {expectedResponse}\r\n" +
                    $"Received: {response}"
                 );
            }
        }

        public void genericSendReadCommand(
            int command,
            Action<int?>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            string messagePrefix = $"SR,{this.hardwareIndex:00},{command:000}";
            string message = messagePrefix;
            if (receiveFunc == null) {
                receiveFunc = this.genericReadReceive;
            }
            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.connection.printSocketException((SocketException)ex);
                    }
                };
            }
            Action<string> responseFunc = this.createReadReceiveCallback(messagePrefix, receiveFunc);
            this.connection.sendMessage(message, responseFunc, excHandler);
        }

        public void genericReadReceive(int? value) {
            this.messageCollection.addLine($"Received: {value}");
        }

        public Action<string> createReadReceiveCallback(string expectedPrefix, Action<int?> action) {
            Action<string> callback = (response) => {
                int? value = this.parseReadReceive(response, expectedPrefix);
                action(value);
            };
            return callback;
        }

        private int? parseReadReceive(string response, string expectedPrefix) {
            if (!response.StartsWith(expectedPrefix)) {
                this.messageCollection.addLine(
                    $"Response does not match expectation!\r\n" +
                    $"Expected: {expectedPrefix}\r\n" +
                    $"Received: {response}"
                 );
                return null;
            }
            string strValue = response.Substring(expectedPrefix.Length + 1);
            if (int.TryParse(strValue, out int value)) {
                return value;
            } else {
                this.messageCollection.addLine($"Unable to parse response value: {response}");
                return null;
            }
        }

        public void stringSendReadCommand(
            int command,
            Action<string?>? receiveFunc = null,
            Action<Exception>? excHandler = null
        ) {
            string messagePrefix = $"SR,{this.hardwareIndex:00},{command:000}";
            string message = messagePrefix;
            if (receiveFunc == null) {
                receiveFunc = this.stringReadReceive;
            }
            if (excHandler == null) {
                excHandler = (ex) => {
                    if (typeof(SocketException).IsAssignableFrom(ex.GetType())) {
                        this.connection.printSocketException((SocketException)ex);
                    }
                };
            }
            Action<string> responseFunc = this.createStringReadReceiveCallback(messagePrefix, receiveFunc);
            this.connection.sendMessage(message, responseFunc, excHandler);
        }

        public void stringReadReceive(string? value) {
            this.messageCollection.addLine($"Received: {value}");
        }

        public Action<string> createStringReadReceiveCallback(string expectedPrefix, Action<string?> action) {
            Action<string> callback = (response) => {
                string? value = this.parseStringReadReceive(response, expectedPrefix);
                action(value);
            };
            return callback;
        }

        private string? parseStringReadReceive(string response, string expectedPrefix) {
            if (!response.StartsWith(expectedPrefix)) {
                this.messageCollection.addLine(
                    $"Response does not match expectation!\r\n" +
                    $"Expected: {expectedPrefix}\r\n" +
                    $"Received: {response}"
                 );
                return null;
            }
            string strValue = response.Substring(expectedPrefix.Length + 1);
            return strValue;
        }

        public void sendZeroShiftExecutionRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(1, value, receiveFunc, excHandler);
        }

        public void sendZeroShiftResetRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(2, value, receiveFunc, excHandler);
        }

        public void sendResetRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(3, value, receiveFunc, excHandler);
        }

        public void sendInitialResetRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(5, value, receiveFunc, excHandler);
        }

        public void sendSystemParametersSetRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(6, value, receiveFunc, excHandler);
        }

        public void sendToleranceTuningRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(14, value, receiveFunc, excHandler);
        }

        public void sendTwoPointHIGHFirstPointRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(15, value, receiveFunc, excHandler);
        }

        public void sendTwoPointHIGHSecondPointRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(16, value, receiveFunc, excHandler);
        }

        public void sendTwoPointLOWFirstPointRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(17, value, receiveFunc, excHandler);
        }

        public void sendTwoPointLOWSecondPointRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(18, value, receiveFunc, excHandler);
        }

        public void sendCalibrationSet1Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(19, value, receiveFunc, excHandler);
        }

        public void sendCalibrationSet2Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(20, value, receiveFunc, excHandler);
        }

        public void sendCalculationTwoPointCalibrationSet1Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(21, value, receiveFunc, excHandler);
        }

        public void sendCalculationTwoPointCalibrationSet2Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(22, value, receiveFunc, excHandler);
        }

        public void sendCalculationThreePointCalibrationSet1Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(23, value, receiveFunc, excHandler);
        }

        public void sendCalculationThreePointCalibrationSet2Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(24, value, receiveFunc, excHandler);
        }

        public void sendCalculationThreePointCalibrationSet3Request(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(25, value, receiveFunc, excHandler);
        }

        public void sendDiffCountFilterOnePointTuningRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(26, value, receiveFunc, excHandler);
        }

        public void sendDiffCountFilterTwoPointFirstPointRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(27, value, receiveFunc, excHandler);
        }

        public void sendDiffCountFilterTwoPointSecondPointRequest(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(28, value, receiveFunc, excHandler);
        }

        public void sendGetSensorAmplifierError(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(33, callback, excHandler);
        }

        public void sendGetJudgementAlarmOutput(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(36, callback, excHandler);
        }

        public void sendGetJudgementValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(37, callback, excHandler);
        }

        public void sendGetInternalMeasurementValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(38, callback, excHandler);
        }

        public void sendGetPeakHoldValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(39, callback, excHandler);
        }

        public void sendGetBottomHoldValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(40, callback, excHandler);
        }

        public void sendGetCalculationValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(41, callback, excHandler);
        }

        public void sendGetAnalogOutputValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(42, callback, excHandler);
        }

        public void sendGetBankStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(43, callback, excHandler);
        }

        public void sendGetTimingStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(44, callback, excHandler);
        }

        public void sendGetLaserEmissionStopStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(50, callback, excHandler);
        }

        public void sendGetAbnormalSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(51, callback, excHandler);
        }

        public void sendGetExternalInputStatus(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(52, callback, excHandler);
        }

        public void sendGetEEPROMWriteResult(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(53, callback, excHandler);
        }

        public void sendGetZeroShiftResult(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(54, callback, excHandler);
        }

        public void sendGetResetRequestResult(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(55, callback, excHandler);
        }

        public void sendGetCurrentSystemParameters(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(56, callback, excHandler);
        }

        public void sendGetTuningResult(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(60, callback, excHandler);
        }

        public void sendGetCalibrationResult(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(61, callback, excHandler);
        }

        private void sendGetHIGH(int bank, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendReadCommand(65 + bank * 5, callback, excHandler);
            }
        }

        public void sendSetHIGH(int bank, int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendWriteCommand(65 + bank * 5, value, receiveFunc, excHandler);
            }
        }

        private void sendGetLOW(int bank, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendReadCommand(66 + bank * 5, callback, excHandler);
            }
        }

        public void sendSetLOW(int bank, int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendWriteCommand(66 + bank * 5, value, receiveFunc, excHandler);
            }
        }

        private void sendGetShiftTarget(int bank, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendReadCommand(67 + bank * 5, callback, excHandler);
            }
        }

        public void sendSetShiftTarget(int bank, int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendWriteCommand(67 + bank * 5, value, receiveFunc, excHandler);
            }
        }

        private void sendGetAnalogOutputUpperLimit(int bank, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendReadCommand(68 + bank * 5, callback, excHandler);
            }
        }

        public void sendSetAnalogOutputUpperLimit(int bank, int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendWriteCommand(68 + bank * 5, value, receiveFunc, excHandler);
            }
        }
        private void sendGetAnalogOutputLowerLimit(int bank, Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendReadCommand(69 + bank * 5, callback, excHandler);
            }
        }

        public void sendSetAnalogOutputLowerLimit(int bank, int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            if (bank >= 0 && bank <= 3) {
                this.genericSendWriteCommand(69 + bank * 5, value, receiveFunc, excHandler);
            }
        }

        public void sendGetHIGHBank0(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetHIGH(0, callback, excHandler);
        }

        public void sendSetHIGHBank0(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetHIGH(0, value, receiveFunc, excHandler);
        }

        public void sendGetLOWBank0(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetLOW(0, callback, excHandler);
        }

        public void sendSetLOWBank0(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetLOW(0, value, receiveFunc, excHandler);
        }

        public void sendGetShiftBank0(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetShiftTarget(0, callback, excHandler);
        }

        public void sendSetShiftBank0(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetShiftTarget(0, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputUpperLimitBank0(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputUpperLimit(0, callback, excHandler);
        }

        public void sendSetAnalogOutputUpperLimitBank0(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputUpperLimit(0, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputLowerLimitBank0(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputLowerLimit(0, callback, excHandler);
        }

        public void sendSetAnalogOutputLowerLimitBank0(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputLowerLimit(0, value, receiveFunc, excHandler);
        }

        public void sendGetHIGHBank1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetHIGH(1, callback, excHandler);
        }

        public void sendSetHIGHBank1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetHIGH(1, value, receiveFunc, excHandler);
        }

        public void sendGetLOWBank1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetLOW(1, callback, excHandler);
        }

        public void sendSetLOWBank1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetLOW(1, value, receiveFunc, excHandler);
        }

        public void sendGetShiftBank1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetShiftTarget(1, callback, excHandler);
        }

        public void sendSetShiftBank1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetShiftTarget(1, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputUpperLimitBank1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputUpperLimit(1, callback, excHandler);
        }

        public void sendSetAnalogOutputUpperLimitBank1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputUpperLimit(1, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputLowerLimitBank1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputLowerLimit(1, callback, excHandler);
        }

        public void sendSetAnalogOutputLowerLimitBank1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputLowerLimit(1, value, receiveFunc, excHandler);
        }

        public void sendGetHIGHBank2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetHIGH(2, callback, excHandler);
        }

        public void sendSetHIGHBank2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetHIGH(2, value, receiveFunc, excHandler);
        }

        public void sendGetLOWBank2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetLOW(2, callback, excHandler);
        }

        public void sendSetLOWBank2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetLOW(2, value, receiveFunc, excHandler);
        }

        public void sendGetShiftBank2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetShiftTarget(2, callback, excHandler);
        }

        public void sendSetShiftBank2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetShiftTarget(2, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputUpperLimitBank2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputUpperLimit(2, callback, excHandler);
        }

        public void sendSetAnalogOutputUpperLimitBank2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputUpperLimit(2, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputLowerLimitBank2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputLowerLimit(2, callback, excHandler);
        }

        public void sendSetAnalogOutputLowerLimitBank2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputLowerLimit(2, value, receiveFunc, excHandler);
        }

        public void sendGetHIGHBank3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetHIGH(3, callback, excHandler);
        }

        public void sendSetHIGHBank3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetHIGH(3, value, receiveFunc, excHandler);
        }

        public void sendGetLOWBank3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetLOW(3, callback, excHandler);
        }

        public void sendSetLOWBank3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetLOW(3, value, receiveFunc, excHandler);
        }

        public void sendGetShiftBank3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetShiftTarget(3, callback, excHandler);
        }

        public void sendSetShiftBank3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetShiftTarget(3, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputUpperLimitBank3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputUpperLimit(3, callback, excHandler);
        }

        public void sendSetAnalogOutputUpperLimitBank3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputUpperLimit(3, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputLowerLimitBank3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.sendGetAnalogOutputLowerLimit(3, callback, excHandler);
        }

        public void sendSetAnalogOutputLowerLimitBank3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.sendSetAnalogOutputLowerLimit(3, value, receiveFunc, excHandler);
        }

        public void sendGetKeyLock(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(97, callback, excHandler);
        }

        public void sendSetKeyLock(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(97, value, receiveFunc, excHandler);
        }

        public void sendGetBank(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(98, callback, excHandler);
        }

        public void sendSetBank(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(98, value, receiveFunc, excHandler);
        }

        public void sendGetTimingInput(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(99, callback, excHandler);
        }

        public void sendSetTimingInput(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(99, value, receiveFunc, excHandler);
        }

        public void sendGetLaserEmissionStop(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(100, callback, excHandler);
        }

        public void sendSetLaserEmissionStop(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(100, value, receiveFunc, excHandler);
        }

        public void sendGetSubDisplayScreen(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(104, callback, excHandler);
        }

        public void sendSetSubDisplayScreen(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(104, value, receiveFunc, excHandler);
        }

        public void sendGetSystemParameterSettings(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(105, callback, excHandler);
        }

        public void sendSetSystemParameterSettings(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(105, value, receiveFunc, excHandler);
        }

        public void sendGetToleranceTuningRange(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(106, callback, excHandler);
        }

        public void sendSetToleranceTuningRange(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(106, value, receiveFunc, excHandler);
        }

        public void sendGetCalibrationFunction(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(107, callback, excHandler);
        }

        public void sendSetCalibrationFunction(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(107, value, receiveFunc, excHandler);
        }

        public void sendGetCalibrationFunctionSet1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(108, callback, excHandler);
        }

        public void sendSetCalibrationFunctionSet1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(108, value, receiveFunc, excHandler);
        }

        public void sendGetCalibrationFunctionSet2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(109, callback, excHandler);
        }

        public void sendSetCalibrationFunctionSet2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(109, value, receiveFunc, excHandler);
        }

        public void sendGetCalculatedValueCalibrationFunction(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(110, callback, excHandler);
        }

        public void sendSetCalculatedValueCalibrationFunction(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(110, value, receiveFunc, excHandler);
        }

        public void sendGetCalculatedValueTwoPointCalibrationSet1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(111, callback, excHandler);
        }

        public void sendSetCalculatedValueTwoPointCalibrationSet1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(111, value, receiveFunc, excHandler);
        }

        public void sendGetCalculatedValueTwoPointCalibrationSet2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(112, callback, excHandler);
        }

        public void sendSetCalculatedValueTwoPointCalibrationSet2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(112, value, receiveFunc, excHandler);
        }

        public void sendGetCalculatedValueThreePointCalibrationSet1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(113, callback, excHandler);
        }

        public void sendSetCalculatedValueThreePointCalibrationSet1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(113, value, receiveFunc, excHandler);
        }

        public void sendGetCalculatedValueThreePointCalibrationSet3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(114, callback, excHandler);
        }

        public void sendSetCalculatedValueThreePointCalibrationSet3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(114, value, receiveFunc, excHandler);
        }

        public void sendGetCalculationFunction(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(129, callback, excHandler);
        }

        public void sendSetCalculationFunction(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(129, value, receiveFunc, excHandler);
        }

        public void sendGetMeasurementDirection(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(131, callback, excHandler);
        }

        public void sendSetMeasurementDirection(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(131, value, receiveFunc, excHandler);
        }

        public void sendGetSamplingCycle(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(132, callback, excHandler);
        }

        public void sendSetSamplingCycle(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(132, value, receiveFunc, excHandler);
        }

        public void sendGetFilterSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(133, callback, excHandler);
        }

        public void sendSetFilterSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(133, value, receiveFunc, excHandler);
        }

        public void sendGetOutputMode(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(134, callback, excHandler);
        }

        public void sendSetOutputMode(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(134, value, receiveFunc, excHandler);
        }

        public void sendGetHoldFunctionSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(136, callback, excHandler);
        }

        public void sendSetHoldFunctionSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(136, value, receiveFunc, excHandler);
        }

        public void sendGetAutoHoldTriggerSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(137, callback, excHandler);
        }

        public void sendSetAutoHoldTriggerSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(137, value, receiveFunc, excHandler);
        }

        public void sendGetTimingInputSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(138, callback, excHandler);
        }

        public void sendSetTimingInputSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(138, value, receiveFunc, excHandler);
        }

        public void sendGetDelayTimer(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(139, callback, excHandler);
        }

        public void sendSetDelayTimer(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(139, value, receiveFunc, excHandler);
        }

        public void sendGetTimerDuration(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(140, callback, excHandler);
        }

        public void sendSetTimerDuration(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(140, value, receiveFunc, excHandler);
        }

        public void sendGetHysteresis(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(141, callback, excHandler);
        }

        public void sendSetHysteresis(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(141, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputScaling(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(142, callback, excHandler);
        }

        public void sendSetAnalogOutputScaling(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(142, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputUpperLimitValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(143, callback, excHandler);
        }

        public void sendSetAnalogOutputUpperLimitValue(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(143, value, receiveFunc, excHandler);
        }

        public void sendGetAnalogOutputLowerLimitValue(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(144, callback, excHandler);
        }

        public void sendSetAnalogOutputLowerLimitValue(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(144, value, receiveFunc, excHandler);
        }

        public void sendGetExternalInputSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(145, callback, excHandler);
        }

        public void sendSetExternalInputSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(145, value, receiveFunc, excHandler);
        }

        public void sendGetExternalInput1(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(146, callback, excHandler);
        }

        public void sendSetExternalInput1(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(146, value, receiveFunc, excHandler);
        }

        public void sendGetExternalInput2(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(147, callback, excHandler);
        }

        public void sendSetExternalInput2(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(147, value, receiveFunc, excHandler);
        }

        public void sendGetExternalInput3(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(148, callback, excHandler);
        }

        public void sendSetExternalInput3(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(148, value, receiveFunc, excHandler);
        }

        public void sendGetExternalInput4(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(149, callback, excHandler);
        }

        public void sendSetExternalInput4(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(149, value, receiveFunc, excHandler);
        }

        public void sendGetBankShiftMethod(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(150, callback, excHandler);
        }

        public void sendSetBankShiftMethod(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(150, value, receiveFunc, excHandler);
        }

        public void sendGetZeroShiftMemoryFunction(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(152, callback, excHandler);
        }

        public void sendSetZeroShiftMemoryFunction(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(152, value, receiveFunc, excHandler);
        }

        public void sendGetMutualInterferencePreventionFunction(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(153, callback, excHandler);
        }

        public void sendSetMutualInterferencePreventionFunction(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(153, value, receiveFunc, excHandler);
        }

        public void sendGetDisplayDigit(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(154, callback, excHandler);
        }

        public void sendSetDisplayDigit(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(154, value, receiveFunc, excHandler);
        }

        public void sendGetPowerSaving(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(155, callback, excHandler);
        }

        public void sendSetPowerSaving(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(155, value, receiveFunc, excHandler);
        }

        public void sendGetHeadDisplayMode(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(156, callback, excHandler);
        }

        public void sendSetHeadDisplayMode(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(156, value, receiveFunc, excHandler);
        }

        public void sendGetDisplayColor(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(157, callback, excHandler);
        }

        public void sendSetDisplayColor(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(157, value, receiveFunc, excHandler);
        }

        public void sendGetTimerDurationDiffCountFilter(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(158, callback, excHandler);
        }

        public void sendSetTimerDurationDiffCountFilter(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(158, value, receiveFunc, excHandler);
        }

        public void sendGetCutoffFrequencyHighPassFilter(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(159, callback, excHandler);
        }

        public void sendSetCutoffFrequencyHighPassFilter(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(159, value, receiveFunc, excHandler);
        }

        public void sendGetAlarmSetting(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(161, callback, excHandler);
        }

        public void sendSetAlarmSetting(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(161, value, receiveFunc, excHandler);
        }

        public void sendGetAlarmCount(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(162, callback, excHandler);
        }

        public void sendSetAlarmCount(int value, Action<string, string>? receiveFunc = null, Action<Exception>? excHandler = null) {
            this.genericSendWriteCommand(162, value, receiveFunc, excHandler);
        }


        public void sendGetProductCode(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(193, callback, excHandler);
        }

        public void sendGetRevision(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(194, callback, excHandler);
        }

        public void sendGetConnectedSensorHead(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(195, callback, excHandler);
        }

        public void sendGetProductName(Action<string?>? callback = null, Action<Exception>? excHandler = null) {
            this.stringSendReadCommand(200, callback, excHandler);
        }

        public void sendGetSeriesCode(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(215, callback, excHandler);
        }

        public void sendGetSeriesVersion(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(216, callback, excHandler);
        }

        public void sendGetDeviceType(Action<int?>? callback = null, Action<Exception>? excHandler = null) {
            this.genericSendReadCommand(217, callback, excHandler);
        }
    }
}
