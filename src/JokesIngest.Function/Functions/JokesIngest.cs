using JokesIngest.Function.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Function.Functions
{
    public class JokesIngest
    {
        private readonly JokesProcessor _jokesProcessor;
        private readonly ILogger _logger;

        public JokesIngest(ILoggerFactory loggerFactory, JokesProcessor jokesProcessor)
        {
            _jokesProcessor = jokesProcessor;
            _logger = loggerFactory.CreateLogger<JokesIngest>();
        }

        [Function("JokesIngest")]
        public async Task Run([TimerTrigger("%TriggerSchedule%", RunOnStartup = true)] TriggerInfo triggerTimer)
        {
            await _jokesProcessor.IngestNextBatch();
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {triggerTimer.ScheduleStatus.Next}");
        }
    }
}
