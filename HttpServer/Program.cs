using System;
using System.Threading.Tasks;

namespace HttpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var simpleServer = new HttpServer();

            Console.WriteLine("Server starting...");
            await simpleServer.StartAsync();
            Console.WriteLine("Server stopped");
            
        }
    }
}