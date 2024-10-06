using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Apinter.Dialogs.APInter.Customer
{
    public class APInterCustomerEmailDialog : ComponentDialog
    {
        public APInterCustomerEmailDialog() : base(nameof(APInterCustomerEmailDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
                {
                EmailStepAsync,
                EmailResolverDialog
            }));
            this.InitialDialogId = nameof(WaterfallDialog);
        }

        #region Steps
        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            string promptMessage = @"📧 Por favor ingrese un mail valido para poder comunicarnos.";

            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        private async Task<DialogTurnResult> EmailResolverDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.NextAsync(new CustomDialogResult() { Success = true, Value = stepContext.Result.ToString(), Origin = "Email" }, cancellationToken);
        }
    }
}
