using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace KMZILib
{
    /// <summary>
    /// Представляет собой объект соединения с БД MySQL.
    /// </summary>
    public class MySQLConnection : IDisposable
    {
        private readonly MySqlConnection connection;
        private readonly MySqlCommand command;

        /// <summary>
        /// Инициализирует новое соединение с БД.
        /// </summary>
        /// <param name="server">Строковое представление адреса сервера с MySQL на борту.</param>
        /// <param name="port">Строковое представление порта, по которому необходимо выполнить подключение.</param>
        /// <param name="user">Имя пользователя, под которым необходимо войти.</param>
        /// <param name="pass">Пароль пользователя.</param>
        /// <param name="database">Имя базы данных, к которой необходимо подключиться.</param>
        public MySQLConnection(string server, string port, string database, string user, string pass)
        {
            Server = server;
            Port = port;
            Username = user;
            Database = database;
            connection = new MySqlConnection($"server={server};port={port};user={user};database={database};password={pass};");
            command = new MySqlCommand("", connection);
            connection.Open();
        }


        /// <summary>
        /// Инициализирует новое соединение с БД.
        /// </summary>
        /// <param name="server">Строковое представление адреса сервера с MySQL на борту.</param>
        /// <param name="port">Номер порта, по которому необходимо выполнить подключение.</param>
        /// <param name="username">Имя пользователя, под которым необходимо войти.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="database">Имя базы данных, к которой необходимо подключиться.</param>
        public MySQLConnection(string server, int port, string username, string password, string database) : this(server,
            port.ToString(), username, password, database)
        {

        }

        /// <summary>
        /// Адрес сервера БД, к которому произведено подключение.
        /// </summary>
        public string Server { get; }

        /// <summary>
        /// Порт сервера БД, к которому произведено подключение.
        /// </summary>
        public string Port { get; }

        /// <summary>
        /// Имя базы данных, к которой произведено подключение.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Имя пользователя, под которым было произведено подключение.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Выполняет запрос и ничего не возвращает.
        /// </summary>
        /// <param name="query"></param>
        public void DoQueryVoid(string query)
        {

            DoQueryVoidWithParams(query, new MySqlDbType[0], new object[0]);
        }

        /// <summary>
        /// Выполняет запрос и возвращает первый столбец первой строки.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object DoQueryObj(string query)
        {
            return DoQueryObjWithParams(query, new MySqlDbType[0], new object[0]);
        }

        /// <summary>
        /// Выполняет запрос и возвращает результата в ввиде прямоугольного массива object[,].
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object[,] DoQueryObjArr(string query)
        {
            return DoQueryObjArrWithParams(query, new MySqlDbType[0], new object[0]);
        }

        /// <summary>
        /// Получает названия всех таблиц базы данных.
        /// </summary>
        /// <returns></returns>
        public string[] GetTablesNames()
        {
            command.CommandText =
                "SELECT * FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema';";
            MySqlDataReader reader = command.ExecuteReader();
            List<string> Result = new List<string>();
            while (reader.Read()) Result.Add((string)reader["tablename"]);
            reader.Close();
            return Result.ToArray();
        }

        /// <summary>
        /// Возвращает таблицу с указанным именем.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public object[,] GetTable(string Name)
        {
            return DoQueryObjArr($"select * from {Name};");
        }

        /// <summary>
        /// Выполняет произвольный запрос с параметрами заданных типов MySqlDbType и не возвращает результат. Параметры в запросе должны быть помечены как "@param0", "@param1" и т.д.
        /// Например: update test set test_date = @param0 file = @param1 where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос к БД.</param>
        /// <param name="types">Тип параметра на стороне MySqlDbType.</param>
        /// <param name="Sources">Параметр, который необходимо прикрепить к запросу.</param>
        public void DoQueryVoidWithParams(string sql, MySqlDbType[] types, object[] Sources)
        {
            command.CommandText = sql;
            MySqlParameter[] Paramsbuffer = new MySqlParameter[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                MySqlParameter param = new MySqlParameter($"@param{i}", types[i]) { Value = Sources[i] };
                command.Parameters.Add(param);
                Paramsbuffer[i] = param;
            }
            command.ExecuteNonQuery();
            for (int i = 0; i < types.Length; i++)
                command.Parameters.Remove(Paramsbuffer[i]);
        }

        /// <summary>
        /// Выполняет произвольный запрос с параметром заданного типа MySqlDbType и возвращает результат - первый столбец первой строки. Параметры в запросе должны быть помечены как "@param0", "@param1" и т.д.
        /// Например: update test set test_date = @param0 file = @param1 where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос к БД.</param>
        /// <param name="types">Тип параметра на стороне MySqlDbType.</param>
        /// <param name="Sources">Параметр, который необходимо прикрепить к запросу.</param>
        /// <returns>Результат выполнения операции.</returns>
        public object DoQueryObjWithParams(string sql, MySqlDbType[] types, object[] Sources)
        {
            command.CommandText = sql;
            MySqlParameter[] paramsbuffer = new MySqlParameter[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                MySqlParameter param = new MySqlParameter($"@param{i}", types[i]) { Value = Sources[i] };
                command.Parameters.Add(param);
                paramsbuffer[i] = param;
            }
            object result = command.ExecuteScalar();
            for (int i = 0; i < types.Length; i++)
                command.Parameters.Remove(paramsbuffer[i]);
            return result;
        }

        /// <summary>
        /// Выполняет произвольный запрос с параметром заданного типа MySqlDbType и возвращает результат - цельная таблица ответа БД. Параметры в запросе должны быть помечены как "@param0", "@param1" и т.д.
        /// Например: update test set test_date = @param0 file = @param1 where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос к БД.</param>
        /// <param name="types">Тип параметра на стороне MySqlDbType.</param>
        /// <param name="Sources">Параметр, который необходимо прикрепить к запросу.</param>
        /// <returns>Результат выполнения операции.</returns>
        public object[,] DoQueryObjArrWithParams(string sql, MySqlDbType[] types, object[] Sources)
        {
            command.CommandText = sql;
            MySqlParameter[] Paramsbuffer = new MySqlParameter[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                MySqlParameter param = new MySqlParameter($"@param{i}", types[i]) { Value = Sources[i] };
                command.Parameters.Add(param);
                Paramsbuffer[i] = param;
            }
            MySqlDataReader reader = command.ExecuteReader();
            List<object[]> Buffer = new List<object[]>();
            int columns_count = 0;
            while (reader.Read())
            {
                columns_count = reader.FieldCount;
                object[] rowbuffer = new object[columns_count];
                for (int i = 0; i < columns_count; i++)
                    rowbuffer[i] = reader.GetValue(i);
                Buffer.Add(rowbuffer);
            }
            reader.Close();
            object[,] result = new object[Buffer.Count, columns_count];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < columns_count; j++)
                    result[i, j] = Buffer[i][j];
            for (int i = 0; i < types.Length; i++)
                command.Parameters.Remove(Paramsbuffer[i]);
            return result;
        }

        /// <summary>
        /// Выполняет произвольный запрос с параметром заданного типа MySqlDbType и не возвращает результат. Параметр в запросе должен быть помечен как "@param0".
        /// Например: update test set test_date = @param0 where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос к БД.</param>
        /// <param name="type">Тип параметра на стороне MySqlDbType.</param>
        /// <param name="Source">Параметр, который необходимо прикрепить к запросу.</param>
        public void DoQueryVoidWithParam(string sql, MySqlDbType type, object Source)
        {
            DoQueryVoidWithParams(sql, new[] { type }, new[] { Source });
        }

        /// <summary>
        /// Выполняет произвольный запрос с параметром заданного типа MySqlDbType и возвращает результат - первый столбец первой строки. Параметр в запросе должен быть помечен как "@param0".
        /// Например: update test set test_date = @param0 where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос к БД.</param>
        /// <param name="type">Тип параметра на стороне MySqlDbType.</param>
        /// <param name="Source">Параметр, который необходимо прикрепить к запросу.</param>
        /// <returns>Результат выполнения операции.</returns>
        public object DoQueryObjWithParam(string sql, MySqlDbType type, object Source)
        {
            return DoQueryObjWithParams(sql, new[] { type }, new[] { Source });
        }

        /// <summary>
        /// Выполняет произвольный запрос с параметром заданного типа MySqlDbType и возвращает результат - цельная таблица ответа БД. Параметр в запросе должен быть помечен как "@param0".
        /// Например: update test set test_date = @param0 where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос к БД.</param>
        /// <param name="type">Тип параметра на стороне MySqlDbType.</param>
        /// <param name="Source">Параметр, который необходимо прикрепить к запросу.</param>
        /// <returns>Результат выполнения операции.</returns>
        public object[,] DoQueryObjArrWithParam(string sql, MySqlDbType type, object Source)
        {
            return DoQueryObjArrWithParams(sql, new[] { type }, new[] { Source });
        }

        /// <summary>
        /// Выполняет простой запрос и возвращает результат в виде одной таблицы в <see cref="DataSet"/>.
        /// </summary>
        /// <param name="query">SQL запрос, который необходимо выполнить.</param>
        /// <param name="binding">Привязка создаваемого <see cref="DataSet"/></param>
        /// <returns></returns>
        public DataSet DoQueryDataSet(string query, string binding)
        {
            DataSet ds = new DataSet();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
            adp.Fill(ds, binding);
            return ds;
        }

        public void Dispose()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();
            connection?.Dispose();
            command?.Dispose();
        }
    }
}
