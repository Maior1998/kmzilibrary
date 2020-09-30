using System;
using System.Net.Sockets;
using System.Text;

namespace OSULib.Misc
{
    /// <summary>
    /// Предоставляет методы для удобной работы с TCP потоком данных.
    /// </summary>
    public class TCPClient : IDisposable
    {
        /// <summary>
        /// Адрес TCP сервера, к которому произведено подключение.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Порт TCP сервера, к которому произведено подключение.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Таймаут подключения.
        /// </summary>
        public int Timeout { get; }

        private readonly NetworkStream stream;
        private readonly TcpClient innerClient;

        /// <summary>
        /// Инициализирует новый объект TCP соединения и потока с заданными настройками.
        /// </summary>
        /// <param name="Host">Адрес TCP сервера, к которому будет произведено подключение.</param>
        /// <param name="Port">Порт TCP сервера, к которому будет произведено подключение.</param>
        /// <param name="Timeout">Таймаут подключения.</param>
        public TCPClient(string Host, int Port, int Timeout = 500)
        {
            innerClient = new TcpClient();
            innerClient.Connect(this.Host = Host, this.Port = Port);
            stream = innerClient.GetStream();
            stream.ReadTimeout = this.Timeout = Timeout;
        }

        /// <summary>
        /// Отправляет строку на сервер.
        /// </summary>
        /// <param name="request">Строка, которую необходимо послать на сервер.</param>
        public void Send(string request)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(request + "\n");
            stream.Write(data, 0, data.Length);
            Console.WriteLine(request);
        }

        /// <summary>
        /// Возвращает строку, прилетевшую с сервера.
        /// </summary>
        /// <returns>Строка - ответ от TCP сервера.</returns>
        public string Read()
        {
            byte[] data = new byte[256];
            StringBuilder response = new StringBuilder();
            do
            {
                int bytes = stream.Read(data, 0, data.Length);
                response.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable); // пока данные есть в потоке
            string result = response.ToString().Trim(new char[] { '\n' });
            Console.WriteLine("Server: " + result);
            return result;
        }

        /// <summary>
        /// Отправляет на сервер запрос и возвращает его ответ.
        /// </summary>
        /// <param name="request">Ответ на TCP запрос.</param>
        public string SendAndRead(string request)
        {
            Send(request);
            return Read();
        }

        /// <summary>
        /// Закрывает соединения и высвобождает их ресурсы.
        /// </summary>
        public void Dispose()
        {
            stream?.Dispose();
            innerClient?.Dispose();
        }
    }
}
