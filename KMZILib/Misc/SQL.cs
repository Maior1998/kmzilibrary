using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Npgsql;
using NpgsqlTypes;

namespace KMZILib
{
    /// <summary>
    /// Предоставляет функции для удобной работы с БД PostgreSQL. Выполняет все запросы в пределах одного соединения, поэтому многопоточное выполнение не рекомендуется.
    /// </summary>
    public class SQL : IDisposable
    {
        private readonly NpgsqlConnection conn;
        private readonly NpgsqlCommand sql_command;

        /// <summary>
        /// Инициализирует новое соединение с БД.
        /// </summary>
        /// <param name="server">Строковое представление адреса сервера с PostgreSQL на борту.</param>
        /// <param name="port">Строковое представление порта, по которому необходимо выполнить подключение.</param>
        /// <param name="username">Имя пользователя, под которым необходимо войти.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="database">Имя базы данных, к которой необходимо подключиться.</param>
        public SQL(string server, string port, string username, string password, string database)
        {
            Server = server;
            Port = port;
            Username = username;
            Database = database;
            conn = new NpgsqlConnection(
                $"Server={server};Port={port};Username={username};Password={password};Database={database}");
            sql_command = new NpgsqlCommand("", conn);
            conn.Open();
        }

        /// <summary>
        /// Инициализирует новое соединение с БД.
        /// </summary>
        /// <param name="server">Строковое представление адреса сервера с PostgreSQL на борту.</param>
        /// <param name="port">Номер порта, по которому необходимо выполнить подключение.</param>
        /// <param name="username">Имя пользователя, под которым необходимо войти.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="database">Имя базы данных, к которой необходимо подключиться.</param>
        public SQL(string server, int port, string username, string password, string database) : this(server,
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
        /// Запускает процесс деконструкции объекта соединения. Нужно для реализации интерфейса <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Выполняет запрос и ничего не возвращает.
        /// </summary>
        /// <param name="query"></param>
        public void DoQueryVoid(string query)
        {
            sql_command.CommandText = query;
            sql_command.ExecuteNonQuery();
        }

        /// <summary>
        ///     Выполняет запрос и возвращает первый столбец первой строки.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object DoQueryObj(string query)
        {
            sql_command.CommandText = query;
            object result = sql_command.ExecuteScalar();

            return result;
        }

        /// <summary>
        ///     Выполняет запрос и возвращает результата в ввиде прямоугольного массива object[,].
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object[,] DoQueryObjArr(string query)
        {
            sql_command.CommandText = query;
            NpgsqlDataReader reader = sql_command.ExecuteReader();
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
            return result;
        }

        /// <summary>
        ///     Получает названия всех таблиц базы данных.
        /// </summary>
        /// <returns></returns>
        public string[] GetTablesNames()
        {
            sql_command.CommandText =
                "SELECT * FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema';";
            NpgsqlDataReader reader = sql_command.ExecuteReader();
            List<string> Result = new List<string>();
            while (reader.Read()) Result.Add((string)reader["tablename"]);
            reader.CloseAsync();
            return Result.ToArray();
        }

        /// <summary>
        ///     Возвращает таблицу с указанным именем.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public object[,] GetTable(string Name)
        {
            return DoQueryObjArr($"select * from {Name};");
        }

        /// <summary>
        /// Выполняет произвольный запрос с участием даты и ничего не возвращает. Поле с датой должно быть помечено как ":target_date". Например: update test set test_date = :target_date where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос, который нужно выполнить.</param>
        /// <param name="Source">Дата, участвующая в запросе. В теле sql запроса должна быть помечена как ":target_date".</param>
        public void DoQueryDateVoid(string sql, DateTime Source)
        {
            sql_command.CommandText = sql;
            NpgsqlParameter param = new NpgsqlParameter(":target_date", NpgsqlDbType.Date) { Value = Source };
            sql_command.Parameters.Add(param);
            sql_command.ExecuteNonQuery();
            sql_command.Parameters.Remove(param);
        }

        /// <summary>
        /// Выполняет произвольный запрос с участием даты и возвращает элемент первой строки первого столбца результата. Поле с датой должно быть помечено как ":target_date". Например: update test set test_date = :target_date where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос, который нужно выполнить.</param>
        /// <param name="Source">Дата, участвующая в запросе. В теле sql запроса должна быть помечена как ":target_date".</param>
        /// <returns>Элемент первого столбца первой строки результата.</returns>
        public object DoQueryDateObj(string sql, DateTime Source)
        {
            sql_command.CommandText = sql;
            NpgsqlParameter param = new NpgsqlParameter(":target_date", NpgsqlDbType.Date) { Value = Source };
            sql_command.Parameters.Add(param);
            object result = sql_command.ExecuteScalar();
            sql_command.Parameters.Remove(param);
            return result;
        }

        /// <summary>
        /// Выполняет произвольный запрос с участием даты и возвращает таблицу-результат. Поле с датой должно быть помечено как ":target_date". Например: update test set test_date = :target_date where id=0;
        /// </summary>
        /// <param name="sql">SQL запрос, который нужно выполнить.</param>
        /// <param name="Source">Дата, участвующая в запросе. В теле sql запроса должна быть помечена как ":target_date".</param>
        /// <returns>Таблица-результат.</returns>
        public object[,] DoQueryDateObjArr(string sql, DateTime Source)
        {
            sql_command.CommandText = sql;
            NpgsqlParameter param = new NpgsqlParameter(":target_date", NpgsqlDbType.Date) { Value = Source };
            sql_command.Parameters.Add(param);
            NpgsqlDataReader reader = sql_command.ExecuteReader();
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
            sql_command.Parameters.Remove(param);
            return result;
        }

        ///// <summary>
        /////     Запись файла с базы.
        ///// </summary>
        ///// <param name="path">Путь для записи.</param>
        ///// <param name="work_id">ID работы.</param>
        //public void GetFile(string path, int work_id)
        //{
        //    sql_command.CommandText = $"SELECT doc FROM student_works w where w.id = {work_id}";
        //    NpgsqlDataReader dr = sql_command.ExecuteReader();
        //    byte[] result = null;
        //    if (dr.Read())
        //        result = (byte[]) dr[0];
        //    dr.Close();
        //    File.WriteAllBytes(path, result);
        //}

        ///// <summary>
        /////     Загрузка локального файла на сервер.
        ///// </summary>
        ///// <param name="id">ID работы.</param>
        ///// <param name="bytes">Байты файла.</param>
        //public void UploadFile(int id, byte[] bytes)
        //{
        //    NpgsqlParameter paramet = new NpgsqlParameter("dataParam", NpgsqlDbType.Bytea);
        //    paramet.Value = bytes;
        //    sql_command.Parameters.Add(paramet);
        //    sql_command.CommandText =
        //        "insert into student_works (doc) values (:dataParam)";
        //    sql_command.CommandText = $"update student_works set doc = :dataParam where id = {id};";
        //    try
        //    {
        //        sql_command.ExecuteNonQuery();
        //    }
        //    finally
        //    {
        //        sql_command.Parameters.Remove(paramet);
        //    }
        //}

        private void ReleaseUnmanagedResources()
        {
            if (conn.State != ConnectionState.Closed)
                conn.Close();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                conn?.Dispose();
                sql_command?.Dispose();
            }
        }
    }
}