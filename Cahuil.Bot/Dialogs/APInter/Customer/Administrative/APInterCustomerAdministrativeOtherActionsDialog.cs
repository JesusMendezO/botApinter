using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativeOtherActionsDialog : ComponentDialog
    {
        #region Fields
        string _consulta = string.Empty;
        #endregion

        #region Constructors
        public APInterCustomerAdministrativeOtherActionsDialog() : base(nameof(APInterCustomerAdministrativeOtherActionsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                OtherActionsReasonStepAsync,
                OtherActionsReasonResolverStepAsync,
                OtherActionsTelephoneStepAsync,
                OtherActionsTelephoneResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> OtherActionsReasonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Ingrese su consulta.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> OtherActionsTelephoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
        private async Task<DialogTurnResult> OtherActionsReasonResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _consulta = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> OtherActionsTelephoneResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Otras gestiones administrativas";
            apInterBotState.Description = "Consulta: " + _consulta;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.IssueId = 8;
            apInterBotState.MessageAttentionTicket = true;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
