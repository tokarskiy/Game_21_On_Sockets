using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SocketsLab
{
	public static class Server
	{
		static int score;
		static Random random;
		public const int MAX_RANDOM = 10;
		public static void Start(int port)
		{
			// Устанавливаем для сокета локальную конечную точку
			IPHostEntry ipHost = Dns.GetHostEntry("localhost");
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

			// Создаем сокет Tcp/Ip
			Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

			random = new Random(DateTime.Now.Millisecond);
			score = random.Next(MAX_RANDOM);

			sListener.Bind(ipEndPoint);
			sListener.Listen(10);
			Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);
			Console.WriteLine("Количество очков: {0, 2}.", score);
			// Начинаем слушать соединения
			Socket handler = sListener.Accept();
			while (true)
			{
				// Программа приостанавливается, ожидая входящее соединение
				string messageReceive = null;

				// Мы дождались клиента, пытающегося с нами соединиться
				Console.WriteLine("Ожидание хода противника");
				byte[] bytes = new byte[1024];
				int bytesRec = handler.Receive(bytes);

				messageReceive = Encoding.UTF8.GetString(bytes, 0, bytesRec);

				// Показываем данные на консоли
				Console.WriteLine(messageReceive == "y" ? "Противник взял карту" : "Противник не взял карту");

				// Отправляем ответ клиенту\
				Console.Write("Вы берёте карту? (y/n): ");
				string message = Console.ReadLine();
				byte[] bytesSend = Encoding.UTF8.GetBytes(message);
				handler.Send(bytesSend);
				if (message == "y")
				{
					score += random.Next(MAX_RANDOM);
				}

				Console.WriteLine("Количество очков: {0, 2}.", score);

				if (messageReceive == "n" && message == "n")
				{
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
					CompareScores(sListener);
					return;
				}
			}
		}

		public static void CompareScores(Socket sListener)
		{
			Socket handler = sListener.Accept();

			byte[] opponentPointsBytes = new byte[1024];
			int bytesReceive = handler.Receive(opponentPointsBytes);
			int opponentScore = Convert.ToInt32(Encoding.UTF8.GetString(opponentPointsBytes, 0, bytesReceive));

			string result = null;
			if (opponentScore > 21 && score > 21)
			{
				result = String.Format("Ничья {0}/{1}", score, opponentScore);
			}
			else if (opponentScore > 21)
			{
				result = String.Format("Сервер выиграл {0}/{1}", score, opponentScore);
			}
			else if (score > 21)
			{
				result = String.Format("Клиент выиграл {0}/{1}", score, opponentScore);
			}
			else 
			{
				if (score > opponentScore)
				{
					result = String.Format("Сервер выиграл {0}/{1}", score, opponentScore);
				}
				else if (score < opponentScore)
				{
					result = String.Format("Клиент выиграл {0}/{1}", score, opponentScore);
				}
				else
				{
					result = String.Format("Ничья {0}/{1}", score, opponentScore);
				}
			}
			Console.WriteLine(result);

			byte[] messageBytes = Encoding.UTF8.GetBytes(result);
			handler.Send(messageBytes);

			handler.Shutdown(SocketShutdown.Both);
			handler.Close();
		}
	}
}

