using Cahuil.Bot.Dialogs.APInter.Customer;
using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerNoIdOwner : ComponentDialog
    {
        #region Constructors
        public APInterCustomerNoIdOwner()
        {
            AddDialog(new TextPrompt(nameof(TextPrompt), APInterCustomerNoIdOwnerValidatorAsync));
            AddDialog(new APInterCustomerNoIdAddress());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterCustomerNoIdOwnerDialog,
                APInterCustomerNoIdOwnerResolverDialog,
                APInterCustomerNoIdEnd
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterCustomerNoIdOwnerDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Ingrese por favor el nombre del titular del servicio.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> APInterCustomerNoIdOwnerResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = $"<b>Titular: </b> {stepContext.Result} <br/>";
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerNoIdAddress), stepContext.Options, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterCustomerNoIdEnd(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync(new CustomDialogResult() { Success = true, Value = "", Origin = "NoId" }, cancellationToken);
        }

        #endregion

        #region Validator
        private Task<bool> APInterCustomerNoIdOwnerValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }
        #endregion
    }
}
