using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerTechnicalServiceReasonDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerTechnicalServiceReasonDialog()
        {
            AddDialog(new TextPrompt(nameof(TextPrompt), TechnicalServiceReasonPromptValidatorAsync));
            AddDialog(new APInterCustomerTechnicalServiceContact());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                CustomerTechnicalServiceReasonDialog,
                CustomerTechnicalServiceReasonResolverDialog
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> CustomerTechnicalServiceReasonDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Ingrese su consulta o reclamo.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> CustomerTechnicalServiceReasonResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = $"<b>{apInterBotState.Title}</b><br><b>Consulta o reclamo:</b> {stepContext.Result}";
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerTechnicalServiceContact), stepContext.Options, cancellationToken);
        }
        #endregion

        #region Validator
        private Task<bool> TechnicalServiceReasonPromptValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }
        #endregion
    }
}
