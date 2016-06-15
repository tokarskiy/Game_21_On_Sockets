using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Sockets
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1-server\n0-client");
            switch (Convert.ToInt32(Console.ReadLine()))
            {
                case 0:
                    try
                    {
                        Client.Start(11000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        Console.ReadLine();
                    }
                    break;
                case 1:
                    try
                    {
                        Server.Start(11000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        Console.ReadLine();
                    }
                    break;
                default:
                    return;
            }
        }
    }
}
