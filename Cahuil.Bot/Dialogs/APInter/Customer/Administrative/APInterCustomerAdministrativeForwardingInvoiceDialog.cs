using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativeForwardingInvoiceDialog : ComponentDialog
    {
        #region Fields 
        string _mail = string.Empty;
        #endregion

        #region Constructors
        public APInterCustomerAdministrativeForwardingInvoiceDialog() : base(nameof(APInterCustomerAdministrativeForwardingInvoiceDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AdministrativeForwardingInvoiceStepAsync,
                AdministrativeForwardingInvoiceResolverStepAsync,
                AdministrativeForwardingInvoicePhoneStepAsync,
                AdministrativeForwardingInvoicePhoneResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> AdministrativeForwardingInvoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Ingrese dirección de e-mail");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> AdministrativeForwardingInvoicePhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Ingrese teléfono de contacto");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        #endregion

        #region Resolver
        private async Task<DialogTurnResult> AdministrativeForwardingInvoiceResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _mail = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> AdministrativeForwardingInvoicePhoneResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Solicitud de reenvio de su factura";
            apInterBotState.Description = "Dirección de mail: " + _mail;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.MessageAttentionTicket = true;
            apInterBotState.IssueId = 10;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }

        #endregion

    }
}
