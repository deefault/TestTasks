using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    public class HttpServer : IDisposable
    {
        private readonly int _port;
        private readonly int _maxThreads;
        private readonly SemaphoreSlim _lock;

        private readonly HttpListener _listener;
        
        [ThreadStatic]
        private static int Counter;

        public HttpServer(int port = 1234, int maxThreads = 5)
        {
            if (port <= 0) throw new ArgumentOutOfRangeException(nameof(port));
            _port = port;
            _maxThreads = maxThreads;
            _lock = new SemaphoreSlim(_maxThreads, _maxThreads);

            _listener = new HttpListener();
        }
        
        public Task StartAsync()
        {
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Start();
            Console.WriteLine("Starting processing requests. Press ctrl+c to interrupt");
            
            return StartProcessingLoop();
        }

        public void Stop()
        {
            _listener.Stop();
        }

        private async Task StartProcessingLoop()
        {
            await _lock.WaitAsync().ConfigureAwait(true);
            while (true)
            {
                HttpListenerContext context = await _listener.GetContextAsync();
#pragma warning disable 4014
                Task.Factory.StartNew(async () =>
#pragma warning restore 4014
                {
                    await ProcessRequestAsync(context).ConfigureAwait(true);
#if DEBUG
                    Console.WriteLine($"Request processed: {context.Request.RequestTraceIdentifier}");
#endif
                    _lock.Release();
                });
            }
        }


        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            Counter++;
            HttpListenerResponse response = context.Response;
            string responseString = string.Format(
                "<HTML><BODY> Thread {0}, request {1}</BODY></HTML>", Thread.CurrentThread.ManagedThreadId, Counter);

            await WriteHtml(response, responseString);

            response.Close();
        }

        private Task WriteHtml(HttpListenerResponse response, string content)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            response.Headers.Add("Content-Type", "text/html; charset=utf-8");
            response.StatusCode = 200;
            Stream output = response.OutputStream;
            
            return output.WriteAsync(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            ((IDisposable) _listener)?.Dispose();
        }
    }
}