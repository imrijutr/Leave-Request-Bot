using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
//using Microsoft.Bot.Builder.AI.LuisV3;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;

namespace CoreBot
{

    public class LusRecognizer : IRecognizer
    {
        private readonly LuisRecognizer _recognizer;

        public LusRecognizer(IConfiguration configuration)
        {
            var luisIsConfigured = !string.IsNullOrEmpty(configuration["LuisAppId"]) && !string.IsNullOrEmpty(configuration["LuisAPIKey"]) && !string.IsNullOrEmpty(configuration["LuisAPIHostName"]);
            if (luisIsConfigured)
            {
                var luisApplication = new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    "https://" + configuration["LuisAPIHostName"]);

                var recognizerOptions = new LuisRecognizerOptionsV3(luisApplication)
                {
                    PredictionOptions = new Microsoft.Bot.Builder.AI.LuisV3.LuisPredictionOptions
                    {
                        IncludeInstanceData = true,
                    }
                };

                _recognizer = new LuisRecognizer(recognizerOptions);
            }
        }
        // Returns true if luis is configured in the appsettings.json and initialized.
        public virtual bool IsConfigured => _recognizer != null;

        public virtual async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            => await _recognizer.RecognizeAsync(turnContext, cancellationToken);

        public virtual async Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken)
            where T : IRecognizerConvert, new()
            => await _recognizer.RecognizeAsync<T>(turnContext, cancellationToken);
    }
}