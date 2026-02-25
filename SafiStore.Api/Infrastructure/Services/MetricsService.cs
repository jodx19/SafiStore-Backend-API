using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SafiStore.Api.Infrastructure.Services
{
    public class MetricsService
    {
        private static long _requestCount = 0;
        private static long _totalResponseMs = 0;

        public static void RecordRequest(long elapsedMs)
        {
            Interlocked.Increment(ref _requestCount);
            Interlocked.Add(ref _totalResponseMs, elapsedMs);
        }

        public static object GetMetrics()
        {
            var count = Interlocked.Read(ref _requestCount);
            var total = Interlocked.Read(ref _totalResponseMs);
            return new { requestCount = count, averageResponseMs = count == 0 ? 0 : total / count };
        }
    }
}
