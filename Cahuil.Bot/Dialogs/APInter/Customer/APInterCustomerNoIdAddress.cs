using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs.APInter.Customer
{
    public class APInterCustomerNoIdAddress : ComponentDialog
    {
        #region Constructors
        public APInterCustomerNoIdAddress()
        {
            AddDialog(new TextPrompt(nameof(TextPrompt), APInterCustomerNoIdValidatorAsync));
            AddDialog(new APInterNoCustomerGenerateTicketDialog());
            AddDialog(new APInterCustomerTechnicalServiceContact());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                APInterCustomerNoIdAddressDialog,
                APInterCustomerNoIdAddressResolverDialog,
                APInterCustomerNoIdDNIDialog,
                APInterCustomerNoIdDNIResolverDialog
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> APInterCustomerNoIdAddressDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Ingrese por favor al dirección del servicio.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterCustomerNoIdDNIDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"📌 Ingrese por favor el DNI del titular.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> APInterCustomerNoIdAddressResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description += $"<b>Dirección: </b> {stepContext.Result} <br/>";
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> APInterCustomerNoIdDNIResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description += $"<b>DNI: </b> {stepContext.Result} <br/>";
            return await stepContext.BeginDialogAsync(nameof(APInterNoCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion

        #region Validator
        private Task<bool> APInterCustomerNoIdValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }
        #endregion
    }
}
