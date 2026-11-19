using System;
using System.Data;
using System.Windows.Forms;
using MySqlConnector;

namespace ComputerStore
{
    internal static class Db
    {
        // Стандартные параметры Open Server. При другой конфигурации измените их здесь.
        private const string ConnectionString = "Server=127.0.0.1;Port=3306;Database=computer_store;User ID=root;Password=;Character Set=utf8mb4;";

        public static MySqlConnection Open()
        {
            var connection = new MySqlConnection(ConnectionString);
            try { connection.Open(); return connection; }
            catch { connection.Dispose(); throw; }
        }

        public static DataTable Query(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = Open())
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddRange(parameters);
                using (var reader = command.ExecuteReader())
                {
                    var table = new DataTable(); table.Load(reader); return table;
                }
            }
        }

        public static int Execute(string sql, params MySqlParameter[] parameters)
        {
            using (var connection = Open())
            using (var command = new MySqlCommand(sql, connection))
            { command.Parameters.AddRange(parameters); return command.ExecuteNonQuery(); }
        }

        public static void ShowError(Exception ex)
        {
            var mysql = ex as MySqlException;
            var message = ex.Message;
            if (mysql != null && mysql.Number == 1451) message = "Запись используется. Удаление запрещено.";
            if (mysql != null && mysql.Number == 1062) message = "Такая запись уже существует.";
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
