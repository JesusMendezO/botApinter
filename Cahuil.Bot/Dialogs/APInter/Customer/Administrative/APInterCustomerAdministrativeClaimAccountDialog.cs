using Cahuil.Bot.Model;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace Cahuil.Bot.Dialogs
{
    public class APInterCustomerAdministrativeClaimAccountDialog : ComponentDialog
    {
        #region Fields
        string _consulta = "";
        #endregion

        #region Constructors
        public APInterCustomerAdministrativeClaimAccountDialog() : base(nameof(APInterCustomerAdministrativeClaimAccountDialog))
        {
            
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new APInterCustomerGenerateTicketDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ClaimAccountReasonStepAsync,
                ClaimAccountReasonResolverStepAsync,
                ClaimAccountTelephoneStepAsync,
                ClaimAccountTelephoneResolverStepAsync
            }));
            InitialDialogId = nameof(WaterfallDialog);
        }
        #endregion

        #region Steps
        private async Task<DialogTurnResult> ClaimAccountReasonStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string promptMessage = string.Format(@"Ingrese su consulta.");
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text(promptMessage),
                RetryPrompt = MessageFactory.Text("⛔ Ingreso incorrecto."),
            };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> ClaimAccountTelephoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
        private async Task<DialogTurnResult> ClaimAccountReasonResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _consulta = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }
        
        private async Task<DialogTurnResult> ClaimAccountTelephoneResolverStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            APInterBotState apInterBotState = (APInterBotState)stepContext.Options;
            apInterBotState.Title = "Dudas con su cuenta";
            apInterBotState.MessageAttentionTicket = true;
            apInterBotState.IssueId = 8;
            apInterBotState.ContactNumber = stepContext.Result.ToString();
            apInterBotState.Description = "Consulta: " + _consulta;
            return await stepContext.BeginDialogAsync(nameof(APInterCustomerGenerateTicketDialog), stepContext.Options, cancellationToken);
        }
        #endregion
    }
}
