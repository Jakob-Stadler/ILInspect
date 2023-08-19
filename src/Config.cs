using System.Text.Json.Serialization;

namespace ILInspect {

    public class JSONConfig {

        public static JSONConfig createDefault() {
            return new JSONConfig() {
                SensorConnections = new List<ConnectionConfig>(),
                Views = new List<ViewConfig>(),
                Database = new() { Source = ":memory:" }
            };
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("$schema")]
        public string Schema {
            get {
                return "file:///" + Path.GetFullPath("./data/schema.json", AppDomain.CurrentDomain.BaseDirectory);
            }
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("sensor_connections")]
        public required IList<ConnectionConfig> SensorConnections {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("views")]
        public required IList<ViewConfig> Views {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("database")]
        public required DatabaseConfig Database {
            get; set;
        }


        [JsonIgnore]
        public string? ConfigDirectory {
            get; set;
        } = null;


    }


    public class ViewConfig {

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("name")]
        public required string Name {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("image_location")]
        public required string ImageLocation {
            get; set;
        }


        [JsonIgnore]
        public Image? Image {
            get; set;
        } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("sensors")]
        public required IList<SensorUsageConfig> Sensors {
            get; set;
        } = new List<SensorUsageConfig>();


    }


    public class SensorUsageConfig : ICloneable {

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("id")]
        public required string ID {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("pos_x")]
        public required double PositionX {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("pos_y")]
        public required double PositionY {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("bank")]
        public int? Bank { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("shift")]
        public double? ShiftTarget { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("high")]
        public double? HighThreshold { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("low")]
        public double? LowThreshold { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("analog_upper")]
        public double? AnalogUpperBound { get; set; } = null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("analog_lower")]
        public double? AnalogLowerBound { get; set; } = null;

        public object Clone() {
            return new SensorUsageConfig() {
                ID = this.ID,
                PositionY = this.PositionY,
                PositionX = this.PositionX,
                Bank = this.Bank,
                ShiftTarget = this.ShiftTarget,
                HighThreshold = this.HighThreshold,
                LowThreshold = this.LowThreshold,
                AnalogUpperBound = this.AnalogUpperBound,
                AnalogLowerBound = this.AnalogLowerBound,
            };
        }
    }


    public class SensorConfig : ICloneable {

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("id")]
        public required string ID {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("conversion_factor")]
        public required double ConversionFactor { get; set; } = 1.0;


        public object Clone() {
            return new SensorConfig() {
                ID = this.ID,
                ConversionFactor = this.ConversionFactor
            };
        }
    }


    public class ConnectionConfig {

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("host")]
        public required string Host {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("port")]
        public required int Port {
            get; set;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("stay_connected")]
        public bool stayConnected { get; set; } = false;


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("stop_lasers")]
        public bool StopLasers { get; set; } = false;


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("laser_startup_wait_time")]
        public int LaserTimeout { get; set; } = 1000;


        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("sensors")]
        public required IList<SensorConfig> Sensors {
            get; set;
        } = new List<SensorConfig>();


    }


    public class DatabaseConfig {

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("source")]
        public required string Source { get; set; } = ":memory:";


    }
}