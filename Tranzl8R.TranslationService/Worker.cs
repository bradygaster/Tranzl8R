using Orleans;

namespace Tranzl8R.TranslationService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IGrainFactory grainFactory;

        public Worker(ILogger<Worker> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            this.grainFactory = grainFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var translationServerGrain = grainFactory.GetGrain<ITranslationServer>(Guid.Empty);
            (await translationServerGrain.GetAllLanguages()).ForEach(async (language) =>
            {
                await translationServerGrain.ToggleLanguageActiveStatus(language.Code);
                _logger.LogInformation($"Translator for {language.Code} ready.");
            });

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}