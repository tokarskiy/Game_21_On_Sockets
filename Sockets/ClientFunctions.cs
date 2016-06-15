using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketsLab
{
	public static class Client
	{
		static int score;
		static Random random;
		public const int MAX_RANDOM = 10;
		public static void Start(int port)
		{
			random = new Random(DateTime.Now.Millisecond);
			score = random.Next(MAX_RANDOM);
			byte[] bytes = new byte[1024];

			IPHostEntry ipHost = Dns.GetHostEntry("localhost");
			IPAddress ipAddress = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
			Console.WriteLine("Количество очков: {0, 2}.", score);
			Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			sender.Connect(ipEndPoint);
			while (true)
			{
				// Игрок берет карту
				Console.Write("Вы берёте карту? (y/n): ");
				string message = Console.ReadLine();
				if (message == "y")
				{
					score += random.Next(MAX_RANDOM);
				}

				byte[] messageBytes = Encoding.UTF8.GetBytes(message);

				sender.Send(messageBytes);
				Console.WriteLine("Количество очков: {0, 2}.", score);
				// Противник берет карту
				Console.WriteLine("Ожидание хода противника");
				int bytesReceive = sender.Receive(bytes);
				string messageReceive = Encoding.UTF8.GetString(bytes, 0, bytesReceive);
				Console.WriteLine(messageReceive == "y" ? "Противник взял карту" : "Противник не взял карту");


				if (messageReceive == "n" && message == "n")
				{
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
					CompareScores(ipHost, ipAddress, ipEndPoint);
					return;
				}
			}
		}

		public static void CompareScores(IPHostEntry ipHost, IPAddress ipAddress, IPEndPoint ipEndPoint)
		{
			Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			sender.Connect(ipEndPoint);

			// Отправить свой счет
			byte[] messageBytes = Encoding.UTF8.GetBytes(score.ToString());
			sender.Send(messageBytes);

			// Получить результат от сервера
			byte[] resultBytes = new byte[1024];
			int bytesReceive = sender.Receive(resultBytes);
			string result = Encoding.UTF8.GetString(resultBytes, 0, bytesReceive);
			Console.WriteLine(result);

			sender.Shutdown(SocketShutdown.Both);
			sender.Close();
		}
	}
}

