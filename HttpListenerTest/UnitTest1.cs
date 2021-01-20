using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HttpListenerTest
{
    public class UnitTest1
    {
        private static readonly Regex _regex = new Regex(@".*Thread\s+(?<thread>[0-9]+),\s*request\s+(?<count>[0-9]+).*", RegexOptions.Compiled);

        [Theory]
        [InlineData(2, 10)]
        [InlineData(100, 10000)]
        public async Task Test1(int maxServer, int maxRequests)
        {
            var server = new HttpServer.HttpServer(1234, maxServer);
            var serverThread = new Thread(async () => await server.StartAsync());
            serverThread.Start();
            var client = new HttpClient(){BaseAddress = new Uri("http://localhost:1234/")};

            var requests = Enumerable.Repeat<Func<Task<HttpResponseMessage>>>(() => client.GetAsync("/"), maxRequests).Select(x => x.Invoke()).ToList();
            await Task.WhenAll(requests);

            Dictionary<int, List<int>> requestsByThread = new Dictionary<int, List<int>>();
            foreach (var r in requests.Select(x => x.Result.Content.ReadAsStringAsync()))
            {
                var str = await r; ;
                var match = _regex.Match(str);
                Assert.True(match.Success);
                ParseAndAddToDict(match, requestsByThread);
            }

            // проверка счетчика
            foreach (var thread in requestsByThread.Keys)
            {
                var max = requestsByThread[thread].Max();
                var actual = requestsByThread[thread].OrderBy(x => x);
                var expected = Enumerable.Range(1, max);
                Assert.Equal(expected, actual);
            }
        }

        private static void ParseAndAddToDict(Match match, Dictionary<int, List<int>> requestsByThread)
        {
            var threadId = Convert.ToInt32(match.Groups["thread"].Value);
            var req = Convert.ToInt32(match.Groups["count"].Value);
            if (!requestsByThread.ContainsKey(threadId))
            {
                requestsByThread.Add(threadId, new List<int>() {req});
            }
            else
            {
                requestsByThread[threadId].Add(req);
            }
        }
    }
}
