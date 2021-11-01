// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with CoreBot .NET Template version v4.14.1.2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace CoreBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly LusRecognizer _luisRecognizer;
        //public QnAMaker qnAMakerObject { get; private set; }

        // Dependency injection uses this constructor to instantiate MainDialog  
        public MainDialog(LusRecognizer luisRecognizer)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            //qnAMakerObject = new QnAMaker(endpoint);

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {

               IntroStepAsync,
               ActStepAsync,
               FinalStepAsync,
            }));

            // The initial child Dialog to run.  
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                // await stepContext.Context.SendActivityAsync( MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);  

                return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)  
            var luisResult = await _luisRecognizer.RecognizeAsync<CognitiveModel>(stepContext.Context, cancellationToken);

            switch (luisResult.TopIntent().intent)
            {
                case CognitiveModel.Intent.LeaveRequest:
                    //Do processing.  
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Leave forwarded to HR Department. Kindly wait for the reply in email"), cancellationToken);
                    break;

                case CognitiveModel.Intent.None:
                    Activity reply = ((Activity)stepContext.Context.Activity).CreateReply();
                    reply.Text = $"😢 **Sorry!!! I found nothing** \n\n Please try to rephrase your query.";
                    await stepContext.Context.SendActivityAsync(reply);
                    break;

                default:
                    {
                        break;
                    }


            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Shows a warning if the requested From or To cities are recognized as entities but they are not in the Airport entity list.
        // In some cases LUIS will recognize the From and To composite entities as a valid cities but the From and To Airport values
        // will be empty if those entity values can't be mapped to a canonical item in the Airport.
        // private static async Task ShowWarningForUnsupportedCities(ITurnContext context, FlightBooking luisResult, CancellationToken cancellationToken)
        // {
        //     var unsupportedCities = new List<string>();

        //     var fromEntities = luisResult.FromEntities;
        //     if (!string.IsNullOrEmpty(fromEntities.From) && string.IsNullOrEmpty(fromEntities.Airport))
        //     {
        //         unsupportedCities.Add(fromEntities.From);
        //     }

        //     var toEntities = luisResult.ToEntities;
        //     if (!string.IsNullOrEmpty(toEntities.To) && string.IsNullOrEmpty(toEntities.Airport))
        //     {
        //         unsupportedCities.Add(toEntities.To);
        //     }

        //     if (unsupportedCities.Any())
        //     {
        //         var messageText = $"Sorry but the following airports are not supported: {string.Join(',', unsupportedCities)}";
        //         var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
        //         await context.SendActivityAsync(message, cancellationToken);
        //     }
        // }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
        }
    }
}
