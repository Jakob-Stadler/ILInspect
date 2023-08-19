using Microsoft.Data.Sqlite;
using System.Data;

namespace ILInspect {

    public class Database {
        private string dataSource;
        private SqliteConnection connection;
        private MessageCollection messageCollection;

        public Database(string dataSource, MessageCollection messageCollection) {
            this.dataSource = dataSource;
            this.messageCollection = messageCollection;
            SqliteConnectionStringBuilder connectionStringBuilder = new() {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = dataSource
            };
            this.connection = new SqliteConnection(connectionStringBuilder.ToString());
            this.connection.Open();
        }

        ~Database() {
            this.connection.Close();
        }

        public void initTable() {
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText =
                @"CREATE TABLE IF NOT EXISTS data (
                    measurement_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    view TEXT,
                    unit_id TEXT,
                    datetime TEXT
                  );
                  CREATE TABLE IF NOT EXISTS headers (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    column_name TEXT UNIQUE,
                    display_name TEXT UNIQUE
                  );";
                command.ExecuteNonQuery();
            }

            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText =
                @"INSERT INTO headers (column_name, display_name)
                    VALUES ('measurement_id', 'ID');
                  INSERT INTO headers (column_name, display_name)
                    VALUES ('view', 'View');
                  INSERT INTO headers (column_name, display_name)
                    VALUES ('unit_id', 'Unit');
                  INSERT INTO headers (column_name, display_name)
                    VALUES ('datetime', 'Time (UTC)');";
                try {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex) {
                    if (ex.SqliteErrorCode == 19) {
                        // Non-Unique column
                        return;
                    }
                    throw;
                }
            }
        }

        public void addColumn(string name) {
            int lastRowId; // Attention! SQL Injection potential!
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText =
                @"INSERT INTO headers (column_name, display_name)
                    VALUES (null, $name);
                  SELECT last_insert_rowid()";
                command.Parameters.AddWithValue("$name", name);
                try {
                    lastRowId = Convert.ToInt32(command.ExecuteScalar());
                }
                catch (SqliteException ex) {
                    if (ex.SqliteErrorCode == 19) {
                        // Non-Unique column
                        return;
                    }
                    throw;
                }
            }
            string colName = $"col{lastRowId}"; // Attention! SQL Injection potential!
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText =
                @"UPDATE headers
                  SET column_name = $col
                  WHERE rowid = $rowId";
                command.Parameters.AddWithValue("rowId", lastRowId);
                command.Parameters.AddWithValue("$col", colName);
                command.ExecuteNonQuery();
            }
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText = $"ALTER TABLE data ADD COLUMN {colName} TEXT"; // Attention! SQL Injection potential!
                command.ExecuteNonQuery();
            }
        }

        public void changeColumn(string oldName, string newName) {
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText =
                @"UPDATE headers
                  SET display_name = $col
                  WHERE display_name = $name AND rowid > 4";
                command.Parameters.AddWithValue("name", oldName);
                command.Parameters.AddWithValue("$col", newName);
                command.ExecuteNonQuery();
            }
        }

        public string? getHeaderFromDisplayName(string displayName) {
            // This function is used for generating dynamic SQL queries
            // Be careful so that it never returns user input!
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText = @"SELECT column_name FROM headers WHERE display_name = $displayName LIMIT 1";
                command.Parameters.AddWithValue("$displayName", displayName);
                return command.ExecuteScalar()?.ToString();
            }
        }

        public string? getDisplayNameFromHeader(string columnName) {
            // This function return user input!
            // Never use it for dynamic SQL queries!
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText = @"SELECT display_name FROM headers WHERE column_name = $columnName LIMIT 1";
                command.Parameters.AddWithValue("columnName", columnName);
                return command.ExecuteScalar()?.ToString();
            }
        }

        public void insertData(string view, string unitId, IDictionary<string, string> data) {
            if (data.Count == 0) {
                return;
            }
            string columns = string.Empty; // Attention! SQL Injection potential!
            string values = string.Empty; // Attention! SQL Injection potential!
            for (int i = 0; i < data.Count; i++) {
                string? key = data.ElementAt(i).Key;
                if (key == null)
                    continue;
                string? columnName = this.getHeaderFromDisplayName(key); // Attention! SQL Injection potential!
                if (columnName == null)
                    continue;
                columns += $", {columnName}"; // Attention! SQL Injection potential!
                values += $", $val{i}"; // Attention! SQL Injection potential!
            }
            // Not a fan of dynamically creating SQL queries, tbh...
            string query =
                @$"INSERT INTO data (view, unit_id, datetime{columns})
                    VALUES ($view, $unitId, datetime('now'){values});";
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText = query;
                command.Parameters.AddWithValue("$view", view);
                command.Parameters.AddWithValue("$unitId", unitId);
                for (int i = 0; i < data.Count; i++) {
                    KeyValuePair<string, string> kvp = data.ElementAt(i);
                    command.Parameters.AddWithValue($"$val{i}", kvp.Value);
                }
                try {
                    command.ExecuteNonQuery();
                }
                catch (SqliteException ex) {
                    this.messageCollection.addLine($"Can't execute query! {ex.Message}");
                }
            }
        }

        public DataTable provideDataTable() {
            DataTable dataTable = new();
            using (SqliteCommand command = this.connection.CreateCommand()) {
                command.CommandText = "SELECT * FROM data ORDER BY measurement_id DESC";
                dataTable.Load(command.ExecuteReader());
            }
            return dataTable;
        }
    }
}
