using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KMZILib
{
    /// <summary>
    /// Предоставляет методы для удобной работы с TCP потоком данных.
    /// </summary>
    public class TCPClient : IDisposable
    {

        private string HOST;
        private int PORT = 33004;
        private int TIMEOUT = 500;
        private readonly NetworkStream Stream = null;
        private readonly TcpClient InnerClient;

        /// <summary>
        /// Инициализирует новый объект TCP соединения и потока с заданными настройками.
        /// </summary>
        /// <param name="Host">Адрес TCP сервера, к которому будет произведено подключение.</param>
        /// <param name="Port">Порт TCP сервера, к которому будет произведено подключение.</param>
        /// <param name="Timeout">Таймаут подключения.</param>
        public TCPClient(string Host, int Port, int Timeout = 500)
        {
            InnerClient = new TcpClient();
            InnerClient.Connect(HOST = Host, PORT = Port);
            Stream = InnerClient.GetStream();
            Stream.ReadTimeout = TIMEOUT = Timeout;
        }

        /// <summary>
        /// Отправляет строку на сервер.
        /// </summary>
        /// <param name="request">Строка, которую необходимо послать на сервер.</param>
        public void Send(string request)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(request + "\n");
            Stream.Write(data, 0, data.Length);
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
                int bytes = Stream.Read(data, 0, data.Length);
                response.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable); // пока данные есть в потоке
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
        /// ЗАкрывает соединения и высвобождает их ресурсы.
        /// </summary>
        public void Dispose()
        {
            Stream?.Dispose();
            InnerClient?.Dispose();
        }
    }
}
