using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerComercialChangeSuscriptionDialog : ComponentDialog
    {
        #region Constructors
        public APInterCustomerComercialChangeSuscriptionDialog() : base(nameof(APInterCustomerComercialChangeSuscriptionDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChangeSuscriptionStepAsync,
                ChangeSuscriptionResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> ChangeSuscriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Por favor introduzca un teléfono de contacto.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> ChangeSuscriptionResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Description = "Solicitar cambio de abono";
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.Title = "Solicitar cambio de abono";
            apInterBotState.MessageAttentionTicket = true;
            apInterBotState.IssueId = 11;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
