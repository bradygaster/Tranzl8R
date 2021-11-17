using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Placement;

namespace Tranzl8R.TranslationService
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IGrainFactory grainFactory;

        public Worker(ILogger<Worker> logger, IGrainFactory grainFactory)
        {
            _logger = logger;
            this.grainFactory = grainFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var translationServerGrain = grainFactory.GetGrain<ITranslationServer>(Guid.Empty);
                (await translationServerGrain.GetAllLanguages())
                    .Where(x => x.IsTranslatorReady == false)
                    .OrderBy(x => Guid.NewGuid())
                    .Take(20)
                    .ToList()
                    .ForEach(async (language) =>
                    {
                        await translationServerGrain.ToggleLanguageActiveStatus(language.Code);
                        _logger.LogInformation($"Translator for {language.Code} ready.");
                    });
            }
            catch (Exception exception)
            {
                _logger.LogError("Error during start-up.", exception);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask; 
        }
    }
}