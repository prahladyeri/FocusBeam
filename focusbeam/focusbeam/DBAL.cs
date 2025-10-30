/**
 * DBAL.cs - Class Definition
 * 
 * @author Prahlad Yeri <prahladyeri@yahoo.com>
 * @license MIT
 */
using focusbeam.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace focusbeam
{
    public static class DBAL
    {
        private static SQLiteConnection conn = null;
        public static string LastError = "";
        private static readonly Dictionary<Type, PropertyInfo[]> _propertyCache = 
            new Dictionary<Type, PropertyInfo[]>();


        public static void Dispose() {
            conn.Dispose();
            conn = null;
        }

        public static bool Init(string dbPath, string initSql) {
            bool isnew = false; string result;
            if (!File.Exists(dbPath)) isnew = true;
            conn = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            conn.Open();
            //using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
            //{
            //    result = (string)cmd.ExecuteScalar();
            //}
            if (!string.Equals(result, "wal", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Failed to set WAL mode on SQLite database.");
            }
            if (DateTime.Now.Date.Day == 1) {
                using (var cmd = new SQLiteCommand("vacuum;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            if (isnew)
            {
                using (var cmd = new SQLiteCommand(initSql, conn))
                {
                    cmd.ExecuteNonQuery(); // a bunch of DDL/DML statements.
                }
            }
            return isnew;
        }


        private static PropertyInfo[] GetCachedProperties(Type type)
        {
            if (!_propertyCache.TryGetValue(type, out var props))
            {
                props = type.GetProperties();
                _propertyCache[type] = props;
            }
            return props;
        }

        public static string ObjectToInsertQuery<T>(T obj, string tblName = "")
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = typeof(T);
            tblName = string.IsNullOrWhiteSpace(tblName) ? type.Name : tblName;

            var props = type.GetProperties()
                .Where(p => p.CanRead && p.GetValue(obj) != null) // exclude nulls
                .Where(p => p.Name != "Id") // exclude Id field
                .ToList();

            var columnNames = new List<string>();
            var paramNames = new List<string>();

            for (int i = 0; i < props.Count; i++)
            {
                string column = props[i].Name;
                columnNames.Add(column);
                paramNames.Add($"@param{i}");
            }

            string colPart = string.Join(", ", columnNames);
            string valPart = string.Join(", ", paramNames);

            return $"INSERT INTO {tblName} ({colPart}) VALUES ({valPart});";
        }

        public static string ObjectToUpdateQuery<T>(T obj, string keyField = "Id", string tblName = "")
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = typeof(T);
            tblName = string.IsNullOrWhiteSpace(tblName) ? type.Name : tblName;

            var props = type.GetProperties()
                .Where(p => p.CanRead && p.GetValue(obj) != null)
                .ToList();

            var assignments = new List<string>();
            string whereClause = "";

            int paramIndex = 0;
            for (int i = 0; i < props.Count; i++)
            {
                string name = props[i].Name;
                string param = $"@param{paramIndex}";

                if (string.Equals(name, keyField, StringComparison.OrdinalIgnoreCase))
                {
                    whereClause = $"WHERE {name} = {param}";
                }
                else
                {
                    assignments.Add($"{name} = {param}");
                }

                paramIndex++;
            }
            //if (string.IsNullOrWhiteSpace(whereClause))
            //    throw new InvalidOperationException($"Key field '{keyField}' not found or is null in object.");

            string setPart = string.Join(", ", assignments);
            return $"UPDATE {tblName} SET {setPart} {whereClause};";
        }

        public static object[] GetParamValues<T>(T obj, bool includeId = true)
        {
            return typeof(T).GetProperties()
                .Where(p => p.CanRead && (includeId ? true : p.Name != "Id") &&  p.GetValue(obj) != null)
                .Select(p => p.GetValue(obj))
                .ToArray();
        }

        public static T DataRowToObject<T>(DataRow row) where T : new()
        {
            T obj = new T();
            //var props = typeof(T).GetProperties();
            var props = GetCachedProperties(typeof(T));

            foreach (var prop in props)
            {
                if (!prop.CanWrite)
                    continue;

                if (row.Table.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                {
                    try
                    {
                        //var value = Convert.ChangeType(row[prop.Name], prop.PropertyType);
                        Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        var value = Convert.ChangeType(row[prop.Name], targetType);
                        prop.SetValue(obj, value);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return obj;
        }


        public static int ExecuteNonQuery(string sql, object[] args = null) 
        {
            try
            {
                LastError = "";
                args = (args == null ? Array.Empty<object>() : args);
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i}", args[i]);
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {
                Logger.WriteLog("DBAL ERROR: " + ex.ToString());
                if (ex is SQLiteException sqliteEx && sqliteEx.ResultCode == SQLiteErrorCode.Constraint)
                {
                    LastError = "Duplicate entry or constraint violation.";
                }
                else {
                    LastError = $"Error occurred: {ex.Message}";
                }
                return -1;
            }
        }

        public static DataTable Execute(string sql, object[] args = null) 
        {
            args = (args == null ? Array.Empty<object>() : args);
            LastError = "";
            try {
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        cmd.Parameters.AddWithValue($"@param{i}", args[i]);
                    }
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                        }
                        while (reader.Read())
                        {
                            var row = dt.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                                row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);
                            dt.Rows.Add(row);
                        }
                        return dt;
                    }
                }
            }
            catch(Exception ex) {
                LastError = LastError = $"Error occurred: {ex.Message}";
                return null;
            }
        }

        public static object ExecuteScalar(string sql) {
            using (var cmd = new SQLiteCommand(sql, conn)) {
                var result= cmd.ExecuteScalar();
                return result == DBNull.Value ? null : result;
            }
        }

    }
}
