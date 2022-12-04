using JokesIngest.Function.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace JokesIngest.Function.Functions
{
    public class JokesIngest
    {
        private readonly JokesProcessor _jokesProcessor;
        private readonly ILogger<JokesIngest> _logger;

        public JokesIngest(ILogger<JokesIngest> logger, JokesProcessor jokesProcessor)
        {
            _jokesProcessor = jokesProcessor;
            _logger = logger;
        }

        [Function("JokesIngest")]
        public async Task Run([TimerTrigger("%TriggerSchedule%", RunOnStartup = true)] TriggerInfo triggerTimer)
        {
            await _jokesProcessor.IngestNextBatch();
            _logger.LogDebug($"Timer trigger function executed at: {DateTime.UtcNow}");
        }
    }
}
