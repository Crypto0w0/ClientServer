using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    static void Main()
    {
        try
        {
            Random rnd = new Random();
            string[] phrases = {"Very nice<EOF>", "So funny<EOF>", "Bye<EOF>", "No way<EOF>"};
            
            byte[] bytes = new Byte[1024];
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostEntry.AddressList[0];
            
            Console.WriteLine("Enter the port");
            int p = Convert.ToInt32(Console.ReadLine());
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, p);
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            
            listener.Bind(localEndPoint);
            listener.Listen(10);
            
            Console.WriteLine("Waiting for connection");
            Socket handler = listener.Accept();
            while (true)
            {
                Console.WriteLine("Enter the message");
                string data = Console.ReadLine();
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }

                if (data == "Bye")
                {
                    Console.WriteLine($"Text recieved: {data}");
                    handler.Shutdown(SocketShutdown.Both);
                    break;
                }

                Console.WriteLine($"Text recieved: {data}");
                byte[] msg = Encoding.ASCII.GetBytes(phrases[rnd.Next(phrases.Length - 1)]);
                byte[] exc = Encoding.ASCII.GetBytes("Bye<EOF>");
                handler.Send(msg);
                if (msg == exc)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}